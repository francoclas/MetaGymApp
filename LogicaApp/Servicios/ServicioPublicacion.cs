using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioPublicacion : IPublicacionServicio
    {
        private readonly IRepositorioPublicacion _repo;
        private readonly IMediaServicio mediaServicio;
        public ServicioPublicacion(IRepositorioPublicacion repo, IMediaServicio mediaServicio)
        {
            _repo = repo;
            this.mediaServicio = mediaServicio;
        }

        public List<PublicacionDTO> ObtenerPublicaciones()
        {
            var lista = _repo.ObtenerAprobadasPublicas();

            var result = new List<PublicacionDTO>();
            foreach (var pub in lista)
            {
                result.Add(ConvertirAPublicacionDTO(pub));
            }
            return result;
        }

        public PublicacionDTO ObtenerPorId(int id)
        {
            var pub = _repo.ObtenerPorId(id);
            if (pub == null) return null;

            return ConvertirAPublicacionDTO(pub);
        }

        public void CrearPublicacion(CrearPublicacionDTO dto)
        {
            var nueva = new Publicacion
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaProgramada = dto.FechaProgramada,
                EsPrivada = dto.EsPrivada,
                ProfesionalId = dto.ProfesionalId,
                ListaMedia = new List<Media>() // de momento vacía
            };

            _repo.Crear(nueva);
        }

        public void ModerarPublicacion(ModerarPublicacionDTO dto)
        {
            var nuevoEstado = dto.Aprobar ? Enum_EstadoPublicacion.Aprobada : Enum_EstadoPublicacion.Rechazada;
            _repo.ActualizarEstado(dto.PublicacionId, nuevoEstado, dto.AdminId, dto.MotivoRechazo);
        }

        public List<PublicacionDTO> ObtenerPendientes()
        {
            var lista = _repo.ObtenerPendientes();
            var result = new List<PublicacionDTO>();

            foreach (var pub in lista)
            {
                result.Add(ConvertirAPublicacionDTO(pub));
            }
            return result;
        }

        // Método privado para convertir
        private PublicacionDTO ConvertirAPublicacionDTO(Publicacion pub)
        {
            return new PublicacionDTO
            {
                Id = pub.Id,
                Titulo = pub.Titulo,
                Descripcion = pub.Descripcion,
                FechaCreacion = pub.FechaCreacion,
                FechaProgramada = pub.FechaProgramada,
                Estado = pub.Estado,
                EsPrivada = pub.EsPrivada,
                Vistas = pub.Vistas,
                CantLikes = pub.CantLikes,
                AutorId = pub.ProfesionalId ?? pub.AdminCreadorId ?? 0,
                Comentarios = pub.Comentarios?.Select(c => new ComentarioDTO
                {
                    ComentarioId = c.ComentarioId,
                    Contenido = c.Contenido,
                    FechaCreacion = c.FechaCreacion,
                    FechaEdicion = c.FechaEdicion,
                    AutorId = c.ProfesionalId ?? c.ClienteId ?? c.AdminId ?? 0,
                    AutorNombre = c.Profesional?.NombreCompleto ?? c.Cliente?.NombreCompleto ?? c.Admin?.NombreCompleto ?? "Desconocido",
                    RolAutor = c.Profesional != null ? "Profesional" : c.Cliente != null ? "Cliente" : "Admin",
                    //CantLikes = c.CantLikes,
                    ComentarioPadreId = c.ComentarioPadreId,
                    Respuestas = c.Respuestas?
                        .Where(r => r.EstaActivo)
                        .Select(r => new ComentarioDTO
                        {
                            ComentarioId = r.ComentarioId,
                            Contenido = r.Contenido,
                            FechaCreacion = r.FechaCreacion,
                            FechaEdicion = r.FechaEdicion,
                            AutorId = r.ProfesionalId ?? r.ClienteId ?? r.AdminId ?? 0,
                            AutorNombre = r.Profesional?.NombreCompleto ?? r.Cliente?.NombreCompleto ?? r.Admin?.NombreCompleto ?? "Desconocido",
                            RolAutor = r.Profesional != null ? "Profesional" : r.Cliente != null ? "Cliente" : "Admin",
                            //CantLikes = r.CantLikes,
                            ComentarioPadreId = r.ComentarioPadreId
                        }).ToList() ?? new()
                }).ToList() ?? new(),
                
                UrlsMedia = pub.ListaMedia?.Select(m => m.Url).ToList() ?? new(),
                Medias = pub.ListaMedia,
                NombreAutor = pub.Profesional?.NombreCompleto ?? pub.AdminCreador?.NombreCompleto ?? "Desconocido",
                ImagenAutorURL = pub.Profesional != null
                        ? mediaServicio.ObtenerFotoFavorita(Enum_TipoEntidad.Profesional, pub.Profesional.Id)?.Url
                        : mediaServicio.ObtenerFotoFavorita(Enum_TipoEntidad.Admin, (int)pub.AdminCreadorId)?.Url,
                RolAutor = pub.Profesional != null ? "Profesional" : "Admin",
                MotivoRechazo = pub.MotivoRechazo,
                NombreAprobador = pub.AdminAprobador?.NombreCompleto,
                NombreCreadorAdmin = pub.AdminCreador?.NombreCompleto
            };
        }

        public List<PublicacionDTO> ObtenerTodas()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            var lista = _repo.ObtenerTodas();
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }

        public List<PublicacionDTO> ObtenerPorProfesionalId(int profesionalId)
        {
            var publicaciones = _repo.ObtenerTodas()
            .Where(p => p.ProfesionalId == profesionalId)
            .ToList();

            var result = new List<PublicacionDTO>();
            foreach (var pub in publicaciones)
            {
                result.Add(ConvertirAPublicacionDTO(pub));
            }

            return result;
        }

        public void CrearPublicacionImagenes(Publicacion publicacion)
        {
            _repo.Crear(publicacion);
        }

        public List<PublicacionDTO> ObtenerCreadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repo.ObtenerCreadasAdmin(adminId);
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }

        public List<PublicacionDTO> ObtenerAutorizadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repo.ObtenerAprobadasAdmin(adminId);
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }
        public List<PublicacionDTO> ObtenerRechazadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repo.ObtenerRechazadasAdmin(adminId);
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }
        public void AprobarPublicacion(int publicacionId, int adminId)
        {
            Publicacion publicacion = _repo.ObtenerPorId(publicacionId);

            if (publicacion == null || publicacion.Estado != Enum_EstadoPublicacion.Pendiente)
                throw new Exception("La publicación no existe o ya fue revisada.");

            publicacion.Estado = Enum_EstadoPublicacion.Aprobada;
            publicacion.FechaModificacion = DateTime.Now;
            publicacion.AdminAprobadorId = adminId;

            _repo.Actualizar(publicacion);
        }

        public void RechazarPublicacion(int publicacionId, string motivoRechazo, int adminId)
        {
            Publicacion publicacion = _repo.ObtenerPorId(publicacionId);

            if (publicacion == null || publicacion.Estado != Enum_EstadoPublicacion.Pendiente)
                throw new Exception("La publicación no existe o ya fue revisada.");

            publicacion.Estado = Enum_EstadoPublicacion.Rechazada;
            publicacion.MotivoRechazo = motivoRechazo;
            publicacion.FechaModificacion = DateTime.Now;
            publicacion.AdminAprobadorId = adminId;

            _repo.Actualizar(publicacion);
        }

        public List<PublicacionDTO> ObtenerPublicacionesInicio()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> list = _repo.ObtenerAprobadasPublicas().OrderByDescending(p => p.FechaCreacion).ToList();

            foreach (var item in list)
            {
                salida.Add(ConvertirAPublicacionDTO(item)); 
            }
            return salida;
        }
        public void DarLikePublicacion(int publicacionId, int usuarioId, string rol)
        {
            _repo.DarLike(publicacionId, usuarioId, rol);
        }

        public void QuitarLikePublicacion(int publicacionId, int usuarioId, string rol)
        {
            _repo.QuitarLike(publicacionId, usuarioId, rol);
        }

        public bool UsuarioYaDioLikePublicacion(int publicacionId, int usuarioId, string rol)
        {
            return _repo.UsuarioYaDioLike(publicacionId, usuarioId, rol);
        }

    }
}
