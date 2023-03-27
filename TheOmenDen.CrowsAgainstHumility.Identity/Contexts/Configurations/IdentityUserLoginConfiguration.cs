using Microsoft.AspNetCore.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal class IdentityUserLoginConfiguration: IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> entity)
    {
        entity.ToTable("UserLogins", "Security");
    }
}
