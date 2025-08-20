using System;
using System.Collections.Generic;
using Library.Database.Models.Public;
using Microsoft.EntityFrameworkCore;

namespace Library.Database.Contexts.Public;

public partial class PublicDbContext : DbContext
{
    public PublicDbContext(DbContextOptions<PublicDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<country> countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<country>(entity =>
        {
            entity.HasKey(e => e.id).HasName("countries_pkey");

            entity.Property(e => e.country_code2)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.country_code3)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.country_name).HasMaxLength(100);
            entity.Property(e => e.currency_code)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.timezone).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
