using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetTracker.DataLayer.Migrations
{
    public partial class Migrationv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    AssetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    OfficeID = table.Column<int>(type: "int", nullable: false),
                    AssetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneOperator = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    OperatingSystem = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    RAM = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Processor = table.Column<string>(type: "nvarchar(128)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.AssetID);
                });

            migrationBuilder.CreateTable(
                name: "Offices",
                columns: table => new
                {
                    OfficeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offices", x => x.OfficeID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Offices");
        }
    }
}
