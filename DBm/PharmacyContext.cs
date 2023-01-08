using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DBm;

public partial class PharmacyContext : DbContext
{
    private string _databasePath;
    public PharmacyContext(string databasePath)
    {
        _databasePath = databasePath;
    }

    public PharmacyContext(DbContextOptions<PharmacyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employees> Employees { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<Suppliers> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Filename={_databasePath}");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employees>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InVacation).HasColumnName("inVacation");
            entity.Property(e => e.Job)
                .HasMaxLength(50)
                .HasColumnName("job");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Salary).HasColumnName("salary");
            entity.Property(e => e.Schedule)
                .HasMaxLength(50)
                .HasColumnName("schedule");
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.SerialNumber).HasColumnName("serialNumber");
            entity.Property(e => e.SupplierId).HasColumnName("supplierId");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Products_Suppliers");
        });

        modelBuilder.Entity<Suppliers>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
