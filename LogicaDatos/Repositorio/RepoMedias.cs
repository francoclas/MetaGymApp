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

        // Inyección de DbContext
        public RepoMedias(DbContextApp contextApp)
        {
            _context = contextApp;
        }

        // =======================
        // Operaciones básicas
        // =======================

        // Alta de un registro Media
        public void Agregar(Media media)
        {
            _context.Medias.Add(media);
            _context.SaveChanges();
        }

        // Eliminación directa de un registro Media
        public void Eliminar(Media media)
        {
            _context.Medias.Remove(media);
            _context.SaveChanges();
        }

        // Búsqueda por Id
        public Media ObtenerPorId(int mediaId)
        {
            return _context.Medias.Find(mediaId);
        }

        // Trae la primera media asociada a una entidad concreta, según su tipo
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
                _ => null // si se agrega un nuevo Enum_TipoEntidad y no se contempla acá, devuelve null
            };
        }

        // Guardar cambios sueltos
        public void Guardar()
        {
            _context.SaveChanges();
        }

        // =======================
        // Favoritas / Perfil
        // =======================

        // Devuelve la media marcada como favorita para una entidad (imagen de perfil, por ejemplo)
        public Media? ObtenerFavorita(Enum_TipoEntidad tipo, int idEntidad)
        {
            return tipo switch
            {
                Enum_TipoEntidad.Cliente => _context.Medias.FirstOrDefault(m => m.EsFavorito && m.ClienteId == idEntidad),
                Enum_TipoEntidad.Profesional => _context.Medias.FirstOrDefault(m => m.EsFavorito && m.ProfesionalId == idEntidad),
                Enum_TipoEntidad.Admin => _context.Medias.FirstOrDefault(m => m.EsFavorito && m.AdminId == idEntidad)
            };
        }

        // Marca una media como favorita dentro del conjunto de medias de esa entidad (desmarca el resto)
        public void AsignarFotoFavorita(int mediaId, Enum_TipoEntidad tipo, int entidadId)
        {
            // Busco todas las medias asociadas a la entidad según el tipo
            var medias = _context.Medias
                   .Where(m =>
                       (tipo == Enum_TipoEntidad.Cliente && m.ClienteId == entidadId) ||
                       (tipo == Enum_TipoEntidad.Profesional && m.ProfesionalId == entidadId) ||
                       (tipo == Enum_TipoEntidad.Admin && m.AdminId == entidadId)
                   ).ToList();

            if (!medias.Any())
                throw new Exception("No se encontraron archivos multimedia para esta entidad.");

            // Verifico que la seleccionada pertenezca a ese conjunto
            var seleccionada = medias.FirstOrDefault(m => m.Id == mediaId);
            if (seleccionada == null)
                throw new Exception("La media especificada no pertenece a la entidad indicada.");

            // Seteo única favorita
            foreach (var media in medias)
                media.EsFavorito = (media.Id == mediaId);

            _context.SaveChanges();
        }

        // =======================
        // Listados útiles
        // =======================

        // Todas las imágenes (Tipo == Imagen) de una entidad según su tipo
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

        // Imagen de perfil (favorita) de una entidad
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

        // Trae todas las medias asociadas a una entidad concreta (sin filtrar por tipo de media)
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