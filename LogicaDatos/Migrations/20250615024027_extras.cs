using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class extras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstablecimientoId",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinalizacion",
                table: "Citas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_EstablecimientoId",
                table: "Media",
                column: "EstablecimientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Establecimientos_EstablecimientoId",
                table: "Media",
                column: "EstablecimientoId",
                principalTable: "Establecimientos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Establecimientos_EstablecimientoId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_EstablecimientoId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "EstablecimientoId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FechaFinalizacion",
                table: "Citas");
        }
    }
}
