using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class EditarSacarejercicioRutina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios");

            migrationBuilder.AddForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios");

            migrationBuilder.AddForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
