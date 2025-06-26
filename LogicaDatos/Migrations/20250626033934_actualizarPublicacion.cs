using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class actualizarPublicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstaActiva",
                table: "Publicaciones");

            migrationBuilder.AddColumn<int>(
                name: "CantLikes",
                table: "Publicaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Publicaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Publicaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "Publicaciones",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantLikes",
                table: "Publicaciones");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Publicaciones");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Publicaciones");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "Publicaciones");

            migrationBuilder.AddColumn<bool>(
                name: "EstaActiva",
                table: "Publicaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
