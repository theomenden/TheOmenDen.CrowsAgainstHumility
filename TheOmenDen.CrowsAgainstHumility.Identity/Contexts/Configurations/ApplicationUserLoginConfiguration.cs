using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal class ApplicationUserLoginConfiguration: IEntityTypeConfiguration<ApplicationUserLogin>
{
    public void Configure(EntityTypeBuilder<ApplicationUserLogin> entity)
    {
        entity.ToTable(name: "UserLogins", schema: "Security");

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserLogins_UserId");
    }
}
