using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class updateAgendaProfesional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoAtencionId",
                table: "Citas",
                type: "int",
                nullable: true
                );

            migrationBuilder.CreateTable(
                name: "AgendaProfesionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesionalId = table.Column<int>(type: "int", nullable: false),
                    Dia = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaProfesionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendaProfesionales_Profesionales_ProfesionalId",
                        column: x => x.ProfesionalId,
                        principalTable: "Profesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoAtenciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuracionMin = table.Column<int>(type: "int", nullable: false),
                    EspecialidadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoAtenciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipoAtenciones_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfesionalTipoAtencion",
                columns: table => new
                {
                    ProfesionalesId = table.Column<int>(type: "int", nullable: false),
                    TiposAtencionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfesionalTipoAtencion", x => new { x.ProfesionalesId, x.TiposAtencionId });
                    table.ForeignKey(
                        name: "FK_ProfesionalTipoAtencion_Profesionales_ProfesionalesId",
                        column: x => x.ProfesionalesId,
                        principalTable: "Profesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfesionalTipoAtencion_TipoAtenciones_TiposAtencionId",
                        column: x => x.TiposAtencionId,
                        principalTable: "TipoAtenciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_TipoAtencionId",
                table: "Citas",
                column: "TipoAtencionId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaProfesionales_ProfesionalId",
                table: "AgendaProfesionales",
                column: "ProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesionalTipoAtencion_TiposAtencionId",
                table: "ProfesionalTipoAtencion",
                column: "TiposAtencionId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoAtenciones_EspecialidadId",
                table: "TipoAtenciones",
                column: "EspecialidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_TipoAtenciones_TipoAtencionId",
                table: "Citas",
                column: "TipoAtencionId",
                principalTable: "TipoAtenciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_TipoAtenciones_TipoAtencionId",
                table: "Citas");

            migrationBuilder.DropTable(
                name: "AgendaProfesionales");

            migrationBuilder.DropTable(
                name: "ProfesionalTipoAtencion");

            migrationBuilder.DropTable(
                name: "TipoAtenciones");

            migrationBuilder.DropIndex(
                name: "IX_Citas_TipoAtencionId",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "TipoAtencionId",
                table: "Citas");
        }
    }
}
