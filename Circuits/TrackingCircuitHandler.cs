using Microsoft.AspNetCore.Components.Server.Circuits;

namespace TheOmenDen.CrowsAgainstHumility.Circuits;
public class TrackingCircuitHandler : CircuitHandler
{
    public TrackingCircuitHandler(SessionDetails sessionData)
    {
        SessionData = sessionData;
    }


    public event EventHandler CircuitsChanged;

    public String CircuitId { get; set; } = String.Empty;

    public SessionDetails SessionData { get; set; }

    protected virtual void OnCircuitsChanged() => CircuitsChanged?.Invoke(this, EventArgs.Empty);

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        SessionData.RemoveSession(circuit.Id);

        OnCircuitsChanged();

        return base.OnCircuitClosedAsync(circuit, cancellationToken);   
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        CircuitId = circuit.Id;

        OnCircuitsChanged();

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }
}