using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class CorregirRutinaEjercicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_Ejercicios_EjercicioId",
                table: "SesionesRutina");

            migrationBuilder.DropIndex(
                name: "IX_SesionesRutina_EjercicioId",
                table: "SesionesRutina");

            migrationBuilder.DropColumn(
                name: "EjercicioId",
                table: "SesionesRutina");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EjercicioId",
                table: "SesionesRutina",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SesionesRutina_EjercicioId",
                table: "SesionesRutina",
                column: "EjercicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesRutina_Ejercicios_EjercicioId",
                table: "SesionesRutina",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id");
        }
    }
}
