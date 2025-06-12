using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class MigracionRepesMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RutinaEjercicio");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Ejercicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Instrucciones",
                table: "Ejercicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GrupoMuscular",
                table: "Ejercicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAsistencia",
                table: "Citas",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Conclusion",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicacionId = table.Column<int>(type: "int", nullable: true),
                    EjercicioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Media_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "Publicaciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RutinasRealizadasClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RutinaId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    FechaRealizada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuracionMin = table.Column<int>(type: "int", nullable: true),
                    EjercicioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutinasRealizadasClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RutinasRealizadasClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RutinasRealizadasClientes_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RutinasRealizadasClientes_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EjercicioRealizadosPorClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RutinaEjercicioId = table.Column<int>(type: "int", nullable: false),
                    EjercicioId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeRealizo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EjercicioRealizadosPorClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EjercicioRealizadosPorClientes_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EjercicioRealizadosPorClientes_RutinasRealizadasClientes_RutinaEjercicioId",
                        column: x => x.RutinaEjercicioId,
                        principalTable: "RutinasRealizadasClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesParaEjerciciosDeCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EjercicioRealizadoId = table.Column<int>(type: "int", nullable: false),
                    NumeroSerie = table.Column<int>(type: "int", nullable: false),
                    Repeticiones = table.Column<int>(type: "int", nullable: false),
                    PesoUtilizado = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesParaEjerciciosDeCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriesParaEjerciciosDeCliente_EjercicioRealizadosPorClientes_EjercicioRealizadoId",
                        column: x => x.EjercicioRealizadoId,
                        principalTable: "EjercicioRealizadosPorClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EjercicioRealizadosPorClientes_EjercicioId",
                table: "EjercicioRealizadosPorClientes",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EjercicioRealizadosPorClientes_RutinaEjercicioId",
                table: "EjercicioRealizadosPorClientes",
                column: "RutinaEjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_EjercicioId",
                table: "Media",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_PublicacionId",
                table: "Media",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinasRealizadasClientes_ClienteId",
                table: "RutinasRealizadasClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinasRealizadasClientes_EjercicioId",
                table: "RutinasRealizadasClientes",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinasRealizadasClientes_RutinaId",
                table: "RutinasRealizadasClientes",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesParaEjerciciosDeCliente_EjercicioRealizadoId",
                table: "SeriesParaEjerciciosDeCliente",
                column: "EjercicioRealizadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "SeriesParaEjerciciosDeCliente");

            migrationBuilder.DropTable(
                name: "EjercicioRealizadosPorClientes");

            migrationBuilder.DropTable(
                name: "RutinasRealizadasClientes");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Ejercicios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instrucciones",
                table: "Ejercicios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GrupoMuscular",
                table: "Ejercicios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAsistencia",
                table: "Citas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Conclusion",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RutinaEjercicio",
                columns: table => new
                {
                    RutinaId = table.Column<int>(type: "int", nullable: false),
                    EjercicioId = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Repeticiones = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutinaEjercicio", x => new { x.RutinaId, x.EjercicioId });
                    table.ForeignKey(
                        name: "FK_RutinaEjercicio_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RutinaEjercicio_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RutinaEjercicio_EjercicioId",
                table: "RutinaEjercicio",
                column: "EjercicioId");
        }
    }
}
