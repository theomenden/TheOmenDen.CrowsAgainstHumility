using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

internal sealed class WhiteCardConfiguration : IEntityTypeConfiguration<WhiteCard>
{
    public void Configure(EntityTypeBuilder<WhiteCard> entity)
    {
        entity.ToTable("WhiteCards", "CAH");
        entity.HasKey(e => e.Id)
            .IsClustered();

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Text)
            .IsRequired()
            .HasMaxLength(255);

        entity.HasOne(e => e.Expansion)
            .WithMany(e => e.WhiteCards)
            .HasForeignKey(e => e.ExpansionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.ExpansionId);
    }
}