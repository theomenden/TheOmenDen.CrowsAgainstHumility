﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>

using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

public partial class BlackCardConfiguration : IEntityTypeConfiguration<BlackCard>
{
    public void Configure(EntityTypeBuilder<BlackCard> entity)
    {
        entity.ToTable("BlackCards");
        entity.HasKey(e => e.Id)
            .IsClustered()
            .HasName("PK_BlackCards_Id");
        
        entity.HasIndex(e => e.PackId, "IX_BlackCards_PackId");

        entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

        entity.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(1000);

        entity.Property(e => e.PickAnswersCount).HasDefaultValueSql("((1))");

        entity.HasOne(d => d.Pack)
            .WithMany(p => p.BlackCards)
            .HasForeignKey(d => d.PackId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BlackCards_Packs");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<BlackCard> entity);
}