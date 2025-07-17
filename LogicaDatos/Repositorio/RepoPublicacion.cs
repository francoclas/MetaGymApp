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
    public class RepoPublicacion : IRepositorioPublicacion
    {
        private readonly DbContextApp _context;
        public RepoPublicacion(DbContextApp context)
        {
            _context = context;
        }  

        public List<Publicacion> ObtenerTodas()
            => _context.Publicaciones.Include(p => p.Comentarios).Include(p => p.ListaMedia).ToList();

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

        public void Crear(Publicacion publicacion)
        {
            publicacion.FechaCreacion = DateTime.Now;
            //Esto verifica si la creo un profesional y la deja en espera de ser aprobada
            if (publicacion.ProfesionalId != null){
                publicacion.Estado = publicacion.FechaProgramada.HasValue
                    ? Enum_EstadoPublicacion.Programada
                    : Enum_EstadoPublicacion.Pendiente;
            }
            else
            {
                publicacion.Estado = Enum_EstadoPublicacion.Aprobada;
            }
                _context.Publicaciones.Add(publicacion);
            _context.SaveChanges();
        }

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

        public List<Publicacion> ObtenerPendientes()
        {
            return _context.Publicaciones
                .Include(p => p.Profesional)
                .Where(p => p.Estado == Enum_EstadoPublicacion.Pendiente).ToList();
        }
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

        public List<Publicacion> ObtenerCreadasAdmin(int adminId)
        {
            return _context.Publicaciones
                .Include(p => p.ListaMedia)
                .Where(p => p.AdminCreadorId == adminId)
                .ToList();
        }

        public List<Publicacion> ObtenerAprobadasAdmin(int adminId)
        {
            return _context.Publicaciones
                .Include(p => p.ListaMedia)
                .Include(p => p.Profesional)
                .Where(p => p.AdminAprobadorId == adminId && p.Estado == Enum_EstadoPublicacion.Aprobada)
                .ToList();
        }
        public List<Publicacion> ObtenerRechazadasAdmin(int adminId)
        {
            return _context.Publicaciones
                .Include(p => p.ListaMedia)
                .Include(p => p.Profesional)
                .Where(p => p.AdminAprobadorId == adminId && p.Estado == Enum_EstadoPublicacion.Rechazada )
                .ToList();
        }
        public void Actualizar(Publicacion entidad)
        {
            _context.Publicaciones.Update(entidad);
            _context.SaveChanges();
        }
        public bool UsuarioYaDioLike(int publicacionId, int usuarioId, string rol)
        {
            return _context.LikePublicaciones.Any(l =>
                l.PublicacionId == publicacionId &&
                l.UsuarioId == usuarioId &&
                l.TipoUsuario == rol);
        }

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
        public int ContarLikes(int publicacionId)
        {
            return _context.LikePublicaciones.Count(l => l.PublicacionId == publicacionId);
        }

    }
}
