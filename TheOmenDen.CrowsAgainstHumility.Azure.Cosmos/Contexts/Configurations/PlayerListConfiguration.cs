using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Cosmos.Contexts.Configurations;
internal sealed class PlayerListConfiguration: IEntityTypeConfiguration<PlayerList>
{
    public void Configure(EntityTypeBuilder<PlayerList> entity)
    {
        entity.ToContainer("PlayerLists");
        entity.HasNoDiscriminator()
            .HasPartitionKey(e => e.Name);
        
        entity.Property(e => e.Name)
            .IsRequired()
            .IsETagConcurrency();
    }
}
