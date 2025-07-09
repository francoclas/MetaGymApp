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

        public Media? ObtenerFavorita(Enum_TipoEntidad tipo, int idEntidad)
        {
            return tipo switch
            {
                Enum_TipoEntidad.Cliente => _context.Medias.FirstOrDefault(m => m.EsFavorito && m.ClienteId == idEntidad),
                Enum_TipoEntidad.Profesional => _context.Medias.FirstOrDefault(m => m.EsFavorito && m.ProfesionalId == idEntidad),
                Enum_TipoEntidad.Admin => _context.Medias.FirstOrDefault(m => m.EsFavorito && m.AdminId == idEntidad),

            };

        }

        public void AsignarFotoFavorita(int mediaId, Enum_TipoEntidad tipo, int entidadId)
        {
            var medias = _context.Medias
                   .Where(m =>
                       (tipo == Enum_TipoEntidad.Cliente && m.ClienteId == entidadId) ||
                       (tipo == Enum_TipoEntidad.Profesional && m.ProfesionalId == entidadId) ||
                       (tipo == Enum_TipoEntidad.Admin && m.AdminId == entidadId)
                   ).ToList();

            if (!medias.Any())
                throw new Exception("No se encontraron archivos multimedia para esta entidad.");

            var seleccionada = medias.FirstOrDefault(m => m.Id == mediaId);

            if (seleccionada == null)
                throw new Exception("La media especificada no pertenece a la entidad indicada.");

            foreach (var media in medias)
                media.EsFavorito = (media.Id == mediaId);

            _context.SaveChanges();
        }
        public List<Media> ObtenerImagenesUsuario(Enum_TipoEntidad tipo, int idEntidad)
        {
            return _context.Medias
                .Where(m =>
                    m.Tipo == Enum_TipoMedia.Imagen &&
                    m.TipoEntidad == tipo &&
                    (
                        (tipo == Enum_TipoEntidad.Cliente && m.ClienteId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Profesional && m.ProfesionalId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Admin && m.AdminId == idEntidad)
                    )
                )
                .ToList();
        }
        public Media? ObtenerImagenPerfil(Enum_TipoEntidad tipo, int idEntidad)
        {
            return _context.Medias
                .FirstOrDefault(m =>
                    m.Tipo == Enum_TipoMedia.Imagen &&
                    m.TipoEntidad == tipo &&
                    m.EsFavorito &&
                    (
                        (tipo == Enum_TipoEntidad.Cliente && m.ClienteId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Profesional && m.ProfesionalId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Admin && m.AdminId == idEntidad)
                    )
                );
        }
        public List<Media> ObtenerPorEntidadGeneral(Enum_TipoEntidad tipo, int idEntidad)
        {
            return _context.Medias
                .Where(m =>
                    m.TipoEntidad == tipo &&
                    (
                        (tipo == Enum_TipoEntidad.Cliente && m.ClienteId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Profesional && m.ProfesionalId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Admin && m.AdminId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Ejercicio && m.EjercicioId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Publicacion && m.PublicacionId == idEntidad) ||
                        (tipo == Enum_TipoEntidad.Establecimiento && m.EstablecimientoId == idEntidad)
                    )
                )
                .ToList();
        }



    }
}
