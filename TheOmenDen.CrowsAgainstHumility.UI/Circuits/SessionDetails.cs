using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Principal;
using SendGrid.Helpers.Mail;
using TheOmenDen.CrowsAgainstHumility.Events;

namespace TheOmenDen.CrowsAgainstHumility.Circuits;
public sealed class SessionDetails : ISessionDetails
{
    private readonly ConcurrentDictionary<string, SessionModel> _sessions = new();

    private readonly ILogger<SessionDetails> _logger;

    public SessionDetails(ILogger<SessionDetails> logger)
    {
        _logger = logger;
    }

    public event EventHandler CircuitsChanged;

    public event UserDisconnectEventHandler UserDisconnect;

    private void OnCircuitsChanged() => CircuitsChanged?.Invoke(this, EventArgs.Empty);

    private void OnUserRemoved(String userId)
    {
        var args = new UserDisconnectEventArgs
        {
            UserId = userId
        };

        UserDisconnect?.Invoke(this, args);
    }

    public void ConnectSession(SessionModel session, String userId)
    {
        var model = _sessions.AddOrUpdate(
            session.CircuitId,
            _ =>
            {
                session.User = SessionUser.DefaultUser with
                {
                    UserId = userId
                };
                session.CreatedAt = DateTime.UtcNow;
                return session;
            },
            (_, existingSession) => UpdateCurrentSessionModel(session.CircuitId, userId, existingSession));

        if (!_sessions.ContainsKey(model.CircuitId))
        {
            _logger.LogCritical("Session with circuitId {CircuitId} could not be found", model.CircuitId);
            return;
        }

        _logger.LogInformation("Successfully added Session with Id {SessionId}", model.Id);
    }

    public void ConnectSession(string circuitId, ClaimsPrincipal user)
    {
        var model = _sessions.AddOrUpdate(
            circuitId,
            _ =>
            {
                var session = new SessionModel
                {
                    CircuitId = circuitId,
                    CreatedAt = DateTime.UtcNow,
                    User = SessionUser.DefaultUser with
                    {
                        UserId = user.Identity?.Name,
                        IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
                        AuthenticationType = user.Identity?.AuthenticationType
                    }
                };
                return session;
            },
            (_, session) => UpdateCurrentSessionModel(circuitId, user.Identity?.Name, session));

        if (!_sessions.ContainsKey(model.CircuitId))
        {
            _logger.LogCritical("Session with circuitId {CircuitId} could not be added for user {@User}", user);
            return;
        }

        _logger.LogInformation("Successfully added Session with Id {SessionId}", model.Id);
    }

    public void ConnectSession(String circuitId, IIdentity user)
    {
        var model = _sessions.AddOrUpdate(
            circuitId,
            _ =>
            {
                var session = new SessionModel
                {
                    User = new SessionUser(user.Name, user.IsAuthenticated, user.AuthenticationType),
                    CircuitId = circuitId,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Session with circuitId {CircuitId} was not found. Creating new session for user {@User}",
                    circuitId, session.User);
                return session;
            },
            (_, session) => UpdateCurrentSessionModel(circuitId, user, session));

        if (!_sessions.ContainsKey(model.CircuitId))
        {
            _logger.LogCritical("Session with circuitId {CircuitId} could not be added for user {@User}", user);
        }
    }

    public void DisconnectSession(String? circuitId)
    {
        if (String.IsNullOrWhiteSpace(circuitId))
        {
            _logger.LogError("{CircuitId} was not specified", nameof(circuitId));
            return;
        }

        var (wasRemoved, userSession) = CanRemoveUserSession(circuitId);


        if (wasRemoved && userSession is not null)
        {
            OnUserRemoved(userSession.User.UserId);
            OnCircuitsChanged();

            _logger.LogInformation("Session removed successfully for {User}", userSession.User.UserId);
            return;
        }

        _logger.LogError("Session with circuitId {CircuitId} could not be found", circuitId);
    }

    private (bool wasRemoved, SessionModel? userSession) CanRemoveUserSession(string circuitId) =>
    (_sessions.Remove(circuitId, out var sessionModel), sessionModel);

    private SessionModel UpdateCurrentSessionModel(string circuitId, IIdentity user, SessionModel session)
    {
        var updateduser = session.User with
        {
            UserId = user.Name ?? string.Empty
        };

        session.User = updateduser;

        _logger.LogInformation("Successfully updated session information for circuit with Id: {Circuit}", circuitId);
        return session;
    }

    private SessionModel UpdateCurrentSessionModel(string circuitId, string username, SessionModel session)
    {
        var updatedUser = session.User with
        {
            UserId = username
        };
        _logger.LogInformation("Successfully updated session informati9on for circuit with Id: {Circuit}", circuitId);
        return session;
    }
}