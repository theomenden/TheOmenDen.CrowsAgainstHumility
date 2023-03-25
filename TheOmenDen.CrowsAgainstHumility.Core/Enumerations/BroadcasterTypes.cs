using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record BroadcasterTypes: EnumerationBase<BroadcasterTypes>
{
    private BroadcasterTypes(string name, int id): base(name, id) {}

    public static readonly BroadcasterTypes Default = new(nameof(Default), 1);
    public static readonly BroadcasterTypes Affiliate = new(nameof(Affiliate), 2);
    public static readonly BroadcasterTypes Partner = new(nameof(Partner), 3);
}
