using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.Shared.Extensions;
namespace TheOmenDen.CrowsAgainstHumility.Identity.Services;

public sealed class ExternalAuthenticationService : IExternalAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserRegistrationService _registration;
    private readonly ILogger<ExternalAuthenticationService> _logger;

    public ExternalAuthenticationService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserRegistrationService registration, ILogger<ExternalAuthenticationService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _registration = registration;
        _logger = logger;
    }
    
    public async Task<string> ExternalSignInAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        try
        {
            var authenticateResult = await httpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (authenticateResult?.Succeeded is not true)
            {
                return $"~/auth-error/{ExternalAuthenticationResult.ExternalAuthenticationError}";
            }

            var externalUser = authenticateResult.Principal;

            if (externalUser is null)
            {
                return $"~/auth-error/{ExternalAuthenticationResult.ExternalAuthenticationError}";
            }

            var claims = externalUser.Claims.ToList();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@Claims}", externalClaims);
            }

            // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used                
            var userIdClaim = externalUser.FindFirst("sub") ??
                externalUser.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
            {
                return $"~/auth-error/{ExternalAuthenticationResult.ExternalUnknownUserId}";
            }

            var externalUserId = userIdClaim.Value;
            var externalProvider = userIdClaim.Issuer;

            //Quick check to sign in
            var externalSignInResult = await _signInManager.ExternalLoginSignInAsync(externalProvider, externalUserId, true);

            if (externalSignInResult.Succeeded)
            {
                // delete temporary cookie used during external authentication
                await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return "~/";
            }

            //If external login/signin failed
            var userEmailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var userNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            //get the user by Email (we are forcing it to be unique)
            var user = await _userManager.FindByEmailAsync(userEmailClaim.Value);

            if (user is not null)
            {
                //check if the login for this provider exists
                var userLogins = await _userManager.GetLoginsAsync(user);

                if (userLogins.Any(ul => ul.LoginProvider == externalProvider && ul.ProviderKey == externalUserId))
                {
                    //something went wrong, it should get logged in

                    // If lock out activated and the max. amounts of attempts is reached.
                    if (externalSignInResult.IsLockedOut)
                    {
                        _logger.LogInformation("User Locked out: {UserName}", user.UserName);
                        return $"~/auth-error/{ExternalAuthenticationResult.UserIsLockedOut}";
                    }

                    // If your email is not confirmed but you require it in the settings for login.
                    if (externalSignInResult.IsNotAllowed)
                    {
                        _logger.LogInformation("User not allowed to log in: {UserName}", user.UserName);
                        return $"~/auth-error/{ExternalAuthenticationResult.UserIsNotAllowed}";
                    }

                    return $"~/auth-error/{ExternalAuthenticationResult.Unknown}";
                }

            }

            var givenNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            var surNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);

            var userName = userEmailClaim.Value;
            var firstName = givenNameClaim?.Value;
            var lastName = surNameClaim?.Value;

            if (string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(userNameClaim?.Value))
            {
                firstName = userNameClaim.Value.Split(' ')?.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(userNameClaim?.Value))
            {
                lastName = userNameClaim.Value.Split(' ')?.LastOrDefault();
            }

            var newUser = new ApplicationUser
            {
                CreatedDate = DateTime.UtcNow,
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = userEmailClaim.Value,
                EmailConfirmed = true,
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser);

                if (result.Succeeded)
                {
                    await _registration.SeedWithDefaultRolesAsync(cancellationToken);

                    // get the newly create user
                    user = await _userManager.FindByEmailAsync(userEmailClaim.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("External login Failed: {@Ex}", ex.GetInnermostExceptionMessage());
                return $"~/auth-error/{ExternalAuthenticationResult.UserCreationFailed}";
            }

            //All if fine, this user (email) did not try to log in before using this external provider
            //Add external login info
            var addExternalLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(externalProvider, externalUserId, userNameClaim.Value));
            if (addExternalLoginResult.Succeeded is false)
            {
                return $"~/auth-error/{ExternalAuthenticationResult.CannotAddExternalLogin}";
            }

            //Try to sign in again
            externalSignInResult = await _signInManager.ExternalLoginSignInAsync(externalProvider, externalUserId, true);

            if (externalSignInResult.Succeeded)
            {
                //// delete temporary cookie used during external authentication
                await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return "~/";
            }

            //Something went terrible wrong, user exists, external login info added
            // If lock out activated and the max. amounts of attempts is reached.
            if (externalSignInResult.IsLockedOut)
            {
                _logger.LogInformation("User Locked out: {UserName}", user.UserName);
                return $"~/auth-error/{ExternalAuthenticationResult.UserIsLockedOut}";
            }

            // If your email is not confirmed but you require it in the settings for login.
            if (externalSignInResult.IsNotAllowed)
            {
                _logger.LogInformation("User not allowed to log in: {UserName}", user.UserName);
                return $"~/auth-error/{ExternalAuthenticationResult.UserIsNotAllowed}";
            }

            return $"~/auth-error/{ExternalAuthenticationResult.Unknown}";
        }
        catch (Exception ex)
        {
            _logger.LogError("External login Failed: {@Ex}", ex);
            return $"~/auth-error/{ExternalAuthenticationResult.Unknown}";
        }
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        foreach (var c in normalizedString
                     .Select(c => new { c, unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c) })
                     .Where(@t => @t.unicodeCategory != UnicodeCategory.NonSpacingMark)
                     .Select(@t => @t.c))
        {
            stringBuilder.Append(c);
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
}
