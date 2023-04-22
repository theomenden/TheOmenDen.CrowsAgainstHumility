using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IEmailManagerService
{
    Task BuildUserCreatedConfirmationMessageAsync(string emailAddress, string temporaryPassword,
        ApplicationUser applicationUser, string baseUri, CancellationToken cancellationToken = default);

    Task BuildRegistrationConfirmationEmailAsync(string emailAddress, ApplicationUser user, string baseUri, CancellationToken cancellationToken = default);

    Task BuildForgotPasswordEmailConfirmationAsync(string emailAddress, ApplicationUser applicationUser,
        string baseUri, CancellationToken cancellationToken = default);
}