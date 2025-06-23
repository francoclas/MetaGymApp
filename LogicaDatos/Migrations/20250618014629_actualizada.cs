using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class actualizada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Ejercicios_EjercicioId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Establecimientos_EstablecimientoId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Publicaciones_PublicacionId",
                table: "Media");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Media",
                table: "Media");

            migrationBuilder.RenameTable(
                name: "Media",
                newName: "Medias");

            migrationBuilder.RenameIndex(
                name: "IX_Media_PublicacionId",
                table: "Medias",
                newName: "IX_Medias_PublicacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Media_EstablecimientoId",
                table: "Medias",
                newName: "IX_Medias_EstablecimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_Media_EjercicioId",
                table: "Medias",
                newName: "IX_Medias_EjercicioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medias",
                table: "Medias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Ejercicios_EjercicioId",
                table: "Medias",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Establecimientos_EstablecimientoId",
                table: "Medias",
                column: "EstablecimientoId",
                principalTable: "Establecimientos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Publicaciones_PublicacionId",
                table: "Medias",
                column: "PublicacionId",
                principalTable: "Publicaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Ejercicios_EjercicioId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Establecimientos_EstablecimientoId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Publicaciones_PublicacionId",
                table: "Medias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medias",
                table: "Medias");

            migrationBuilder.RenameTable(
                name: "Medias",
                newName: "Media");

            migrationBuilder.RenameIndex(
                name: "IX_Medias_PublicacionId",
                table: "Media",
                newName: "IX_Media_PublicacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Medias_EstablecimientoId",
                table: "Media",
                newName: "IX_Media_EstablecimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_Medias_EjercicioId",
                table: "Media",
                newName: "IX_Media_EjercicioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Media",
                table: "Media",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Ejercicios_EjercicioId",
                table: "Media",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Establecimientos_EstablecimientoId",
                table: "Media",
                column: "EstablecimientoId",
                principalTable: "Establecimientos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Publicaciones_PublicacionId",
                table: "Media",
                column: "PublicacionId",
                principalTable: "Publicaciones",
                principalColumn: "Id");
        }
    }
}
