using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCustomMetadataForTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tickets_metadata");

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("4333325b-c1c4-451b-81d1-908e13ea2ee7"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("b861393b-678f-48df-81ff-c02de58006da"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("d6028a9e-2c80-46e2-bfd7-ccb8ee99c8bf"));

            migrationBuilder.InsertData(
                table: "tickets_configuration",
                columns: ["Id", "TicketType"],
                values: new object[,]
                {
                    { new Guid("0c9edcab-e027-44f8-99f5-95524cd3e856"), 2 },
                    { new Guid("51f4e55f-d274-496e-8c38-803ed72b1fb5"), 0 },
                    { new Guid("df44f012-eef2-470f-9920-ee9f415eee57"), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("0c9edcab-e027-44f8-99f5-95524cd3e856"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("51f4e55f-d274-496e-8c38-803ed72b1fb5"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("df44f012-eef2-470f-9920-ee9f415eee57"));

            migrationBuilder.CreateTable(
                name: "tickets_metadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: true),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    PropertyType = table.Column<int>(type: "integer", nullable: false),
                    PropertyValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets_metadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tickets_metadata_tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "tickets",
                        principalColumn: "Id");
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
                name: "IX_tickets_metadata_TicketId",
                table: "tickets_metadata",
                column: "TicketId");
        }
    }
}
