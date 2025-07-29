using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class relacionSesionCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "SesionesRutina",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SesionesRutina_ClienteId",
                table: "SesionesRutina",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesRutina_Clientes_ClienteId",
                table: "SesionesRutina",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SesionesRutina_Clientes_ClienteId",
                table: "SesionesRutina");

            migrationBuilder.DropIndex(
                name: "IX_SesionesRutina_ClienteId",
                table: "SesionesRutina");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "SesionesRutina");
        }
    }
}
