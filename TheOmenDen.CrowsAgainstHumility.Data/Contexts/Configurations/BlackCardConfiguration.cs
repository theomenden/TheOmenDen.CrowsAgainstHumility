using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

internal sealed class BlackCardConfiguration : IEntityTypeConfiguration<BlackCard>
{
    public void Configure(EntityTypeBuilder<BlackCard> entity)
    {
        entity.ToTable("BlackCards", "CAH");
        entity.HasKey(e => e.Id)
            .IsClustered();

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Text)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.NumberOfBlanks)
            .IsRequired()
            .HasDefaultValue(1);

        entity.HasOne(e => e.Expansion)
            .WithMany(e => e.BlackCards)
            .HasForeignKey(e => e.ExpansionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.ExpansionId);
    }
}