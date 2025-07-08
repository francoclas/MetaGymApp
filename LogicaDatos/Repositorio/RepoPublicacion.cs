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
            publicacion.Estado = publicacion.FechaProgramada.HasValue
                ? Enum_EstadoPublicacion.Programada
                : Enum_EstadoPublicacion.Pendiente;

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

        
    }
}
