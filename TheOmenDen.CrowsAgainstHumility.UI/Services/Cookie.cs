using Microsoft.JSInterop;

namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class Cookie : ICookie
{
    #region Constants
    private const string NameKey = "Name";
    private const string RoleKey = "RoleId";
    private const string RoomKey = "RoomCode";
    #endregion
    #region Private Members
    private readonly IJSRuntime _jsRuntime;
    #endregion
    #region Constructors
    public Cookie(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }
    #endregion
    #region Helper Methods
    public ValueTask SetNameAsync(string value) => SetValueAsync(NameKey, value);
    public ValueTask SetRoleAsync(string value) => SetValueAsync(RoleKey, value);
    public ValueTask SetRoomAsync(string value) => SetValueAsync(RoomKey, value);
    #endregion
    #region Public Methods
    public async ValueTask SetValueAsync(string key, string value, int? days = null)
    => await _jsRuntime.InvokeVoidAsync("WriteCookie", key, value, days);

    public async ValueTask<string> GetValueAsync(string key, string defaultValue = "")
    {
        var cookieValue = await _jsRuntime.InvokeAsync<string>("ReadCookie", key);

        var cookieValues = cookieValue.Split(';');

        foreach (var val in cookieValues)
        {
            var index = val.IndexOf('=', StringComparison.Ordinal);

            if (index > 0
                && val[..index].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                return val[(index + 1)..];
            }
        }
        return defaultValue;
    }
    #endregion
}
