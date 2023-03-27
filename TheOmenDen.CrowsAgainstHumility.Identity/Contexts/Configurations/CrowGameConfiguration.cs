using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Identity.Converters;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal sealed class CrowGameConfiguration: IEntityTypeConfiguration<CrowGame>
{
    public void Configure(EntityTypeBuilder<CrowGame> entity)
    {
        entity.ToTable("Games");

        entity.HasKey(x => x.Id)
            .IsClustered()
            .HasName("PK_Games_Id");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())");

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Packs)
            .HasConversion<GuidCollectionConverterBySemicolon>();
    }
}
