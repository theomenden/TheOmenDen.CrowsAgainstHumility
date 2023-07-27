namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;

public sealed record ServerCreationResult(bool WasCreated, Guid? ServerId, String? ValidationMessage);
