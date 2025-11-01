using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetInteriorDesign.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "JetConfigurations",
                columns: table => new
                {
                    ConfigID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SeatingCapacity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JetConfigurations", x => x.ConfigID);
                    table.ForeignKey(
                        name: "FK_JetConfigurations_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteriorComponents",
                columns: table => new
                {
                    ComponentID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConfigID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Material = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    Tier = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false),
                    Width = table.Column<float>(type: "REAL", nullable: false),
                    Height = table.Column<float>(type: "REAL", nullable: false),
                    Depth = table.Column<float>(type: "REAL", nullable: false),
                    Cost = table.Column<float>(type: "REAL", nullable: false),
                    PropertiesJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteriorComponents", x => x.ComponentID);
                    table.ForeignKey(
                        name: "FK_InteriorComponents_JetConfigurations_ConfigID",
                        column: x => x.ConfigID,
                        principalTable: "JetConfigurations",
                        principalColumn: "ConfigID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentSettings",
                columns: table => new
                {
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SettingsJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentSettings", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_ComponentSettings_InteriorComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "InteriorComponents",
                        principalColumn: "ComponentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InteriorComponents_ConfigID",
                table: "InteriorComponents",
                column: "ConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_JetConfigurations_UserID",
                table: "JetConfigurations",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentSettings");

            migrationBuilder.DropTable(
                name: "InteriorComponents");

            migrationBuilder.DropTable(
                name: "JetConfigurations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
