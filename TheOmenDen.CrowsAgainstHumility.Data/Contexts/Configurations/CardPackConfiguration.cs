namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;
internal sealed class CardPackConfiguration: IEntityTypeConfiguration<PackData>
{
    private const string PacksTableName = "Packs";

    private const string PacksPrimaryKeyName = $"PK_{PacksTableName}_Id";

    public void Configure(EntityTypeBuilder<PackData> entity)
    {
        entity.ToTable(PacksTableName);

        entity.HasKey(e => e.Id)
            .HasName(PacksPrimaryKeyName)
            .IsClustered();


        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.IsOfficialPack)
            .HasDefaultValue(false)
            .IsRequired();

        entity.Property(e => e.Name)
            .IsRequired();

        entity.HasIndex(e => e.Name)
            .IncludeProperties(e => e.IsOfficialPack)
            .HasDatabaseName("IX_Packs_Name");
    }
}

