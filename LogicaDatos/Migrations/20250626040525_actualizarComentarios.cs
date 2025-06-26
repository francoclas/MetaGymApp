using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class actualizarComentarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantLikes",
                table: "Comentarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComentarioPadreId",
                table: "Comentarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEdicion",
                table: "Comentarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ComentarioPadreId",
                table: "Comentarios",
                column: "ComentarioPadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Comentarios_ComentarioPadreId",
                table: "Comentarios",
                column: "ComentarioPadreId",
                principalTable: "Comentarios",
                principalColumn: "ComentarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Comentarios_ComentarioPadreId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_ComentarioPadreId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "CantLikes",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "ComentarioPadreId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "FechaEdicion",
                table: "Comentarios");
        }
    }
}
