using TheOmenDen.Shared.Responses;

namespace TheOmenDen.CrowsAgainstHumility.Middleware;

public sealed class ApiExceptionOptions
{
    public Action<HttpContext, Exception, OperationOutcome> AddResponseDetails { get; set; }

    public Func<Exception, LogLevel> DetermineLogLevel {get; set;}
}