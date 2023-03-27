
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal class RoomStateConfiguration : IEntityTypeConfiguration<RoomState>
{
    public void Configure(EntityTypeBuilder<RoomState> entity)
    {
        entity.ToTable("Rooms");
        
        entity.HasKey(e => e.Id)
            .IsClustered()
            .HasName("PK_Rooms_Id");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())");

        entity.Property(e => e.Name)
            .HasMaxLength(90);

        entity.Property(e => e.RoomCode)
            .HasMaxLength(8);

        entity.Property(e => e.Password)
            .HasMaxLength(8);

        entity.HasIndex(e => e.GameId)
            .HasDatabaseName("IX_Rooms_GameId");

        entity.HasMany(e => e.Games)
            .WithOne(g => g.Room)
            .HasForeignKey(e => e.RoomId)
            .HasConstraintName("FK_Games_Rooms_GameId");
    }
}
