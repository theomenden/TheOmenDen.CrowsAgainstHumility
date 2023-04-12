﻿namespace TheOmenDen.CrowsAgainstHumility.Core.Exceptions;
public sealed class CannotShowCardsException : Exception
{
    public CannotShowCardsException() { }
    public CannotShowCardsException(string message) : base(message) { }
    public CannotShowCardsException(string message, Exception innerException) : base(message, innerException) { }
}
