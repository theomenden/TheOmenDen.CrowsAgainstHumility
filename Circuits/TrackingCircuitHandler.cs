using Microsoft.AspNetCore.Components.Server.Circuits;

namespace TheOmenDen.CrowsAgainstHumility.Circuits;
public class TrackingCircuitHandler : CircuitHandler
{
    public TrackingCircuitHandler(ISessionDetails sessionData)
    {
        SessionData = sessionData;
    }

    public String CircuitId { get; private set; } = String.Empty;

    public ISessionDetails SessionData { get; }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        SessionData.DisconnectSession(circuit.Id);

        return base.OnCircuitClosedAsync(circuit, cancellationToken);   
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        CircuitId = circuit.Id;

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }
}