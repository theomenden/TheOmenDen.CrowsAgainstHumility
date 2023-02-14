using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Controllers;
public sealed class AccountController : Controller
{
    #region Private readonly members
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly IExternalAuthenticationService _externalAuthenticationService;
    #endregion
    #region Constructor
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, IDataProtectionProvider dataProtectionProvider, IExternalAuthenticationService externalAuthenticationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _dataProtectionProvider = dataProtectionProvider;
        _externalAuthenticationService = externalAuthenticationService; 
    }
    #endregion

    [AllowAnonymous]
    [HttpGet("/Account/LoginInternal")]
    public async Task<IActionResult> LoginInternalAsync(String data, String? returnUrl = null, CancellationToken cancellationToken = default)
    {
        returnUrl ??= Url.Content("~/");

        var dataProtector = _dataProtectionProvider.CreateProtectorForLogin();
        var loginData = dataProtector.Unprotect(data);
        var parts = loginData.Split('|');

        var email = parts[0];
        var password = parts[1];
        var token = parts[2];
        var isRemembered = Boolean.Parse(parts[3]);

        var user = await _userManager.FindByEmailAsync(email);

        var isTokenValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "Login", token);

        if (!isTokenValid)
        {
            return LocalRedirect($"~login?email={email}&hasErrors=true");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var signInResult =
            await _signInManager.PasswordSignInAsync(user.UserName, password, isRemembered, lockoutOnFailure: false);

        if (signInResult.Succeeded)
        {
            _logger.LogInformation("User logged in");
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

        return LocalRedirect($"~/login?email={email}&hasErrors=true");
    }
    
    [AllowAnonymous]
    [HttpGet("/Account/challenge/{provider}")]
    [HttpGet("/Account/challenge/{provider}/{returnUrl?}")]
    public async Task<IActionResult> ExternalChallengeAsync(String provider, String returnUrl)
    {
        if (String.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = "~/";
        }

        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalSignIn)),
            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", provider }
            }
        };

        return await Task.FromResult(Challenge(authenticationProperties, provider));
    }

    [AllowAnonymous]
    [HttpGet("ExternalSignIn", Name = "ExternalSignIn")]
    public async Task<IActionResult> ExternalSignIn(CancellationToken cancellationToken = default)
        => Redirect(await _externalAuthenticationService.ExternalSignInAsync(HttpContext, cancellationToken));

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
            return NotFound($"User with id {userId} not found.");
        }

        var confirmResult = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)));

        return confirmResult.Succeeded
            ? LocalRedirect("~/confirmed-email")
            : BadRequest("Failed to confirm user email");
    }
}
