namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Surveys;

public sealed record Survey(Guid Id, Int32 rating, IEnumerable<string> QuestionNames);
