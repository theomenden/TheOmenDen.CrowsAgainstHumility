using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Controllers;
public sealed class AccountController : Controller
{
    #region Private readonly members
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    #endregion
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _dataProtectionProvider = dataProtectionProvider;
    }

    [AllowAnonymous]
    [HttpGet("/Account/LoginInternal")]
    public async Task<IActionResult> LoginInternalAsync(String data, String? returnUrl = null, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return LocalRedirect("~/");
        }

        returnUrl ??= Url.Content("~/");

        var dataProtector = _dataProtectionProvider.CreateProtectorForLogin();

        var loginData = dataProtector.Unprotect(data);

        var parts = loginData.Split('|');

        var email= parts[0];
        var password= parts[1];
        var token = parts[2];
        var rememberMe = Boolean.Parse(parts[3]);

        var user = await _userManager.FindByEmailAsync(email);

        var isTokenValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "Login", token);

        if (!isTokenValid)
        {
            return LocalRedirect($"~/login?email={email}&hasErrors=true");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var result = await _signInManager.PasswordSignInAsync(user.UserName, password, rememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("{User} logged in.", user.Email);
            return LocalRedirect(returnUrl);
        }

        if (result.IsNotAllowed)
        {
            return LocalRedirect("~/access-denied");
        }

        if (result.RequiresTwoFactor)
        {
            return LocalRedirect("~/login-2fa");
        }

        if (result.IsLockedOut)
        {
            return LocalRedirect("~/lockout");
        }

        return LocalRedirect($"~/login?email={email}&hasErrors=true");
    }

    [AllowAnonymous]
    [HttpGet("/Account/LoginExternal")]
    public Task<IActionResult> LoginExternalAsync(String data, String? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("/Account/AssociateExternal")]
    public async Task<IActionResult> AssociateExternalProvideAsync(String userId, String provider, String providerName, String? returnUrl )
    {
        returnUrl ??= Url.Content("~/");

        var user = await _userManager.FindByIdAsync(userId);

        var signInAuthProperties = _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl, userId);

        var signInResult = await _signInManager.ExternalLoginSignInAsync(provider, "", signInAuthProperties.IsPersistent);
        
        if (signInResult.Succeeded && user is not null)
        {
            var loginInfo = new UserLoginInfo(provider, "", providerName);

            await _userManager.AddLoginAsync(user, loginInfo);

            return LocalRedirect(returnUrl);
        }

        if (signInResult.IsNotAllowed)
        {
            return LocalRedirect("~/access-denied");
        }

        if (signInResult.RequiresTwoFactor)
        {
            return LocalRedirect("~/login-2fa");
        }

        if (signInResult.IsLockedOut)
        {
            return LocalRedirect("~/lockout");
        }

        return LocalRedirect($"~/login?email={user.Email}&hasErrors=true");
    }

    [Authorize]
    [HttpPost("/Account/LogoutInternal")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested && _signInManager.IsSignedIn(User))
        {
            await _signInManager.SignOutAsync();
        }

        return LocalRedirect("~/");
    }

    [AllowAnonymous]
    [HttpGet("/Account/ConfirmEmailInternal")]
    public async Task<IActionResult> ConfirmEmailAsync(String userId, String token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return BadRequest($"User with id {userId} not found.");
        }

        var confirmResult = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)));

        return confirmResult.Succeeded
            ? LocalRedirect("~/confirmed-email")
            : BadRequest("Failed to confirm user email");
    }
}
