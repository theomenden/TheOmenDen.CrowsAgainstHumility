﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Data.Models;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations
{
    public partial class WhiteCardConfiguration : IEntityTypeConfiguration<WhiteCard>
    {
        public void Configure(EntityTypeBuilder<WhiteCard> entity)
        {
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
}
