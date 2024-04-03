using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Adroit_v8.Model;

public partial class AdroitDbContext : DbContext
{
    public AdroitDbContext()
    {
    }

    public AdroitDbContext(DbContextOptions<AdroitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminRegularLoanCharge> AdminRegularLoanCharges { get; set; }

    public virtual DbSet<AdminRegularLoanInterestRate> AdminRegularLoanInterestRates { get; set; }

    public virtual DbSet<Adminproduct> Adminproducts { get; set; }

    public virtual DbSet<Applicationchannel> Applicationchannels { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DeclineReason> DeclineReasons { get; set; }

    public virtual DbSet<Educationallevel> Educationallevels { get; set; }

    public virtual DbSet<EmploymentSector> EmploymentSectors { get; set; }

    public virtual DbSet<Employmentstatus> Employmentstatuses { get; set; }

    public virtual DbSet<Employmenttype> Employmenttypes { get; set; }

    public virtual DbSet<FeeFrequency> FeeFrequencys { get; set; }

    public virtual DbSet<FixedDepositAmountRange> FixedDepositAmountRanges { get; set; }

    public virtual DbSet<FixedDepositAmountRange1> FixedDepositAmountRanges1 { get; set; }

    public virtual DbSet<FixedDepositDailyProgress> FixedDepositDailyProgresses { get; set; }

    public virtual DbSet<FixedDepositInterestEarned> FixedDepositInterestEarneds { get; set; }

    public virtual DbSet<FixedDepositJob> FixedDepositJobs { get; set; }

    public virtual DbSet<FixedDepositLiquidation> FixedDepositLiquidations { get; set; }

    public virtual DbSet<FixedDepositPreLiquidationCharge> FixedDepositPreLiquidationCharges { get; set; }

    public virtual DbSet<FixedDepositPreliquidationChargese> FixedDepositPreliquidationChargeses { get; set; }

    public virtual DbSet<FixedDepositSetup> FixedDepositSetups { get; set; }

    public virtual DbSet<FixedDepositSetupTopUp> FixedDepositSetupTopUps { get; set; }

    public virtual DbSet<FixedDepositStatus> FixedDepositStatuses { get; set; }
    public virtual DbSet<FixDepositStatus> FixDepositStatus { get; set; }
    public virtual DbSet<EscrowStatus> EscrowStatus { get; set; }
    public virtual DbSet<SavingsStatus> SavingsStatus { get; set; }
    public virtual DbSet<BillsPaymentStatus> BillsPaymentStatus { get; set; }

    public virtual DbSet<FixedDepositTenor> FixedDepositTenors { get; set; }

    public virtual DbSet<FixedDepositTenor1> FixedDepositTenors1 { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<GovernmentIdcardType> GovernmentIdcardTypes { get; set; }

    public virtual DbSet<LateFeePrincipal> LateFeePrincipals { get; set; }

    public virtual DbSet<LateFeeType> LateFeeTypes { get; set; }

    public virtual DbSet<Lga> Lgas { get; set; }

    public virtual DbSet<LoanOfferLoanTenor> LoanOfferLoanTenors { get; set; }

    public virtual DbSet<LoanTenor> LoanTenors { get; set; }

    public virtual DbSet<Maritalstatus> Maritalstatuses { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<Newtable> Newtables { get; set; }

    public virtual DbSet<Noofdependant> Noofdependants { get; set; }

    public virtual DbSet<Noofyearofresidence> Noofyearofresidences { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<P2ploanTenor> P2ploanTenors { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<ProductLateFee> ProductLateFees { get; set; }

    public virtual DbSet<LoanRepaymentDetail> LoanRepaymentDetails { get; set; }
    public virtual DbSet<ProductLoanProcessingFee> ProductLoanProcessingFees { get; set; }

    public virtual DbSet<RegularLoanCharge> RegularLoanCharges { get; set; }

    public virtual DbSet<RegularLoanInterestRate> RegularLoanInterestRates { get; set; }

    public virtual DbSet<RegularLoanTenor> RegularLoanTenors { get; set; }

    public virtual DbSet<Residentialstatus> Residentialstatuses { get; set; }

    public virtual DbSet<Salarypaymentdate> Salarypaymentdates { get; set; }

    public virtual DbSet<Salaryrange> Salaryranges { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Title> Titles { get; set; }

    public virtual DbSet<UnderwriterLevel> UnderwriterLevels { get; set; }

    public virtual DbSet<UtilityBillType> UtilityBillTypes { get; set; }

    public virtual DbSet<WalletCustomerTransaction> WalletCustomerTransactions { get; set; }

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

        modelBuilder.Entity<FixedDepositAmountRange>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FixedDepositAmountRange");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("now()");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<FixedDepositAmountRange1>(entity =>
        {
            entity.ToTable("FixedDepositAmountRanges");
        });

        modelBuilder.Entity<FixedDepositDailyProgress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("FixedDepositDailyProgress_pkey");

            entity.ToTable("FixedDepositDailyProgress");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone");
        });
        modelBuilder.Entity<LoanRepaymentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LoanRepaymentDetails_pkey");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });
        modelBuilder.Entity<FixedDepositInterestEarned>(entity =>
        {
            entity.HasKey(e => e.InterestEarnedId).HasName("FixedDepositInterestEarned_pkey");

            entity.ToTable("FixedDepositInterestEarned");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone");
        });

        modelBuilder.Entity<FixedDepositJob>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FixedDepositJob");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<FixedDepositLiquidation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FixedDepositLiquidation_pkey");

            entity.ToTable("FixedDepositLiquidation");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone");
        });

        modelBuilder.Entity<FixedDepositPreLiquidationCharge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FixedDepositPreLiquidationCharges_pkey");
        });

        modelBuilder.Entity<FixedDepositSetup>(entity =>
        {
            entity.HasKey(e => e.SetupId).HasName("FixedDepositSetup_pkey");

            entity.ToTable("FixedDepositSetup");

            //entity.Property(e => e.DateCreated)
            //    .HasDefaultValueSql("now()")
            //    .HasColumnType("time with time zone");
        });

        modelBuilder.Entity<FixedDepositSetupTopUp>(entity =>
        {
            entity.HasKey(e => e.SetupTopUpId).HasName("FixedDepositSetupTopUp_pkey");

            entity.ToTable("FixedDepositSetupTopUp");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");
        });

        modelBuilder.Entity<FixedDepositTenor>(entity =>
        {
            entity.HasKey(e => e.TenorId).HasName("FixedDepositTenor_pkey");

            entity.ToTable("FixedDepositTenor");

            entity.Property(e => e.DateCreated).HasColumnType("time with time zone");
            entity.Property(e => e.IsActive).HasDefaultValueSql("1");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");
        });

        modelBuilder.Entity<FixedDepositTenor1>(entity =>
        {
            entity.ToTable("FixedDepositTenors");
        });

        modelBuilder.Entity<GovernmentIdcardType>(entity =>
        {
            entity.ToTable("GovernmentIDCardTypes");
        });

        modelBuilder.Entity<Lga>(entity =>
        {
            entity.Property(e => e.Stateid).HasColumnName("stateid");
        });

        modelBuilder.Entity<LoanOfferLoanTenor>(entity =>
        {
            entity.HasKey(e => e.TenorId).HasName("LoanOfferLoanTenor_pkey");

            entity.ToTable("LoanOfferLoanTenor");

            entity.Property(e => e.TenorId).HasDefaultValueSql("nextval('\"LoanOfferLoanTenor\"'::regclass)");
            entity.Property(e => e.DateCreated).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValueSql("true");
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

        modelBuilder.Entity<P2ploanTenor>(entity =>
        {
            entity.HasKey(e => e.TenorId).HasName("P2PLoanTenor_pkey");

            entity.ToTable("P2PLoanTenor");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValueSql("true");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PaymentGateway_pkey");

            entity.ToTable("PaymentType");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.Property(e => e.Countryid).HasColumnName("countryid");
        });

        modelBuilder.Entity<WalletCustomerTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("WalletTransaction_pkey");

            entity.ToTable("WalletCustomerTransaction");

            entity.HasIndex(e => e.PaystackPaymentReference, "Paystack_Reference_Unique_Constraint").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("ID");
            entity.Property(e => e.DrCr).HasColumnType("char");
            entity.Property(e => e.IsReversed).HasColumnType("char");
            entity.Property(e => e.PayerBalanceAfterDebit).HasColumnType("money");
            entity.Property(e => e.PayerBalanceBeforeDebit).HasColumnType("money");
            entity.Property(e => e.PaymentTypeId).HasColumnName("PaymentTypeID");
            entity.Property(e => e.PaystackCharge).HasColumnType("money");
            entity.Property(e => e.PhoneId).HasColumnName("PhoneID");
            entity.Property(e => e.ReceiverBalanceAfterCredit).HasColumnType("money");
            entity.Property(e => e.ReceiverBalanceBeforeCredit).HasColumnType("money");
            entity.Property(e => e.ServiceCharge).HasColumnType("money");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.WalletCustomerTransactions)
                .HasForeignKey(d => d.PaymentTypeId)
                .HasConstraintName("fk_payment_gateway");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
