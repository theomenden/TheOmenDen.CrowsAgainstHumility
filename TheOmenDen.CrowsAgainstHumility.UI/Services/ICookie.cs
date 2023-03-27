namespace TheOmenDen.CrowsAgainstHumility.Services;

public interface ICookie
{
    ValueTask SetNameAsync(string value);
    ValueTask SetRoleAsync(string value);
    ValueTask SetRoomAsync(string value);
    ValueTask SetValueAsync(string key, string value, int? days = null);
    ValueTask<string> GetValueAsync(string key, string defaultValue = "");
}
