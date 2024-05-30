namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

public sealed class BlackCard(string text, int numberOfBlanks) : BaseCard(text)
{
    public int NumberOfBlanks { get; private set; } = numberOfBlanks;
}