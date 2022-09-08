namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;
internal sealed class BlackCardConfiguration : IEntityTypeConfiguration<BlackCard>
{
    private const string BlackCardTableName = "BlackCards";

    private const string BlackCardPrimaryKeyName = $"PK_{BlackCardTableName}_Id";

    public void Configure(EntityTypeBuilder<BlackCard> entity)
    {
        entity.ToTable(BlackCardTableName);

        entity.HasKey(e => e.Id)
            .HasName(BlackCardPrimaryKeyName)
            .IsClustered();

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())");

        entity.Property(e => e.Message)
            .IsRequired();

        entity.Property(e => e.PickAnswersCount)
            .HasDefaultValue(1)
            .IsRequired();

        entity.Property(e => e.PackId)
            .IsRequired();
    }
}

