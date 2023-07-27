using TheOmenDen.CrowsAgainstHumility.Circuits;

namespace TheOmenDen.CrowsAgainstHumility.Middleware;

public class CorvidSessionMiddleware
{
    private readonly RequestDelegate _next;

    public CorvidSessionMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);
        _next = next;
    }

    public Task InvokeAsync(HttpContext context, ISessionDetails sessionDetails)
    {
        sessionDetails.ConnectSession(context.Connection.Id, context.User);
        try
        {
            return _next(context);
        }
        catch (Exception ex)
        {
            // TODO: Handle the System.Exception
        }
    }
}
