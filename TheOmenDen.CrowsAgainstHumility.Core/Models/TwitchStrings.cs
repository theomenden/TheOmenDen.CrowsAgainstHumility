using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record TwitchStrings(String Key, String ClientId): ITwitchStrings;