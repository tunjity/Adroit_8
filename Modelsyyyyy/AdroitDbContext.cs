using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Adroit_v8.Modelsyyyyy;

public partial class AdroitDbContext : DbContext
{
    public AdroitDbContext()
    {
    }

    public AdroitDbContext(DbContextOptions<AdroitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adminproduct> Adminproducts { get; set; }

    public virtual DbSet<Applicationchannel> Applicationchannels { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Educationallevel> Educationallevels { get; set; }

    public virtual DbSet<EmploymentSector> EmploymentSectors { get; set; }

    public virtual DbSet<Employmentstatus> Employmentstatuses { get; set; }

    public virtual DbSet<Employmenttype> Employmenttypes { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Lga> Lgas { get; set; }

    public virtual DbSet<Maritalstatus> Maritalstatuses { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<Newtable> Newtables { get; set; }

    public virtual DbSet<Noofdependant> Noofdependants { get; set; }

    public virtual DbSet<Noofyearofresidence> Noofyearofresidences { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<ProductLateFee> ProductLateFees { get; set; }

    public virtual DbSet<ProductLoanProcessingFee> ProductLoanProcessingFees { get; set; }

    public virtual DbSet<Residentialstatus> Residentialstatuses { get; set; }

    public virtual DbSet<Salarypaymentdate> Salarypaymentdates { get; set; }

    public virtual DbSet<Salaryrange> Salaryranges { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Title> Titles { get; set; }

    public virtual DbSet<UnderwriterLevel> UnderwriterLevels { get; set; }

    public virtual DbSet<UtilityBillType> UtilityBillTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=104.248.202.214;Database=adroit_db;Username=tayo;Password=rollingdollar");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Customer_pkey");

            entity.ToTable("Customer");

            entity.Property(e => e.Bvn).HasColumnName("BVN");
            entity.Property(e => e.DateCreated).HasDefaultValueSql("now()");
            entity.Property(e => e.HasValidBvn)
                .HasDefaultValueSql("0")
                .HasColumnName("HasValidBVN");
            entity.Property(e => e.IsActive).HasDefaultValueSql("0");
            entity.Property(e => e.IsBlackListedCustomer).HasDefaultValueSql("0");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");
            entity.Property(e => e.Nin).HasColumnName("NIN");
        });

        modelBuilder.Entity<Newtable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("newtable", "db");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.Property(e => e.Countryid).HasColumnName("countryid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
