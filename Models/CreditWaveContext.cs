﻿

using Adroit_v8.Models.Administration;
using Adroit_v8.Models.CRM;

namespace Adroit_v8.Models;

public partial class CreditWaveContext : DbContext
{
    public CreditWaveContext()
    {
    }

    public CreditWaveContext(DbContextOptions<CreditWaveContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Adminproduct> Adminproducts { get; set; }

    public virtual DbSet<Applicationchannel> Applicationchannels { get; set; }
    public virtual DbSet<DeclineReason> DeclineReasons { get; set; }
    public virtual DbSet<Bank> Banks { get; set; }
    public virtual DbSet<LoanTenor> LoanTenors { get; set; }
    public virtual DbSet<FeeFrequency> FeeFrequencys { get; set; }
    public virtual DbSet<LateFeePrincipal> LateFeePrincipals { get; set; }
    public virtual DbSet<LateFeeType> LateFeeTypes { get; set; }
    public virtual DbSet<FixedDepositTenor> FixedDepositTenors { get; set; }
    public virtual DbSet<AdminRegularLoanCharge> AdminRegularLoanCharges { get; set; }
    public virtual DbSet<RegularLoanCharge> RegularLoanCharges { get; set; }
    public virtual DbSet<AdminRegularLoanInterestRate> AdminRegularLoanInterestRates { get; set; }
    public virtual DbSet<RegularLoanInterestRate> RegularLoanInterestRates { get; set; }
    public virtual DbSet<FixedDepositPreliquidationCharges> FixedDepositPreliquidationChargeses { get; set; }
    public virtual DbSet<FixedDepositStatus> FixedDepositStatuses { get; set; }
    public virtual DbSet<FixedDepositAmountRange> FixedDepositAmountRanges { get; set; }
    public virtual DbSet<FixedDepositInterestRate> FixedDepositInterestRate { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Educationallevel> Educationallevels { get; set; }

    public virtual DbSet<EmploymentSector> EmploymentSectors { get; set; }

    public virtual DbSet<Employmentstatus> Employmentstatuses { get; set; }

    public virtual DbSet<RegularLoanTenor> RegularLoanTenors { get; set; }
    public virtual DbSet<GovernmentIDCardType> GovernmentIDCardTypes { get; set; }
    public virtual DbSet<Employmenttype> Employmenttypes { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Lga> Lgas { get; set; }

    public virtual DbSet<Maritalstatus> Maritalstatuses { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }


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


        modelBuilder.Entity<State>(entity =>
        {
            entity.Property(e => e.Countryid).HasColumnName("countryid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
