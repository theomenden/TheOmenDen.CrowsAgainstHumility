namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts.Configurations;
internal sealed class CawChatMessageConfiguration: IEntityTypeConfiguration<CawChatMessage>
{
    public void Configure(EntityTypeBuilder<CawChatMessage> entity)
    {
        entity.ToTable("ChatMessages");

        entity.HasKey(e => e.Id)
            .IsClustered(false);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())");

        entity.Property(e => e.Message)
            .HasMaxLength(280);

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("getdate()");

        entity.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_CawChat_CreatedAt")
            .IncludeProperties(e => new { e.ToUserId, e.FromUserId, e.Message });

        entity.HasIndex(e => e.ToUserId)
            .HasDatabaseName("IX_CawChat_ToUserId")
            .IncludeProperties(e => e.Message);

        entity.HasIndex(e => e.FromUserId)
            .HasDatabaseName("IX_CawChat_FromUserId")
            .IncludeProperties(e => e.Message);
    }
}
