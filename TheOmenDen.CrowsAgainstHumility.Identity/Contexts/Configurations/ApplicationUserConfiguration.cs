using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.ToTable("Users", "Security");
        
        entity.HasKey(t => t.Id)
            .IsClustered()
            .HasName("PK_Users_Id");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialId())");
    }
}
