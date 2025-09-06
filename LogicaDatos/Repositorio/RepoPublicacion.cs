using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace LogicaDatos.Repositorio
{
    // Maneja alta, actualización, likes y consultas de publicaciones
    public class RepoPublicacion : IRepositorioPublicacion
    {
        private readonly DbContextApp _context;
        public RepoPublicacion(DbContextApp context)
        {
            _context = context;
        }

        // Traer todas con comentarios y media
        public List<Publicacion> ObtenerTodas()
            => _context.Publicaciones.Include(p => p.Comentarios).Include(p => p.ListaMedia).ToList();

        // Una publicación puntual con todas las relaciones principales
        public Publicacion ObtenerPorId(int id)
        {
            return _context.Publicaciones
                 .Include(p => p.ListaMedia)
                 .Include(p => p.Profesional)
                 .Include(p => p.AdminCreador)
                 .Include(p => p.AdminAprobador)
                 .Include(p => p.Comentarios)
                 .FirstOrDefault(p => p.Id == id);
        }

        // Crear nueva publicación
        public void Crear(Publicacion publicacion)
        {
            publicacion.FechaCreacion = DateTime.Now;

            // Si es profesional, queda pendiente o programada
            if (publicacion.ProfesionalId != null)
            {
                publicacion.Estado = publicacion.FechaProgramada.HasValue
                    ? Enum_EstadoPublicacion.Programada
                    : Enum_EstadoPublicacion.Pendiente;
            }
            else
            {
                // Si la crea un admin, arranca aprobada
                publicacion.Estado = Enum_EstadoPublicacion.Aprobada;
            }

            _context.Publicaciones.Add(publicacion);
            _context.SaveChanges();
        }

        // Cambiar estado desde la revisión
        public void ActualizarEstado(int idPublicacion, Enum_EstadoPublicacion nuevoEstado, int adminId, string? motivoRechazo)
        {
            var pub = _context.Publicaciones.Find(idPublicacion);
            if (pub == null) return;

            pub.Estado = nuevoEstado;
            pub.AdminAprobadorId = adminId;
            pub.MotivoRechazo = motivoRechazo;
            pub.FechaModificacion = DateTime.Now;

            _context.SaveChanges();
        }

        // Listar pendientes de aprobación
        public List<Publicacion> ObtenerPendientes()
        {
            return _context.Publicaciones
                .Include(p => p.Profesional)
                .Where(p => p.Estado == Enum_EstadoPublicacion.Pendiente).ToList();
        }

        // Aprobadas y públicas
        public List<Publicacion> ObtenerAprobadasPublicas()
        {
            return _context.Publicaciones
                    .Include(p => p.ListaMedia)
                    .Include(p => p.Profesional)
                    .Include(p => p.AdminCreador)
                    .Include(p => p.AdminAprobador)
                    .Include(p => p.Comentarios)
                        .ThenInclude(c => c.Profesional)
                    .Include(p => p.Comentarios)
                        .ThenInclude(c => c.Cliente)
                    .Include(p => p.Comentarios)
                        .ThenInclude(c => c.Admin)
                    .Include(p => p.Comentarios)
                        .ThenInclude(c => c.Respuestas)
                    .ThenInclude(r => r.Profesional)
                    .Include(p => p.Comentarios)
                        .ThenInclude(c => c.Respuestas)
                    .ThenInclude(r => r.Cliente)
                    .Include(p => p.Comentarios)
                        .ThenInclude(c => c.Respuestas)
                    .ThenInclude(r => r.Admin)
                    .Where(p => p.Estado == Enum_EstadoPublicacion.Aprobada && !p.EsPrivada)
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToList();
        }

        // Publicaciones creadas por un admin puntual
        public List<Publicacion> ObtenerCreadasAdmin(int adminId)
        {
            return _context.Publicaciones
                .Include(p => p.ListaMedia)
                .Where(p => p.AdminCreadorId == adminId)
                .ToList();
        }

        // Publicaciones aprobadas por un admin
        public List<Publicacion> ObtenerAprobadasAdmin(int adminId)
        {
            return _context.Publicaciones
                .Include(p => p.ListaMedia)
                .Include(p => p.Profesional)
                .Where(p => p.AdminAprobadorId == adminId && p.Estado == Enum_EstadoPublicacion.Aprobada || p.Estado == Enum_EstadoPublicacion.Oculto)
                .ToList();
        }

        // Publicaciones rechazadas por un admin
        public List<Publicacion> ObtenerRechazadasAdmin(int adminId)
        {
            return _context.Publicaciones
                .Include(p => p.ListaMedia)
                .Include(p => p.Profesional)
                .Where(p => p.AdminAprobadorId == adminId && p.Estado == Enum_EstadoPublicacion.Rechazada)
                .ToList();
        }

        // Update genérico de una publicación
        public void Actualizar(Publicacion entidad)
        {
            _context.Publicaciones.Update(entidad);
            _context.SaveChanges();
        }

        // Verifica si un usuario ya dio like (según rol)
        public bool UsuarioYaDioLike(int publicacionId, int usuarioId, string rol)
        {
            return _context.LikePublicaciones.Any(l =>
                l.PublicacionId == publicacionId &&
                l.UsuarioId == usuarioId &&
                l.TipoUsuario == rol);
        }

        // Da like y suma al contador de la publicación
        public void DarLike(int publicacionId, int usuarioId, string rol)
        {
            if (!UsuarioYaDioLike(publicacionId, usuarioId, rol))
            {
                _context.LikePublicaciones.Add(new LikePublicacion
                {
                    PublicacionId = publicacionId,
                    UsuarioId = usuarioId,
                    TipoUsuario = rol,
                    Fecha = DateTime.Now
                });

                var pub = _context.Publicaciones.Find(publicacionId);
                if (pub != null) pub.CantLikes++;

                _context.SaveChanges();
            }
        }

        // Quita like y resta del contador si corresponde
        public void QuitarLike(int publicacionId, int usuarioId, string rol)
        {
            var like = _context.LikePublicaciones.FirstOrDefault(l =>
                l.PublicacionId == publicacionId &&
                l.UsuarioId == usuarioId &&
                l.TipoUsuario == rol);

            if (like != null)
            {
                _context.LikePublicaciones.Remove(like);

                var pub = _context.Publicaciones.Find(publicacionId);
                if (pub != null && pub.CantLikes > 0) pub.CantLikes--;

                _context.SaveChanges();
            }
        }

        // Contador de likes
        public int ContarLikes(int publicacionId)
        {
            return _context.LikePublicaciones.Count(l => l.PublicacionId == publicacionId);
        }

        // Noticias públicas
        public List<Publicacion> ObtenerNovedades()
        {
            return _context.Publicaciones
                .Include(p => p.ListaMedia)
                .Include(p => p.Profesional)
                .Include(p => p.AdminCreador)
                .Where(p => p.MostrarEnNoticiasPublicas == true).ToList();
        }
    }
}