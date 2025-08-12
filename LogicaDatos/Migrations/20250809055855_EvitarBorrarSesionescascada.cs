using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class EvitarBorrarSesionescascada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinaEjercicios_Ejercicios_EjercicioId",
                table: "RutinaEjercicios");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinasAsignadas_Rutinas_RutinaId",
                table: "RutinasAsignadas");

            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina");

            migrationBuilder.AddForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RutinaEjercicios_Ejercicios_EjercicioId",
                table: "RutinaEjercicios",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RutinasAsignadas_Rutinas_RutinaId",
                table: "RutinasAsignadas",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina",
                column: "RutinaAsignadaId",
                principalTable: "RutinasAsignadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinaEjercicios_Ejercicios_EjercicioId",
                table: "RutinaEjercicios");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinasAsignadas_Rutinas_RutinaId",
                table: "RutinasAsignadas");

            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina");

            migrationBuilder.AddForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RutinaEjercicios_Ejercicios_EjercicioId",
                table: "RutinaEjercicios",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RutinaEjercicios_Rutinas_RutinaId",
                table: "RutinaEjercicios",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RutinasAsignadas_Rutinas_RutinaId",
                table: "RutinasAsignadas",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina",
                column: "RutinaAsignadaId",
                principalTable: "RutinasAsignadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
