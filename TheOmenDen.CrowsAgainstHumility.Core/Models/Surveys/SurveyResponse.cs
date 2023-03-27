namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Surveys;

public sealed record SurveyResponse(Guid Id, Int32 QuestionId, String Response, Guid UserId);
