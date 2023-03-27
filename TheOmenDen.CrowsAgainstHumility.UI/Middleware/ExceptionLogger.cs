using System.Text.Json;
using System.Net.Mime;
using System.Net;
using TheOmenDen.Shared.Enumerations;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Logging.Templates;
using TheOmenDen.Shared.Responses.Templates;
using TheOmenDen.Shared.Responses;
using TheOmenDen.CrowsAgainstHumility.Bootstrapping;

namespace TheOmenDen.CrowsAgainstHumility.Middleware;
public sealed class ExceptionLogger
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionLogger> _logger;

    private readonly ApiExceptionOptions _options;

    public ExceptionLogger(RequestDelegate next, ILogger<ExceptionLogger> logger, ApiExceptionOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task Invoke(HttpContext context, NavigationManager navigationManager)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _options);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, ApiExceptionOptions options)
    {
        var correlationId = Guid.NewGuid().ToString();

        var outcome = OperationOutcome.UnsuccessfulOutcome;
        outcome.CorrelationId = correlationId;
        outcome.ClientErrorPayload.Message = String.Format(Errors.UnhandledError, correlationId);

        options.AddResponseDetails?.Invoke(context, exception, outcome);

        var resolvedExceptionMessage = exception.GetInnermostExceptionMessage();

        var level = _options.DetermineLogLevel?.Invoke(exception) ?? LogLevel.Error;

        _logger.Log(
            level,
            EventIDs.EventIdUncaughtGlobal,
            exception,
            MessageTemplates.UncaughtGlobal,
            resolvedExceptionMessage,
            correlationId
            );

        var apiResponse = new ApiResponse<IEnumerable<String>>
        {           
            StatusCode = ResponseCodes.ApiError,
            Data = Enumerable.Empty<string>(),
            Outcome = outcome
        };

        var result = JsonSerializer.Serialize(apiResponse, Common.JsonSerializerOptions);
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(result);
    }
}
