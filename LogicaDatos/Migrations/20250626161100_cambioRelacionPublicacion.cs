using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class cambioRelacionPublicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publicaciones_Administradores_AdminId",
                table: "Publicaciones");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId",
                table: "Publicaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AdminAprobadorId",
                table: "Publicaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminCreadorId",
                table: "Publicaciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_AdminAprobadorId",
                table: "Publicaciones",
                column: "AdminAprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_AdminCreadorId",
                table: "Publicaciones",
                column: "AdminCreadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Publicaciones_Administradores_AdminAprobadorId",
                table: "Publicaciones",
                column: "AdminAprobadorId",
                principalTable: "Administradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Publicaciones_Administradores_AdminCreadorId",
                table: "Publicaciones",
                column: "AdminCreadorId",
                principalTable: "Administradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Publicaciones_Administradores_AdminId",
                table: "Publicaciones",
                column: "AdminId",
                principalTable: "Administradores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publicaciones_Administradores_AdminAprobadorId",
                table: "Publicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Publicaciones_Administradores_AdminCreadorId",
                table: "Publicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Publicaciones_Administradores_AdminId",
                table: "Publicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Publicaciones_AdminAprobadorId",
                table: "Publicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Publicaciones_AdminCreadorId",
                table: "Publicaciones");

            migrationBuilder.DropColumn(
                name: "AdminAprobadorId",
                table: "Publicaciones");

            migrationBuilder.DropColumn(
                name: "AdminCreadorId",
                table: "Publicaciones");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId",
                table: "Publicaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Publicaciones_Administradores_AdminId",
                table: "Publicaciones",
                column: "AdminId",
                principalTable: "Administradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
