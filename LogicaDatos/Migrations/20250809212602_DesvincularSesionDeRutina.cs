using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class DesvincularSesionDeRutina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina");

            migrationBuilder.DropColumn(
                name: "GrupoMuscular",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropColumn(
                name: "Instrucciones",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.RenameColumn(
                name: "TipoEjercicio",
                table: "EjercicioRealizadosPorClientes",
                newName: "InstruccionesHistorial");

            migrationBuilder.RenameColumn(
                name: "NombreEjercicio",
                table: "EjercicioRealizadosPorClientes",
                newName: "TipoHistorial");

            migrationBuilder.AlterColumn<int>(
                name: "RutinaAsignadaId",
                table: "SesionesRutina",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NombreRutinaHistorial",
                table: "SesionesRutina",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoRutinaHistorial",
                table: "SesionesRutina",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "EjercicioId",
                table: "EjercicioRealizadosPorClientes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "GrupoMuscularHistorial",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrlHistorial",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreHistorial",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina",
                column: "RutinaAsignadaId",
                principalTable: "RutinasAsignadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina");

            migrationBuilder.DropColumn(
                name: "NombreRutinaHistorial",
                table: "SesionesRutina");

            migrationBuilder.DropColumn(
                name: "TipoRutinaHistorial",
                table: "SesionesRutina");

            migrationBuilder.DropColumn(
                name: "GrupoMuscularHistorial",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropColumn(
                name: "ImagenUrlHistorial",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropColumn(
                name: "NombreHistorial",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.RenameColumn(
                name: "TipoHistorial",
                table: "EjercicioRealizadosPorClientes",
                newName: "NombreEjercicio");

            migrationBuilder.RenameColumn(
                name: "InstruccionesHistorial",
                table: "EjercicioRealizadosPorClientes",
                newName: "TipoEjercicio");

            migrationBuilder.AlterColumn<int>(
                name: "RutinaAsignadaId",
                table: "SesionesRutina",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EjercicioId",
                table: "EjercicioRealizadosPorClientes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrupoMuscular",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instrucciones",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                table: "EjercicioRealizadosPorClientes",
                column: "EjercicioId",
                principalTable: "Ejercicios",
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
    }
}
