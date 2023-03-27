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
        
        entity.HasOne(e => e.FromUser)
            .WithMany(p => p.ChatMessagesFromUsers)
            .HasForeignKey(d => d.FromUserId)
            .HasConstraintName("FK_Chat_User_FromUserId")
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasOne(e => e.ToUser)
            .WithMany(p => p.ChatMessagesToUsers)
            .HasForeignKey(d => d.ToUserId)
            .HasConstraintName("FK_Chat_User_ToUserId")
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
