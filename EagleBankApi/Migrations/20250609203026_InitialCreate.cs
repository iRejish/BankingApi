using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EagleBankApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address_Line1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address_Line2 = table.Column<string>(type: "TEXT", nullable: true),
                    Address_Line3 = table.Column<string>(type: "TEXT", nullable: true),
                    Address_Town = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Address_County = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Address_Postcode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
