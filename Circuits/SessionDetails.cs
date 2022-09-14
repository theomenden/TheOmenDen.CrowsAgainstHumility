using System.Security.Principal;
using TheOmenDen.CrowsAgainstHumility.Events;

namespace TheOmenDen.CrowsAgainstHumility.Circuits;
public sealed class SessionDetails : ISessionDetails
{
    private readonly Dictionary<String, SessionModel> _sessions = new(30);

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
        if (_sessions.ContainsKey(session.CircuitId))
        {
            var updatedUser = _sessions[session.CircuitId].User with
            {
                UserId = userId
            };

            _sessions[session.CircuitId].User = updatedUser;

            return;
        }
        session.CreatedAt = DateTime.UtcNow;

        _sessions.Add(session.CircuitId, session);

        _logger.LogInformation("Successfully added Session with Id {SessionId}", session.Id);
    }

    public void ConnectSession(String circuitId, IIdentity user)
    {
        if (_sessions.ContainsKey(circuitId))
        {
            var updatedUser = _sessions[circuitId].User with
            {
                UserId = user.Name ?? String.Empty
            };

            _sessions[circuitId].User = updatedUser;

            _logger.LogInformation("Successfully updated Session Information for circuit with Id: {Circuit}", circuitId);
            return;
        }


        var session = new SessionModel
        {
            User = new SessionUser(user.Name, user.IsAuthenticated, user.AuthenticationType),
            CreatedAt = DateTime.UtcNow,
            CircuitId = circuitId
        };

        _sessions.Add(circuitId, session);

        _logger.LogInformation("Successfully added Session with Id {SessionId}", session.Id);
    }

    public void DisconnectSession(String? circuitId)
    {
        if (String.IsNullOrWhiteSpace(circuitId))
        {
            _logger.LogError("{CircuitId} was not specified", nameof(circuitId));
            return;
        }

        if (_sessions.TryGetValue(circuitId, out var userSession)
            && _sessions.Remove(circuitId))
        {
            OnUserRemoved(userSession.User.UserId);
            OnCircuitsChanged();

            _logger.LogInformation("Session removed successfully for {User}", userSession.User.UserId);
            return;
        }

        _logger.LogError("Session with circuitId {CircuitId} could not be found", circuitId);
    }
}