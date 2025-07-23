using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class medicionEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsObligatoria = table.Column<bool>(type: "bit", nullable: false),
                    EjercicioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicion_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValorMedicion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EjercicioRealizadoId = table.Column<int>(type: "int", nullable: false),
                    MedicionId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValorMedicion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValorMedicion_EjercicioRealizadosPorClientes_EjercicioRealizadoId",
                        column: x => x.EjercicioRealizadoId,
                        principalTable: "EjercicioRealizadosPorClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValorMedicion_Medicion_MedicionId",
                        column: x => x.MedicionId,
                        principalTable: "Medicion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicion_EjercicioId",
                table: "Medicion",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ValorMedicion_EjercicioRealizadoId",
                table: "ValorMedicion",
                column: "EjercicioRealizadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ValorMedicion_MedicionId",
                table: "ValorMedicion",
                column: "MedicionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValorMedicion");

            migrationBuilder.DropTable(
                name: "Medicion");
        }
    }
}
