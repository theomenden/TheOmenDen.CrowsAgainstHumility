namespace TheOmenDen.CrowsAgainstHumility.Circuits;
public sealed class SessionDetails : IDisposable, IAsyncDisposable
{
    private readonly List<SessionModel> _sessions = new(30);

    private readonly ILogger<SessionDetails> _logger;

    private bool disposedValue;

    public SessionDetails(ILogger<SessionDetails> logger)
    {
        _logger = logger;
    }
    
    public void AddSession(SessionModel session)
    {
        if(_sessions.Contains(session))
        {
            return;
        }

        session.CreatedAt = DateTime.UtcNow;

        _sessions.Add(session);

        _logger.LogInformation("Successfully added Session with Id {SessionId}", session.Id);
    }
        
    public void RemoveSession(String? circuitId)
    {
        if(String.IsNullOrWhiteSpace(circuitId))
        {
            _logger.LogError("{CircuitId} was not specified", nameof(circuitId));
            return;
        }

        if(_sessions.RemoveAll(session => session.CircuitId?.Equals(circuitId) == true) == 0)
        {
            _logger.LogError("Session with circuitId {CircuitId} could not be found", circuitId);
        }
    }



    public SessionModel GetSessionById(Guid sessionId)
    => _sessions.FirstOrDefault(session => session.Id == sessionId, SessionModel.DefaultSession);

    public SessionModel GetSessionByCircuitId(String circuitId)
    => _sessions.FirstOrDefault(session => String.Equals(session.CircuitId, circuitId, StringComparison.OrdinalIgnoreCase), SessionModel.DefaultSession);

    #region Destruction Methods
    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _sessions.Clear();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        _sessions.Clear();

        return ValueTask.CompletedTask;
    }
    #endregion
}
