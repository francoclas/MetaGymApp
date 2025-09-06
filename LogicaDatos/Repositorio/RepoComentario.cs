using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace LogicaDatos.Repositorio
{
    public class RepoComentario : IRepositorioComentario
    {
        private readonly DbContextApp _context;

        // Inyección del DbContext
        public RepoComentario(DbContextApp context)
        {
            _context = context;
        }

        // Comentarios raíz de una publicación, con autores y respuestas, activos y ordenados por fecha desc.
        public List<Comentario> ObtenerPorPublicacion(int publicacionId)
        {
            return _context.Comentarios
                .Include(c => c.Profesional)
                .Include(c => c.Cliente)
                .Include(c => c.Admin)
                .Include(c => c.Respuestas)
                .Where(c => c.PublicacionId == publicacionId && c.ComentarioPadreId == null && c.EstaActivo)
                .OrderByDescending(c => c.FechaCreacion)
                .ToList();
        }

        // Comentario puntual por Id, con navegación a publicación, autores, padre y respuestas.
        public Comentario ObtenerPorId(int comentarioId)
        {
            return _context.Comentarios
                .Include(c => c.Publicacion)
                .Include(c => c.Profesional)
                    .ThenInclude(P => P.FotosPerfil)
                .Include(c => c.Cliente)
                    .ThenInclude(C => C.FotosPerfil)
                .Include(c => c.Admin)
                    .ThenInclude(A => A.FotosPerfil)
                .Include(c => c.ComentarioPadre)
                .Include(c => c.Respuestas)
                .FirstOrDefault(c => c.ComentarioId == comentarioId);
        }

        // Alta de comentario: setea fecha y lo deja activo.
        public void Agregar(Comentario comentario)
        {
            comentario.FechaCreacion = DateTime.Now;
            comentario.EstaActivo = true;
            _context.Comentarios.Add(comentario);
            _context.SaveChanges();
        }

        // Editar contenido de un comentario activo.
        public void ActualizarContenido(int comentarioId, string nuevoContenido)
        {
            var comentario = _context.Comentarios.Find(comentarioId);
            if (comentario != null && comentario.EstaActivo)
            {
                comentario.Contenido = nuevoContenido;
                comentario.FechaEdicion = DateTime.Now;
                _context.SaveChanges();
            }
        }

        // Borrado lógico marca como inactivo.
        public void Desactivar(int comentarioId)
        {
            var comentario = _context.Comentarios.Find(comentarioId);
            if (comentario != null && comentario.EstaActivo)
            {
                comentario.EstaActivo = false;
                _context.SaveChanges();
            }
        }

        // ====================
        // Likes de comentarios
        // ====================

        // Verifica si ese usuario ya dio like al comentario.
        public bool UsuarioYaDioLike(int comentarioId, int usuarioId, string rol)
        {
            return _context.LikeComentarios.Any(l =>
                l.ComentarioId == comentarioId &&
                l.UsuarioId == usuarioId &&
                l.TipoUsuario == rol);
        }

        // Da like si no existía previamente.
        public void DarLike(int comentarioId, int usuarioId, string rol)
        {
            if (!UsuarioYaDioLike(comentarioId, usuarioId, rol))
            {
                _context.LikeComentarios.Add(new LikeComentario
                {
                    ComentarioId = comentarioId,
                    UsuarioId = usuarioId,
                    TipoUsuario = rol,
                    Fecha = DateTime.Now
                });

                var comentario = _context.Comentarios.Find(comentarioId);

                _context.SaveChanges();
            }
        }

        // Quitar like si existía.
        public void QuitarLike(int comentarioId, int usuarioId, string rol)
        {
            var like = _context.LikeComentarios.FirstOrDefault(l =>
                l.ComentarioId == comentarioId &&
                l.UsuarioId == usuarioId &&
                l.TipoUsuario == rol);

            if (like != null)
            {
                _context.LikeComentarios.Remove(like);
                _context.SaveChanges();
            }
        }

        // Contador de likes del comentario.
        public int ContarLikes(int comentarioId)
        {
            return _context.LikeComentarios.Count(l => l.ComentarioId == comentarioId);
        }

        // Guardar cambios de un comentario ya trackeado.
        public void Actualizar(Comentario comentario)
        {
            _context.SaveChanges();
        }
    }
}