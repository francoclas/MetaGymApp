using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class registroInteracciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantLikes",
                table: "Comentarios");

            migrationBuilder.CreateTable(
                name: "LikeComentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComentarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    ProfesionalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeComentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikeComentarios_Administradores_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Administradores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikeComentarios_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikeComentarios_Comentarios_ComentarioId",
                        column: x => x.ComentarioId,
                        principalTable: "Comentarios",
                        principalColumn: "ComentarioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikeComentarios_Profesionales_ProfesionalId",
                        column: x => x.ProfesionalId,
                        principalTable: "Profesionales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LikePublicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicacionId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    ProfesionalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikePublicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikePublicaciones_Administradores_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Administradores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikePublicaciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikePublicaciones_Profesionales_ProfesionalId",
                        column: x => x.ProfesionalId,
                        principalTable: "Profesionales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikePublicaciones_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeComentarios_AdminId",
                table: "LikeComentarios",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeComentarios_ClienteId",
                table: "LikeComentarios",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeComentarios_ComentarioId",
                table: "LikeComentarios",
                column: "ComentarioId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeComentarios_ProfesionalId",
                table: "LikeComentarios",
                column: "ProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_LikePublicaciones_AdminId",
                table: "LikePublicaciones",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_LikePublicaciones_ClienteId",
                table: "LikePublicaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_LikePublicaciones_ProfesionalId",
                table: "LikePublicaciones",
                column: "ProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_LikePublicaciones_PublicacionId",
                table: "LikePublicaciones",
                column: "PublicacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikeComentarios");

            migrationBuilder.DropTable(
                name: "LikePublicaciones");

            migrationBuilder.AddColumn<int>(
                name: "CantLikes",
                table: "Comentarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
