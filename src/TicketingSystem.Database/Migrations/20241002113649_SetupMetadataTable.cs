using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SetupMetadataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .Annotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .Annotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic")
                .OldAnnotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .OldAnnotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic");

            migrationBuilder.CreateTable(
                name: "tickets_metadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    PropertyValue = table.Column<string>(type: "text", nullable: false),
                    PropertyType = table.Column<int>(type: "integer", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_tickets_metadata_TicketId",
                table: "tickets_metadata",
                column: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tickets_metadata");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .Annotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic")
                .OldAnnotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .OldAnnotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .OldAnnotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic");
        }
    }
}
