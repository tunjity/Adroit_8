using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Adroit_v8.Migrations
{
    /// <inheritdoc />
    public partial class ttrtrdfgttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adminproducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Minimuimamount = table.Column<decimal>(type: "numeric", nullable: true),
                    Maximuimamount = table.Column<decimal>(type: "numeric", nullable: true),
                    InterestRate = table.Column<string>(type: "text", nullable: true),
                    Tenor = table.Column<int>(type: "integer", nullable: true),
                    Startdate = table.Column<DateOnly>(type: "date", nullable: true),
                    AsEndDate = table.Column<bool>(type: "boolean", nullable: true),
                    Enddate = table.Column<DateOnly>(type: "date", nullable: true),
                    Datecreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adminproducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminRegularLoanCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmploymentTypeId = table.Column<string>(type: "text", nullable: false),
                    IsPercentage = table.Column<bool>(type: "boolean", nullable: false),
                    ChargeAmount = table.Column<string>(type: "text", nullable: false),
                    LoanAmountFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanAmountTo = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanTenorid = table.Column<int>(type: "integer", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRegularLoanCharges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminRegularLoanInterestRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<string>(type: "text", nullable: false),
                    InterestRate = table.Column<string>(type: "text", nullable: false),
                    LoanAmountFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanAmountTo = table.Column<decimal>(type: "numeric", nullable: false),
                    EmploymentTypeId = table.Column<string>(type: "text", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRegularLoanInterestRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applicationchannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicationchannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    BankCode = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleId = table.Column<int>(type: "integer", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    EscrowAccountNumber = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    GenderId = table.Column<int>(type: "integer", nullable: true),
                    DateOfBirth = table.Column<string>(type: "text", nullable: true),
                    MaritalStatusId = table.Column<int>(type: "integer", nullable: true),
                    NumberOfDependent = table.Column<int>(type: "integer", nullable: true),
                    EducationalLevelId = table.Column<int>(type: "integer", nullable: true),
                    FacebookId = table.Column<string>(type: "text", nullable: true),
                    WhatsappNumber = table.Column<string>(type: "text", nullable: true),
                    LinkedinId = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    BVN = table.Column<string>(type: "text", nullable: true),
                    AlternativePhoneNumber = table.Column<string>(type: "text", nullable: true),
                    EmailAddress = table.Column<string>(type: "text", nullable: true),
                    NIN = table.Column<string>(type: "text", nullable: true),
                    CustomerRef = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "0"),
                    IsActive = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "0"),
                    IsBlackListedCustomer = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "0"),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    RegistrationStageId = table.Column<int>(type: "integer", nullable: true),
                    RegistrationChannelId = table.Column<int>(type: "integer", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    HasValidBVN = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "0"),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Customer_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeclineReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclineReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Educationallevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educationallevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentSectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentSectors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employmentstatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employmentstatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employmenttypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employmenttypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeeFrequencys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeFrequencys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixedDepositAmountRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromAmount = table.Column<string>(type: "text", nullable: true),
                    ToAmount = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDepositAmountRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixedDepositPreliquidationChargeses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromAmount = table.Column<string>(type: "text", nullable: true),
                    ToAmount = table.Column<string>(type: "text", nullable: true),
                    IsPercentage = table.Column<string>(type: "text", nullable: true),
                    AmountCharge = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDepositPreliquidationChargeses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixedDepositStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDepositStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixedDepositTenors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Days = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDepositTenors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GovernmentIDCardTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GovernmentIDCardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LateFeePrincipals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LateFeePrincipals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LateFeeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LateFeeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lgas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    stateid = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lgas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanTenors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanTenors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maritalstatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maritalstatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Noofdependants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noofdependants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Noofyearofresidences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noofyearofresidences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductLateFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    LateFeeType = table.Column<string>(type: "text", nullable: false),
                    FixedPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LateFeePrincipal = table.Column<string>(type: "text", nullable: false),
                    FeeFrequency = table.Column<string>(type: "text", nullable: false),
                    GracePeriod = table.Column<string>(type: "text", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLateFees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductLoanProcessingFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    IsOptInProcessingFee = table.Column<bool>(type: "boolean", nullable: false),
                    IsFixedPrice = table.Column<bool>(type: "boolean", nullable: false),
                    FixedPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Principal = table.Column<decimal>(type: "numeric", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLoanProcessingFees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularLoanCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmploymentTypeId = table.Column<string>(type: "text", nullable: false),
                    IsPercentage = table.Column<bool>(type: "boolean", nullable: false),
                    ChargeAmount = table.Column<string>(type: "text", nullable: false),
                    LoanAmountFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanAmountTo = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanTenorid = table.Column<int>(type: "integer", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularLoanCharges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularLoanInterestRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InterestRate = table.Column<string>(type: "text", nullable: false),
                    LoanAmountFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanAmountTo = table.Column<decimal>(type: "numeric", nullable: false),
                    EmploymentTypeId = table.Column<string>(type: "text", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularLoanInterestRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularLoanTenors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularLoanTenors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Residentialstatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residentialstatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salarypaymentdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salarypaymentdates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salaryranges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaryranges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    countryid = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Titles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnderwriterLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    MinimuimAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    MaximuimAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Loanrange = table.Column<string>(type: "text", nullable: true),
                    Datecreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderwriterLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UtilityBillTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityBillTypes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adminproducts");

            migrationBuilder.DropTable(
                name: "AdminRegularLoanCharges");

            migrationBuilder.DropTable(
                name: "AdminRegularLoanInterestRates");

            migrationBuilder.DropTable(
                name: "Applicationchannels");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "DeclineReasons");

            migrationBuilder.DropTable(
                name: "Educationallevels");

            migrationBuilder.DropTable(
                name: "EmploymentSectors");

            migrationBuilder.DropTable(
                name: "Employmentstatuses");

            migrationBuilder.DropTable(
                name: "Employmenttypes");

            migrationBuilder.DropTable(
                name: "FeeFrequencys");

            migrationBuilder.DropTable(
                name: "FixedDepositAmountRanges");

            migrationBuilder.DropTable(
                name: "FixedDepositPreliquidationChargeses");

            migrationBuilder.DropTable(
                name: "FixedDepositStatuses");

            migrationBuilder.DropTable(
                name: "FixedDepositTenors");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "GovernmentIDCardTypes");

            migrationBuilder.DropTable(
                name: "LateFeePrincipals");

            migrationBuilder.DropTable(
                name: "LateFeeTypes");

            migrationBuilder.DropTable(
                name: "Lgas");

            migrationBuilder.DropTable(
                name: "LoanTenors");

            migrationBuilder.DropTable(
                name: "Maritalstatuses");

            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropTable(
                name: "Noofdependants");

            migrationBuilder.DropTable(
                name: "Noofyearofresidences");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "ProductLateFees");

            migrationBuilder.DropTable(
                name: "ProductLoanProcessingFees");

            migrationBuilder.DropTable(
                name: "RegularLoanCharges");

            migrationBuilder.DropTable(
                name: "RegularLoanInterestRates");

            migrationBuilder.DropTable(
                name: "RegularLoanTenors");

            migrationBuilder.DropTable(
                name: "Residentialstatuses");

            migrationBuilder.DropTable(
                name: "Salarypaymentdates");

            migrationBuilder.DropTable(
                name: "Salaryranges");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Titles");

            migrationBuilder.DropTable(
                name: "UnderwriterLevels");

            migrationBuilder.DropTable(
                name: "UtilityBillTypes");
        }
    }
}
