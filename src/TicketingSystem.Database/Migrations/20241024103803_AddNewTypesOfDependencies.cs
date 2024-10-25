using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTypesOfDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:TicketDependenciesEnum.ticket_dependencies_enum", "sf_in_progress,sf_resolved,ss_dependnecy,sf_dependency,fs_dependency,ff_dependency")
                .Annotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .Annotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .Annotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic")
                .OldAnnotation("Npgsql:Enum:TicketDependenciesEnum.ticket_dependencies_enum", "sf_in_progress,sf_resolved")
                .OldAnnotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .OldAnnotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .OldAnnotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:TicketDependenciesEnum.ticket_dependencies_enum", "sf_in_progress,sf_resolved")
                .Annotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .Annotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .Annotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic")
                .OldAnnotation("Npgsql:Enum:TicketDependenciesEnum.ticket_dependencies_enum", "sf_in_progress,sf_resolved,ss_dependnecy,sf_dependency,fs_dependency,ff_dependency")
                .OldAnnotation("Npgsql:Enum:TicketMetadataTypeEnum.ticket_metadata_type_enum", "string,int,bool,date")
                .OldAnnotation("Npgsql:Enum:TicketStatusEnum.ticket_status_enum", "open,in_progress,resolved")
                .OldAnnotation("Npgsql:Enum:TicketTypeEnum.ticket_type_enum", "bug,improvement,epic");
        }
    }
}
