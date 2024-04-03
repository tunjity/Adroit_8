using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Adroit_v8.Migrations
{
    /// <inheritdoc />
    public partial class gddcfgvctfygbjnkl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FixedDepositInterestRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InterestRate = table.Column<string>(type: "text", nullable: false),
                    LoanAmountFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanAmountTo = table.Column<decimal>(type: "numeric", nullable: false),
                    FixedDepositTenor = table.Column<int>(type: "integer", nullable: false),
                    UniqueId = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    Createdby = table.Column<string>(type: "text", nullable: true),
                    Isdeleted = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDepositInterestRate", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FixedDepositInterestRate");
        }
    }
}
