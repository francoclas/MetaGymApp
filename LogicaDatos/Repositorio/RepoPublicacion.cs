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
            return _context.Publicaciones.Include(p => p.Comentarios).Include(p => p.ListaMedia).FirstOrDefault(p => p.Id == id);
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
            pub.AdminId = adminId;
            pub.MotivoRechazo = motivoRechazo;
            pub.FechaModificacion = DateTime.Now;

            _context.SaveChanges();
        }

        public List<Publicacion> ObtenerPendientes()
        {
            return _context.Publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Pendiente).ToList();
        }
        public List<Publicacion> ObtenerAprobadasPublicas()
        { 
            return _context.Publicaciones.Where(p => p.Estado == Enum_EstadoPublicacion.Aprobada && !p.EsPrivada)
                                     .OrderByDescending(p => p.FechaCreacion)
                                     .ToList();
        }
    }
}
