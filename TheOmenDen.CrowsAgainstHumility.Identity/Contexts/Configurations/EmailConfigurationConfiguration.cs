namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal sealed class EmailConfigurationConfiguration: IEntityTypeConfiguration<EmailConfiguration>
{
    public void Configure(EntityTypeBuilder<EmailConfiguration> entity)
    {
        entity.ToTable("Configurations", "Crows");
    }
}
