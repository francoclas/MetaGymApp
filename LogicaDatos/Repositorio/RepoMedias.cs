using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;

namespace LogicaDatos.Repositorio
{
    public class RepoMedias : IRepositorioMedia
    {
        private readonly DbContextApp _context;

        public RepoMedias(DbContextApp contextApp)
        {
            _context = contextApp;
        }


        public void Agregar(Media media)
        {
            _context.Medias.Add(media);
            _context.SaveChanges();
        }

        public void Eliminar(Media media)
        {
            _context.Medias.Remove(media);
            _context.SaveChanges();

        }

        public Media ObtenerPorId(int mediaId)
        {
            return _context.Medias.Find(mediaId);
        }

        public Media ObtenerPorEntidad(Enum_TipoEntidad tipoEntidad, int idEntidad)
        {
            return tipoEntidad switch
            {
                Enum_TipoEntidad.Cliente => _context.Medias.FirstOrDefault(m => m.ClienteId == idEntidad),
                Enum_TipoEntidad.Profesional => _context.Medias.FirstOrDefault(m => m.ProfesionalId == idEntidad),
                Enum_TipoEntidad.Admin => _context.Medias.FirstOrDefault(m => m.AdminId == idEntidad),
                Enum_TipoEntidad.Ejercicio => _context.Medias.FirstOrDefault(m => m.EjercicioId == idEntidad),
                Enum_TipoEntidad.Publicacion => _context.Medias.FirstOrDefault(m => m.PublicacionId == idEntidad),
                Enum_TipoEntidad.Establecimiento => _context.Medias.FirstOrDefault(m => m.EstablecimientoId == idEntidad),
                _ => null
            };
        }

        public void Guardar()
        {
            _context.SaveChanges(); 
        }
    }
}
