using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenameDatabases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigurationMapFieldsRelation_tickets_configuration_fields~",
                table: "ConfigurationMapFieldsRelation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tickets_configuration_fields",
                table: "tickets_configuration_fields");

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("09ff2f50-c226-4847-a79f-366ec9be5072"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("0ea5bdd8-d952-4d17-9beb-e9520f3ade9e"));

            migrationBuilder.DeleteData(
                table: "tickets_configuration",
                keyColumn: "Id",
                keyValue: new Guid("8a574380-ecc0-489c-96a1-1112f5f72ba1"));

            migrationBuilder.RenameTable(
                name: "tickets_configuration_fields",
                newName: "tickets_configuration_metadata");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tickets_configuration_metadata",
                table: "tickets_configuration_metadata",
                column: "Id");

            migrationBuilder.InsertData(
                table: "tickets_configuration",
                columns: ["Id", "TicketType"],
                values: new object[,]
                {
                    { new Guid("304acc8e-506b-4d42-af86-d0fa60f1c1ce"), 1 },
                    { new Guid("40696dc4-7d84-407a-b044-a010675db6e7"), 0 },
                    { new Guid("eeae012d-f05b-4667-98a8-0d3964e78400"), 2 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigurationMapFieldsRelation_tickets_configuration_metada~",
                table: "ConfigurationMapFieldsRelation",
                column: "MetadataId",
                principalTable: "tickets_configuration_metadata",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigurationMapFieldsRelation_tickets_configuration_metada~",
                table: "ConfigurationMapFieldsRelation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tickets_configuration_metadata",
                table: "tickets_configuration_metadata");

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

            migrationBuilder.RenameTable(
                name: "tickets_configuration_metadata",
                newName: "tickets_configuration_fields");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tickets_configuration_fields",
                table: "tickets_configuration_fields",
                column: "Id");

            migrationBuilder.InsertData(
                table: "tickets_configuration",
                columns: ["Id", "TicketType"],
                values: new object[,]
                {
                    { new Guid("09ff2f50-c226-4847-a79f-366ec9be5072"), 1 },
                    { new Guid("0ea5bdd8-d952-4d17-9beb-e9520f3ade9e"), 2 },
                    { new Guid("8a574380-ecc0-489c-96a1-1112f5f72ba1"), 0 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigurationMapFieldsRelation_tickets_configuration_fields~",
                table: "ConfigurationMapFieldsRelation",
                column: "MetadataId",
                principalTable: "tickets_configuration_fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
