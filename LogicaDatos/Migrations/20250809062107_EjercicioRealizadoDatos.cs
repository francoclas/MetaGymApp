using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class EjercicioRealizadoDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrupoMuscular",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instrucciones",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreEjercicio",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoEjercicio",
                table: "EjercicioRealizadosPorClientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrupoMuscular",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropColumn(
                name: "Instrucciones",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropColumn(
                name: "NombreEjercicio",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropColumn(
                name: "TipoEjercicio",
                table: "EjercicioRealizadosPorClientes");
        }
    }
}
