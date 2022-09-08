namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

internal sealed class WhiteCardConfiguration : IEntityTypeConfiguration<WhiteCard>
{
    private const String WhiteCardTableName = "WhiteCards";
    private const String WhiteCardPrimaryKeyName = $"PK_{WhiteCardTableName}_Id";

    public void Configure(EntityTypeBuilder<WhiteCard> entity)
    {
        entity.ToTable(WhiteCardTableName);

        entity.HasKey(x => x.Id)
            .IsClustered()
            .HasName(WhiteCardPrimaryKeyName);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())")
            .IsRequired();

        entity.Property(e => e.Message)
            .IsRequired();

        entity.Property(e => e.PackId)
            .IsRequired();

        entity.HasIndex(e => e.PackId)
            .HasDatabaseName($"IX_{WhiteCardTableName}_PackId");
    }
}