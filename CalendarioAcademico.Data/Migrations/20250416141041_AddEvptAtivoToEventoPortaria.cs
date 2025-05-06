using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarioAcademico.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEvptAtivoToEventoPortaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EVPT_Ativo",
                table: "EVPT_Evento_Portaria",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EVPT_Ativo",
                table: "EVPT_Evento_Portaria");
        }
    }
}
