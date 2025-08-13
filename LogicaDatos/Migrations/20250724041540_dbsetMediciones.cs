using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class dbsetMediciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicion_Ejercicios_EjercicioId",
                table: "Medicion");

            migrationBuilder.DropForeignKey(
                name: "FK_ValorMedicion_EjercicioRealizadosPorClientes_EjercicioRealizadoId",
                table: "ValorMedicion");

            migrationBuilder.DropForeignKey(
                name: "FK_ValorMedicion_Medicion_MedicionId",
                table: "ValorMedicion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValorMedicion",
                table: "ValorMedicion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medicion",
                table: "Medicion");

            migrationBuilder.RenameTable(
                name: "ValorMedicion",
                newName: "MedicionesEjercicio");

            migrationBuilder.RenameTable(
                name: "Medicion",
                newName: "Mediciones");

            migrationBuilder.RenameIndex(
                name: "IX_ValorMedicion_MedicionId",
                table: "MedicionesEjercicio",
                newName: "IX_MedicionesEjercicio_MedicionId");

            migrationBuilder.RenameIndex(
                name: "IX_ValorMedicion_EjercicioRealizadoId",
                table: "MedicionesEjercicio",
                newName: "IX_MedicionesEjercicio_EjercicioRealizadoId");

            migrationBuilder.RenameIndex(
                name: "IX_Medicion_EjercicioId",
                table: "Mediciones",
                newName: "IX_Mediciones_EjercicioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicionesEjercicio",
                table: "MedicionesEjercicio",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mediciones",
                table: "Mediciones",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mediciones_Ejercicios_EjercicioId",
                table: "Mediciones",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicionesEjercicio_EjercicioRealizadosPorClientes_EjercicioRealizadoId",
                table: "MedicionesEjercicio",
                column: "EjercicioRealizadoId",
                principalTable: "EjercicioRealizadosPorClientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicionesEjercicio_Mediciones_MedicionId",
                table: "MedicionesEjercicio",
                column: "MedicionId",
                principalTable: "Mediciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mediciones_Ejercicios_EjercicioId",
                table: "Mediciones");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicionesEjercicio_EjercicioRealizadosPorClientes_EjercicioRealizadoId",
                table: "MedicionesEjercicio");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicionesEjercicio_Mediciones_MedicionId",
                table: "MedicionesEjercicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicionesEjercicio",
                table: "MedicionesEjercicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mediciones",
                table: "Mediciones");

            migrationBuilder.RenameTable(
                name: "MedicionesEjercicio",
                newName: "ValorMedicion");

            migrationBuilder.RenameTable(
                name: "Mediciones",
                newName: "Medicion");

            migrationBuilder.RenameIndex(
                name: "IX_MedicionesEjercicio_MedicionId",
                table: "ValorMedicion",
                newName: "IX_ValorMedicion_MedicionId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicionesEjercicio_EjercicioRealizadoId",
                table: "ValorMedicion",
                newName: "IX_ValorMedicion_EjercicioRealizadoId");

            migrationBuilder.RenameIndex(
                name: "IX_Mediciones_EjercicioId",
                table: "Medicion",
                newName: "IX_Medicion_EjercicioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValorMedicion",
                table: "ValorMedicion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medicion",
                table: "Medicion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicion_Ejercicios_EjercicioId",
                table: "Medicion",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ValorMedicion_EjercicioRealizadosPorClientes_EjercicioRealizadoId",
                table: "ValorMedicion",
                column: "EjercicioRealizadoId",
                principalTable: "EjercicioRealizadosPorClientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ValorMedicion_Medicion_MedicionId",
                table: "ValorMedicion",
                column: "MedicionId",
                principalTable: "Medicion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
