using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class Comentarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Administradores_AdminId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Clientes_ClienteId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Profesionales_ProfesionalId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Publicaciones_PublicacionId",
                table: "Comentario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comentario",
                table: "Comentario");

            migrationBuilder.RenameTable(
                name: "Comentario",
                newName: "Comentarios");

            migrationBuilder.RenameIndex(
                name: "IX_Comentario_PublicacionId",
                table: "Comentarios",
                newName: "IX_Comentarios_PublicacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentario_ProfesionalId",
                table: "Comentarios",
                newName: "IX_Comentarios_ProfesionalId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentario_ClienteId",
                table: "Comentarios",
                newName: "IX_Comentarios_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentario_AdminId",
                table: "Comentarios",
                newName: "IX_Comentarios_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios",
                column: "ComentarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Administradores_AdminId",
                table: "Comentarios",
                column: "AdminId",
                principalTable: "Administradores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Clientes_ClienteId",
                table: "Comentarios",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Profesionales_ProfesionalId",
                table: "Comentarios",
                column: "ProfesionalId",
                principalTable: "Profesionales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Publicaciones_PublicacionId",
                table: "Comentarios",
                column: "PublicacionId",
                principalTable: "Publicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Administradores_AdminId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Clientes_ClienteId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Profesionales_ProfesionalId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Publicaciones_PublicacionId",
                table: "Comentarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios");

            migrationBuilder.RenameTable(
                name: "Comentarios",
                newName: "Comentario");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_PublicacionId",
                table: "Comentario",
                newName: "IX_Comentario_PublicacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_ProfesionalId",
                table: "Comentario",
                newName: "IX_Comentario_ProfesionalId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_ClienteId",
                table: "Comentario",
                newName: "IX_Comentario_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_AdminId",
                table: "Comentario",
                newName: "IX_Comentario_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comentario",
                table: "Comentario",
                column: "ComentarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Administradores_AdminId",
                table: "Comentario",
                column: "AdminId",
                principalTable: "Administradores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Clientes_ClienteId",
                table: "Comentario",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Profesionales_ProfesionalId",
                table: "Comentario",
                column: "ProfesionalId",
                principalTable: "Profesionales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Publicaciones_PublicacionId",
                table: "Comentario",
                column: "PublicacionId",
                principalTable: "Publicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
