namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed record CaptchaResponseDto(Boolean Success, Double Score, String Action, DateTime ChallengeTs, String HostName);