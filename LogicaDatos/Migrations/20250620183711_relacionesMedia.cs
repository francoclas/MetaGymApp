using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class relacionesMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Medias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Medias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfesionalId",
                table: "Medias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medias_AdminId",
                table: "Medias",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_ClienteId",
                table: "Medias",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_ProfesionalId",
                table: "Medias",
                column: "ProfesionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Administradores_AdminId",
                table: "Medias",
                column: "AdminId",
                principalTable: "Administradores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Clientes_ClienteId",
                table: "Medias",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Profesionales_ProfesionalId",
                table: "Medias",
                column: "ProfesionalId",
                principalTable: "Profesionales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Administradores_AdminId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Clientes_ClienteId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Profesionales_ProfesionalId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_AdminId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_ClienteId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_ProfesionalId",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "ProfesionalId",
                table: "Medias");
        }
    }
}
