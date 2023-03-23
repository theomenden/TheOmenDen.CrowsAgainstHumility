namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;
internal partial class FilteredBlackCardsByPackConfiguration: IEntityTypeConfiguration<FilteredBlackCardsByPack>
{
    public void Configure(EntityTypeBuilder<FilteredBlackCardsByPack> entity)
    {
        entity.HasNoKey();

        entity.ToView("vw_FilteredBlackCardsByPack");

        entity.Property(e => e.Id)
            .IsRequired();

        entity.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(1000);

        entity.Property(e => e.PickAnswersCount)
            .IsRequired();

        entity.Property(e => e.PackId)
            .IsRequired();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<FilteredBlackCardsByPack> entity);
}
