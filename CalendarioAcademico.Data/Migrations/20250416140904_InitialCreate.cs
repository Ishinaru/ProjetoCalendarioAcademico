using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarioAcademico.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EVPT_Evento_Portaria");

            migrationBuilder.DropTable(
                name: "EVNT_Evento");

            migrationBuilder.DropTable(
                name: "PORT_Portaria");

            migrationBuilder.DropTable(
                name: "CAD_Calendario");
        }
    }
}
