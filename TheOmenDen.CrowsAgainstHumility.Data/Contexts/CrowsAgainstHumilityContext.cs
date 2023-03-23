﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts;

public partial class CrowsAgainstHumilityContext : DbContext
{
    public CrowsAgainstHumilityContext(DbContextOptions<CrowsAgainstHumilityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlackCard> BlackCards { get; set; }
    public virtual DbSet<Pack> Packs { get; set; }
    public virtual DbSet<WhiteCard> WhiteCards { get; set; }
    public virtual DbSet<FilteredWhiteCardsByPack> vw_FilteredWhiteCardsByPack { get; set; }
    public virtual DbSet<FilteredBlackCardsByPack> vw_FilteredBlackCardsByPack { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var containingAssembly = typeof(PackConfiguration).Assembly;

        modelBuilder.ApplyConfigurationsFromAssembly(containingAssembly);

        OnModelCreatingGeneratedFunctions(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}