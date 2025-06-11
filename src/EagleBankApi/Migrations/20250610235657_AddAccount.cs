using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EagleBankApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountNumber = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 8, nullable: false),
                    SortCode = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false, defaultValue: "10-10-10"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Balance = table.Column<double>(type: "decimal(18,2)", nullable: false, defaultValue: 0.0),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false, defaultValue: "GBP"),
                    CreatedTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountNumber);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
