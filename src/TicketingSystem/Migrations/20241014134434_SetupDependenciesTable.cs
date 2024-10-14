using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SetupDependenciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:TicketDependenciesEnum.ticket_dependencies_enum", "sf_in_progress,sf_resolved")
                .Annotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .Annotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .Annotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic")
                .OldAnnotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .OldAnnotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .OldAnnotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic");

            migrationBuilder.CreateTable(
                name: "tickets_dependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DependencyType = table.Column<int>(type: "integer", nullable: false),
                    SourceTicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetTicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketEntityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets_dependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tickets_dependencies_tickets_TicketEntityId",
                        column: x => x.TicketEntityId,
                        principalTable: "tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_dependencies_TicketEntityId",
                table: "tickets_dependencies",
                column: "TicketEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tickets_dependencies");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .Annotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .Annotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic")
                .OldAnnotation("Npgsql:Enum:TicketDependenciesEnum.ticket_dependencies_enum", "sf_in_progress,sf_resolved")
                .OldAnnotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .OldAnnotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .OldAnnotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic");
        }
    }
}
