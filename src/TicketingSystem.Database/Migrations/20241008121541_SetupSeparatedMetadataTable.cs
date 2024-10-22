using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SetupSeparatedMetadataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationMapFieldsRelation");

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("304acc8e-506b-4d42-af86-d0fa60f1c1ce"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("40696dc4-7d84-407a-b044-a010675db6e7"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("eeae012d-f05b-4667-98a8-0d3964e78400"));

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "tickets");

            migrationBuilder.CreateTable(
                name: "metadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MetadataId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadata_tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metadata_tickets_configuration_metadata_MetadataId",
                        column: x => x.MetadataId,
                        principalTable: "tickets_configuration_metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketConfigurationMapEntityTicketMetadataFieldEntity",
                columns: table => new
                {
                    ConfigurationsId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetadataId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketConfigurationMapEntityTicketMetadataFieldEntity", x => new { x.ConfigurationsId, x.MetadataId });
                    table.ForeignKey(
                        name: "FK_TicketConfigurationMapEntityTicketMetadataFieldEntity_ticke~",
                        column: x => x.ConfigurationsId,
                        principalTable: "tickets_configuration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketConfigurationMapEntityTicketMetadataFieldEntity_tick~1",
                        column: x => x.MetadataId,
                        principalTable: "tickets_configuration_metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metadata_MetadataId",
                table: "metadata",
                column: "MetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_metadata_TicketId",
                table: "metadata",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketConfigurationMapEntityTicketMetadataFieldEntity_Metad~",
                table: "TicketConfigurationMapEntityTicketMetadataFieldEntity",
                column: "MetadataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadata");

            migrationBuilder.DropTable(
                name: "TicketConfigurationMapEntityTicketMetadataFieldEntity");

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "tickets",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

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
                        name: "FK_ConfigurationMapFieldsRelation_tickets_configuration_metada~",
                        column: x => x.MetadataId,
                        principalTable: "tickets_configuration_metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tickets_configuration",
                columns: ["Id", "TicketType"],
                values: new object[,]
                {
                    { new Guid("304acc8e-506b-4d42-af86-d0fa60f1c1ce"), 1 },
                    { new Guid("40696dc4-7d84-407a-b044-a010675db6e7"), 0 },
                    { new Guid("eeae012d-f05b-4667-98a8-0d3964e78400"), 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationMapFieldsRelation_MetadataId",
                table: "ConfigurationMapFieldsRelation",
                column: "MetadataId");
        }
    }
}
