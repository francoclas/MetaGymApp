using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class actualizarRutina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_RutinasRealizadasClientes_RutinaEjercicioId",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Clientes_ClienteId",
                table: "Rutinas");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinasRealizadasClientes_Clientes_ClienteId",
                table: "RutinasRealizadasClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinasRealizadasClientes_Ejercicios_EjercicioId",
                table: "RutinasRealizadasClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_RutinasRealizadasClientes_Rutinas_RutinaId",
                table: "RutinasRealizadasClientes");

            migrationBuilder.DropIndex(
                name: "IX_Rutinas_ClienteId",
                table: "Rutinas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RutinasRealizadasClientes",
                table: "RutinasRealizadasClientes");

            migrationBuilder.DropIndex(
                name: "IX_RutinasRealizadasClientes_ClienteId",
                table: "RutinasRealizadasClientes");

            migrationBuilder.DropColumn(
                name: "NumeroSerie",
                table: "SeriesParaEjerciciosDeCliente");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "RutinasRealizadasClientes");

            migrationBuilder.RenameTable(
                name: "RutinasRealizadasClientes",
                newName: "SesionesRutina");

            migrationBuilder.RenameColumn(
                name: "RutinaEjercicioId",
                table: "EjercicioRealizadosPorClientes",
                newName: "SesionRutinaId");

            migrationBuilder.RenameIndex(
                name: "IX_EjercicioRealizadosPorClientes_RutinaEjercicioId",
                table: "EjercicioRealizadosPorClientes",
                newName: "IX_EjercicioRealizadosPorClientes_SesionRutinaId");

            migrationBuilder.RenameColumn(
                name: "RutinaId",
                table: "SesionesRutina",
                newName: "RutinaAsignadaId");

            migrationBuilder.RenameIndex(
                name: "IX_RutinasRealizadasClientes_RutinaId",
                table: "SesionesRutina",
                newName: "IX_SesionesRutina_RutinaAsignadaId");

            migrationBuilder.RenameIndex(
                name: "IX_RutinasRealizadasClientes_EjercicioId",
                table: "SesionesRutina",
                newName: "IX_SesionesRutina_EjercicioId");

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "EjercicioRealizadosPorClientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SesionesRutina",
                table: "SesionesRutina",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RutinasAsignadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    RutinaId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutinasAsignadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RutinasAsignadas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RutinasAsignadas_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RutinasAsignadas_ClienteId",
                table: "RutinasAsignadas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinasAsignadas_RutinaId",
                table: "RutinasAsignadas",
                column: "RutinaId");

            migrationBuilder.AddForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_SesionesRutina_SesionRutinaId",
                table: "EjercicioRealizadosPorClientes",
                column: "SesionRutinaId",
                principalTable: "SesionesRutina",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesRutina_Ejercicios_EjercicioId",
                table: "SesionesRutina",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina",
                column: "RutinaAsignadaId",
                principalTable: "RutinasAsignadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_SesionesRutina_SesionRutinaId",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_Ejercicios_EjercicioId",
                table: "SesionesRutina");

            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_RutinasAsignadas_RutinaAsignadaId",
                table: "SesionesRutina");

            migrationBuilder.DropTable(
                name: "RutinasAsignadas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SesionesRutina",
                table: "SesionesRutina");

            migrationBuilder.DropColumn(
                name: "Orden",
                table: "EjercicioRealizadosPorClientes");

            migrationBuilder.RenameTable(
                name: "SesionesRutina",
                newName: "RutinasRealizadasClientes");

            migrationBuilder.RenameColumn(
                name: "SesionRutinaId",
                table: "EjercicioRealizadosPorClientes",
                newName: "RutinaEjercicioId");

            migrationBuilder.RenameIndex(
                name: "IX_EjercicioRealizadosPorClientes_SesionRutinaId",
                table: "EjercicioRealizadosPorClientes",
                newName: "IX_EjercicioRealizadosPorClientes_RutinaEjercicioId");

            migrationBuilder.RenameColumn(
                name: "RutinaAsignadaId",
                table: "RutinasRealizadasClientes",
                newName: "RutinaId");

            migrationBuilder.RenameIndex(
                name: "IX_SesionesRutina_RutinaAsignadaId",
                table: "RutinasRealizadasClientes",
                newName: "IX_RutinasRealizadasClientes_RutinaId");

            migrationBuilder.RenameIndex(
                name: "IX_SesionesRutina_EjercicioId",
                table: "RutinasRealizadasClientes",
                newName: "IX_RutinasRealizadasClientes_EjercicioId");

            migrationBuilder.AddColumn<int>(
                name: "NumeroSerie",
                table: "SeriesParaEjerciciosDeCliente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Rutinas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "RutinasRealizadasClientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RutinasRealizadasClientes",
                table: "RutinasRealizadasClientes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rutinas_ClienteId",
                table: "Rutinas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinasRealizadasClientes_ClienteId",
                table: "RutinasRealizadasClientes",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_EjercicioRealizadosPorClientes_RutinasRealizadasClientes_RutinaEjercicioId",
                table: "EjercicioRealizadosPorClientes",
                column: "RutinaEjercicioId",
                principalTable: "RutinasRealizadasClientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Clientes_ClienteId",
                table: "Rutinas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RutinasRealizadasClientes_Clientes_ClienteId",
                table: "RutinasRealizadasClientes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RutinasRealizadasClientes_Ejercicios_EjercicioId",
                table: "RutinasRealizadasClientes",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RutinasRealizadasClientes_Rutinas_RutinaId",
                table: "RutinasRealizadasClientes",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
