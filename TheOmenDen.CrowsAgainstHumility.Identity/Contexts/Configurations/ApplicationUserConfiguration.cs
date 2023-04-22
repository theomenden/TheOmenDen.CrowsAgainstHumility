using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Identity.Converters;

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

        entity.Property(e => e.NotificationType)
            .HasConversion<EnumerationBaseConverter<NotificationType,Int32>>();
    }
}
