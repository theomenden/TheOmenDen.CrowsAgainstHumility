using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

internal sealed class ExpansionConfiguration : IEntityTypeConfiguration<Expansion>
{
    public void Configure(EntityTypeBuilder<Expansion> entity)
    {
        entity.ToTable("Expansions", "CAH");
        entity.HasKey(e => e.Id)
            .IsClustered();

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1500);
    }
}