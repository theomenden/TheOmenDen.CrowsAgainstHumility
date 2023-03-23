namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

public partial class WhiteCardConfiguration : IEntityTypeConfiguration<WhiteCard>
{
    public void Configure(EntityTypeBuilder<WhiteCard> entity)
    {
        entity.ToTable("WhiteCards");
        entity.HasKey(e => e.Id)
            .IsClustered()
            .HasName("PK_WhiteCards_Id");

        entity.HasIndex(e => e.PackId, "IX_WhiteCards_PackId");

        entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

        entity.Property(e => e.CardText)
            .IsRequired()
            .HasMaxLength(1000);

        entity.HasOne(d => d.Pack)
            .WithMany(p => p.WhiteCards)
            .HasForeignKey(d => d.PackId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WhiteCards_Packs");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<WhiteCard> entity);
}