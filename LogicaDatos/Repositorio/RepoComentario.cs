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

        public RepoComentario(DbContextApp context)
        {
            _context = context;
        }

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

        public Comentario ObtenerPorId(int comentarioId)
        {
            return _context.Comentarios
                .Include(c => c.Profesional)
                .Include(c => c.Cliente)
                .Include(c => c.Admin)
                .Include(c => c.Respuestas)
                .FirstOrDefault(c => c.ComentarioId == comentarioId);
        }

        public void Agregar(Comentario comentario)
        {
            comentario.FechaCreacion = DateTime.Now;
            comentario.EstaActivo = true;
            _context.Comentarios.Add(comentario);
            _context.SaveChanges();
        }

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

        public void Desactivar(int comentarioId)
        {
            var comentario = _context.Comentarios.Find(comentarioId);
            if (comentario != null && comentario.EstaActivo)
            {
                comentario.EstaActivo = false;
                _context.SaveChanges();
            }
        }

        public void IncrementarLikes(int comentarioId)
        {
            var comentario = _context.Comentarios.Find(comentarioId);
            if (comentario != null)
            {
                comentario.CantLikes++;
                _context.SaveChanges();
            }
        }

        public void DecrementarLikes(int comentarioId)
        {
            var comentario = _context.Comentarios.Find(comentarioId);
            if (comentario != null && comentario.CantLikes > 0)
            {
                comentario.CantLikes--;
                _context.SaveChanges();
            }
        }
    }
}
