using Microsoft.AspNetCore.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> entity)
    {
        entity.ToTable("UserTokens", "Security");
    }
}
