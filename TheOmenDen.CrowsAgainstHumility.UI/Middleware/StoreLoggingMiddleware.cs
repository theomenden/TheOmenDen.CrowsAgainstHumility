using Fluxor;
using System.Text.Json;

namespace TheOmenDen.CrowsAgainstHumility.Middleware;

public class StoreLoggingMiddleware : Fluxor.Middleware
{
    private IStore _store;
    private readonly ILogger<StoreLoggingMiddleware> _logger;

    public StoreLoggingMiddleware(ILogger<StoreLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public override Task InitializeAsync(IDispatcher dispatcher, IStore store)
    {
        _store = store;
        _logger.LogInformation("{Initialize}", nameof(InitializeAsync));
        return Task.CompletedTask;
    }

    public override void AfterInitializeAllMiddlewares()
    {
        _logger.LogInformation("{AfterInitialized}", nameof(AfterInitializeAllMiddlewares));
    }

    public bool MayDispatchAction<T>(T action)
    {
        _logger.LogInformation("{DispatchAction}: {ObjectInfo}", nameof(MayDispatchAction), ObjectInformation(action));
        return true;
    }

    public override bool MayDispatchAction(object action)
    {
        _logger.LogInformation("{DispatchAction}: {ObjectInfo}",nameof(MayDispatchAction), ObjectInformation(action));
        return true;
    }

    public void BeforeDispatch<T>(T action)
    {
        _logger.LogInformation("{DispatchAction}: {ObjectInfo}", nameof(BeforeDispatch), ObjectInformation(action));
    }

    public override void BeforeDispatch(object action)
    {
        _logger.LogInformation("{DispatchAction}: {ObjectInfo}", nameof(BeforeDispatch), ObjectInformation(action));
    }


    public void AfterDispatch<T>(T action)
    {
        _logger.LogInformation("{DispatchAction}: {ObjectInfo}", nameof(AfterDispatch), ObjectInformation(action));
    }

    public override void AfterDispatch(object action)
    {
        _logger.LogInformation("{DispatchAction}: {ObjectInfo}", nameof(AfterDispatch), ObjectInformation(action));
    }

    private static String ObjectInformation<T>(T obj)
    => $"{typeof(T).Name}:{JsonSerializer.Serialize(obj, typeof(T))}";
}