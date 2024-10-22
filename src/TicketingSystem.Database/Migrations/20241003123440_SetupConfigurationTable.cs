using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SetupConfigurationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConfigurationId",
                table: "tickets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "tickets_configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets_configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tickets_configuration_fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    PropertyType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets_configuration_fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationMapFieldsRelation",
                columns: table => new
                {
                    ConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetadataId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationMapFieldsRelation", x => new { x.ConfigurationId, x.MetadataId });
                    table.ForeignKey(
                        name: "FK_ConfigurationMapFieldsRelation_tickets_configuration_Config~",
                        column: x => x.ConfigurationId,
                        principalTable: "tickets_configuration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationMapFieldsRelation_tickets_configuration_fields~",
                        column: x => x.MetadataId,
                        principalTable: "tickets_configuration_fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tickets_configuration",
                columns: ["Id", "TicketType"],
                values: new object[,]
                {
                    { new Guid("4333325b-c1c4-451b-81d1-908e13ea2ee7"), 2 },
                    { new Guid("b861393b-678f-48df-81ff-c02de58006da"), 1 },
                    { new Guid("d6028a9e-2c80-46e2-bfd7-ccb8ee99c8bf"), 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_ConfigurationId",
                table: "tickets",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationMapFieldsRelation_MetadataId",
                table: "ConfigurationMapFieldsRelation",
                column: "MetadataId");

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_tickets_configuration_ConfigurationId",
                table: "tickets",
                column: "ConfigurationId",
                principalTable: "tickets_configuration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_tickets_configuration_ConfigurationId",
                table: "tickets");

            migrationBuilder.DropTable(
                name: "ConfigurationMapFieldsRelation");

            migrationBuilder.DropTable(
                name: "tickets_configuration");

            migrationBuilder.DropTable(
                name: "tickets_configuration_fields");

            migrationBuilder.DropIndex(
                name: "IX_tickets_ConfigurationId",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "ConfigurationId",
                table: "tickets");
        }
    }
}
