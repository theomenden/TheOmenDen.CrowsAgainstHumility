using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Controllers;
public sealed class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly ILogger<AccountController> _logger;

    private readonly IDataProtectionProvider _dataProtectionProvider;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _dataProtectionProvider = dataProtectionProvider;
    }

    [AllowAnonymous]
    [HttpGet("/Account/LoginInternal")]
    public async Task<IActionResult> LoginInternalAsync(String data, String? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        var dataProtector = _dataProtectionProvider.CreateProtectorForLogin();

        var loginData = dataProtector.Unprotect(data);

        var parts = loginData.Split('|');

        var email = parts[0];

        var password = parts[1];

        var token = parts[2];

        var canRememberMe = Boolean.Parse(parts[3]);

        var user = await _userManager.FindByEmailAsync(email);

        var isTokenValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "Login", token);

        if (!isTokenValid)
        {
            return LocalRedirect($"~/login?email={email}&hasErrors=true");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var result = await _signInManager.PasswordSignInAsync(user.UserName, password, canRememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in");
            return LocalRedirect(returnUrl);
        }

        if (result.IsNotAllowed)
        {
            _logger.LogInformation("User attempted to log in, but was denied access {User}", user.UserName);
            return LocalRedirect("~/access-denied");
        }

        if (result.RequiresTwoFactor)
        {
            return LocalRedirect("~/login-2fa");
        }

        if (result.IsLockedOut)
        {
            _logger.LogInformation("User attempted to log in, but was locked out {User}", user.UserName);
            return LocalRedirect("~/lockout");
        }

        return LocalRedirect($"~/login?email={email}&hasErrors=true");
    }

    [Authorize]
    [HttpPost("/Account/LogoutInternal")]
    public async Task<IActionResult> LogoutAsync()
    {
        if(_signInManager.IsSignedIn(User))
        {
            await _signInManager.SignOutAsync();
        }

        return LocalRedirect("~/");
    }

    [AllowAnonymous]
    [HttpGet("/Account/LoginExternalProvider")]
    public async Task<IActionResult> LoginByExternalProviderAsync(String data, String? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        
        var dataProtector = _dataProtectionProvider.CreateProtectorForLogin();

        var loginData = dataProtector.Unprotect(data);

        var parts = loginData.Split('|');

        var userId = parts[0];

        var token = parts[1];

        var authenticationScheme = parts[2];

        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
        {
            return BadRequest($"User with id '{userId}' not found");
        }
        
        var tokenIsValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "Login", token);

        if (!tokenIsValid)
        {
            return LocalRedirect($"~/login?email={user.Email}&hasErrors=true");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync(token);

        if(externalLoginInfo is null)
        {
            return LocalRedirect($"~/login?email={user.Email}&hasErrors=true");
        }


        var loggedInResult =
            await _signInManager.ExternalLoginSignInAsync(authenticationScheme, externalLoginInfo.ProviderKey,
                true);

        if (loggedInResult.Succeeded)
        {
            _logger.LogInformation("User logged in");
            return LocalRedirect(returnUrl);
        }

        if (loggedInResult.IsNotAllowed)
        {
            _logger.LogInformation("User attempted to log in, but was denied access {User}", user.UserName);
            return LocalRedirect("~/access-denied");
        }

        if (loggedInResult.RequiresTwoFactor)
        {
            return LocalRedirect("~/login-2fa");
        }

        if (loggedInResult.IsLockedOut)
        {
            _logger.LogInformation("User attempted to log in, but was locked out {User}", user.UserName);
            return LocalRedirect("~/lockout");
        }

        return LocalRedirect($"~/login?email={user.Email}&hasErrors=true");
    }

    [AllowAnonymous]
    [HttpGet("/Account/ConfirmEmailInternal")]
    public async Task<IActionResult> ConfirmEmailAsync(String userId, String token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return BadRequest($"User with id '{userId}' not found");
        }

        var decodedToken = WebEncoders.Base64UrlDecode(token);

        var utfString = Encoding.UTF8.GetString(decodedToken);

        var confirmResult = await _userManager.ConfirmEmailAsync(user, utfString);

        return confirmResult.Succeeded 
            ? LocalRedirect("~/confirmed-email") 
            : BadRequest("Failed to confirm user email");
    }
}
