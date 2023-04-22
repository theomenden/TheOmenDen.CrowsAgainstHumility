using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal class ApplicationRoleConfiguration: IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> entity)
    {
        entity.ToTable("Roles", "Security");

        entity.HasKey(e => e.Id)
            .IsClustered()
            .HasName("PK_Roles_Id");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())");
    }
}
