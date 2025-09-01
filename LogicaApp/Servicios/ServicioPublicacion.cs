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
        private readonly IRepositorioPublicacion _repositorioPublicacion;
        private readonly IMediaServicio _mediaServicio;
        private readonly IComentarioServicio _comentarioServicio;
        private readonly INotificacionServicio _notificacionServicio;
        public ServicioPublicacion(IRepositorioPublicacion repo, IMediaServicio mediaServicio, IComentarioServicio comentarioServicio,INotificacionServicio notificacion)
        {
            _repositorioPublicacion = repo;
            this._mediaServicio = mediaServicio;
            this._comentarioServicio = comentarioServicio;
            _notificacionServicio = notificacion;    
        }

        public List<PublicacionDTO> ObtenerPublicaciones()
        {
            var lista = _repositorioPublicacion.ObtenerAprobadasPublicas();

            var result = new List<PublicacionDTO>();
            foreach (var pub in lista)
            {
                result.Add(ConvertirAPublicacionDTO(pub));
            }
            return result;
        }

        public PublicacionDTO ObtenerPorId(int id)
        {
            var pub = _repositorioPublicacion.ObtenerPorId(id);
            if (pub == null) return null;

            return ConvertirAPublicacionDTO(pub);
        }

        public void CrearPublicacion(CrearPublicacionDTO dto)
        {
            Publicacion nueva = new Publicacion
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaProgramada = dto.FechaProgramada,
                EsPrivada = dto.EsPrivada,
                ProfesionalId = dto.ProfesionalId,
                ListaMedia = new List<Media>() // de momento vacía
            };

            _repositorioPublicacion.Crear(nueva);
        }
        public void CrearPublicacionAdmin(Publicacion publicacion)
        {
            _repositorioPublicacion.Crear(publicacion);
        }
        public void ModerarPublicacion(ModerarPublicacionDTO dto)
        {
            var nuevoEstado = dto.Aprobar ? Enum_EstadoPublicacion.Aprobada : Enum_EstadoPublicacion.Rechazada;
            _repositorioPublicacion.ActualizarEstado(dto.PublicacionId, nuevoEstado, dto.AdminId, dto.MotivoRechazo);
        }

        public List<PublicacionDTO> ObtenerPendientes()
        {
            var lista = _repositorioPublicacion.ObtenerPendientes();
            var result = new List<PublicacionDTO>();

            foreach (var pub in lista)
            {
                result.Add(ConvertirAPublicacionDTO(pub));
            }
            return result;
        }
        public List<PublicacionDTO> ObtenerTodas()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            var lista = _repositorioPublicacion.ObtenerTodas();
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }

        public List<PublicacionDTO> ObtenerPorProfesionalId(int profesionalId)
        {
            var publicaciones = _repositorioPublicacion.ObtenerTodas()
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
            _repositorioPublicacion.Crear(publicacion);
        }

        public List<PublicacionDTO> ObtenerCreadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerCreadasAdmin(adminId);
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }
        public List<PublicacionDTO> ObtenerNovedades()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerNovedades();
            foreach (Publicacion item in lista)
            {
                salida.Add(ConvertirNoticia(item));
            }
            return salida;

        }
        public List<PublicacionDTO> ObtenerAutorizadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerAprobadasAdmin(adminId);
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }
        public List<PublicacionDTO> ObtenerRechazadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerRechazadasAdmin(adminId);
            foreach (var o in lista)
            {
                salida.Add(ConvertirAPublicacionDTO(o));
            }
            return salida;
        }
        public void ActualizarPublicacion(PublicacionDTO pubActualizada)
        {
            Publicacion original = _repositorioPublicacion.ObtenerPorId(pubActualizada.Id);
            if (original == null) throw new Exception("La publicación no existe.");

            original.Titulo = pubActualizada.Titulo;
            original.Descripcion = pubActualizada.Descripcion;
            original.EsPrivada = pubActualizada.EsPrivada;
            original.MostrarEnNoticiasPublicas = pubActualizada.MostrarEnNoticiasPublicas;
            original.FechaModificacion = DateTime.Now;
            original.Estado = pubActualizada.Estado;
            _repositorioPublicacion.Actualizar(original);
        }

        public void AprobarPublicacion(int publicacionId, int adminId)
        {
            Publicacion publicacion = _repositorioPublicacion.ObtenerPorId(publicacionId);

            if (publicacion == null || publicacion.Estado != Enum_EstadoPublicacion.Pendiente)
                throw new Exception("La publicación no existe o ya fue revisada.");

            publicacion.Estado = Enum_EstadoPublicacion.Aprobada;
            publicacion.FechaModificacion = DateTime.Now;
            publicacion.AdminAprobadorId = adminId;
            _repositorioPublicacion.Actualizar(publicacion);
            _notificacionServicio.NotificacionPersonalizada((int)publicacion.ProfesionalId, "Profesional", 
                new Notificacion {
                    ProfesionalId = (int)publicacion.ProfesionalId,
                    Titulo = "Se aprobo tu publicacion!",
                    Mensaje = "Felicidades, se acepto tu publicacion para el portal.",
                    PublicacionId = publicacion.Id,
                    Tipo = Enum_TipoNotificacion.Publicacion,
                    Fecha = DateTime.Now,
                    Leido = false
                });
        }

        public void RechazarPublicacion(int publicacionId, string motivoRechazo, int adminId)
        {
            Publicacion publicacion = _repositorioPublicacion.ObtenerPorId(publicacionId);

            if (publicacion == null || publicacion.Estado != Enum_EstadoPublicacion.Pendiente)
                throw new Exception("La publicación no existe o ya fue revisada.");

            publicacion.Estado = Enum_EstadoPublicacion.Rechazada;
            publicacion.MotivoRechazo = motivoRechazo;
            publicacion.FechaModificacion = DateTime.Now;
            publicacion.AdminAprobadorId = adminId;

            _repositorioPublicacion.Actualizar(publicacion);
            _notificacionServicio.NotificacionPersonalizada((int)publicacion.ProfesionalId, "Profesional",
                new Notificacion
                {
                    ProfesionalId = (int)publicacion.ProfesionalId,
                    Titulo = "Se rechazo tu publicacion!",
                    Mensaje = "Desgraciadamente no se acepto tu solicitud de publicacion. Accede para ver mas detalles.",
                    PublicacionId = publicacion.Id,
                    Tipo = Enum_TipoNotificacion.Publicacion,
                    Fecha = DateTime.Now,
                    Leido = false
                });
        }

        public List<PublicacionDTO> ObtenerPublicacionesInicio()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> list = _repositorioPublicacion.ObtenerAprobadasPublicas().OrderByDescending(p => p.FechaCreacion).ToList();

            foreach (var item in list)
            {
                salida.Add(ConvertirAPublicacionDTO(item)); 
            }
            return salida;
        }
        public List<PublicacionDTO> ObtenerPublicacionesInicioAPI()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> list = _repositorioPublicacion.ObtenerAprobadasPublicas().OrderByDescending(p => p.FechaCreacion).ToList();

            foreach (var item in list)
            {
                salida.Add(ConvertirAPublicacionDTOFiltroComentario(item));
            }
            return salida;
        }
        public void DarLikePublicacion(int publicacionId, int usuarioId, string rol)
        {
            _repositorioPublicacion.DarLike(publicacionId, usuarioId, rol);
        }

        public void QuitarLikePublicacion(int publicacionId, int usuarioId, string rol)
        {
            _repositorioPublicacion.QuitarLike(publicacionId, usuarioId, rol);
        }

        public bool UsuarioYaDioLikePublicacion(int publicacionId, int usuarioId, string rol)
        {
            return _repositorioPublicacion.UsuarioYaDioLike(publicacionId, usuarioId, rol);
        }
        public int ContarLikesPublicacion(int id)
        {
            return _repositorioPublicacion.ContarLikes(id);
        }

        // Método privado para convertir
        private PublicacionDTO ConvertirAPublicacionDTO(Publicacion pub)
        {
            var autorId = pub.ProfesionalId ?? pub.AdminCreadorId ?? 0;
            var rolAutor = pub.Profesional != null ? Enum_TipoEntidad.Profesional : Enum_TipoEntidad.Admin;

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
                AutorId = autorId,
                RolAutor = rolAutor.ToString(),
                NombreAutor = pub.Profesional?.NombreCompleto ?? pub.AdminCreador?.NombreCompleto ?? "Desconocido",
                ImagenAutorURL = _mediaServicio.ObtenerFotoFavorita(rolAutor, autorId)?.Url,
                UrlsMedia = pub.ListaMedia?.Select(m => m.Url).ToList() ?? new(),
                Medias = pub.ListaMedia,
                CantLikes = _repositorioPublicacion.ContarLikes(pub.Id),
                Comentarios = pub.Comentarios?
                    .Where(c => c.ComentarioPadreId == null)
                    .Select(c => MapearComentario(c))
                    .ToList() ?? new(),
                MotivoRechazo = pub.MotivoRechazo,
                NombreAprobador = pub.AdminAprobador?.NombreCompleto,
                NombreCreadorAdmin = pub.AdminCreador?.NombreCompleto
            };
        }
        private PublicacionDTO ConvertirAPublicacionDTOFiltroComentario(Publicacion pub)
        {
            var autorId = pub.ProfesionalId ?? pub.AdminCreadorId ?? 0;
            var rolAutor = pub.Profesional != null ? Enum_TipoEntidad.Profesional : Enum_TipoEntidad.Admin;

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
                AutorId = autorId,
                RolAutor = rolAutor.ToString(),
                NombreAutor = pub.Profesional?.NombreCompleto ?? pub.AdminCreador?.NombreCompleto ?? "Desconocido",
                ImagenAutorURL = _mediaServicio.ObtenerFotoFavorita(rolAutor, autorId)?.Url,
                UrlsMedia = pub.ListaMedia?.Select(m => m.Url).ToList() ?? new(),
                Medias = pub.ListaMedia,
                CantLikes = _repositorioPublicacion.ContarLikes(pub.Id),
                Comentarios = pub.Comentarios?
                    .Where(c => c.ComentarioPadreId == null && c.EstaActivo)
                    .Select(c => MapearComentario(c))
                    .ToList() ?? new(),
                MotivoRechazo = pub.MotivoRechazo,
                NombreAprobador = pub.AdminAprobador?.NombreCompleto,
                NombreCreadorAdmin = pub.AdminCreador?.NombreCompleto
            };
        }
        private PublicacionDTO ConvertirNoticia(Publicacion pub)
        {
            var autorId = pub.ProfesionalId ?? pub.AdminCreadorId ?? 0;
            var rolAutor = pub.Profesional != null ? Enum_TipoEntidad.Profesional : Enum_TipoEntidad.Admin;

            return new PublicacionDTO
            {
                Titulo = pub.Titulo,
                Descripcion = pub.Descripcion,
                FechaCreacion = pub.FechaCreacion,
                FechaProgramada = pub.FechaProgramada,
                Estado = pub.Estado,
                EsPrivada = pub.EsPrivada,
                Vistas = pub.Vistas,
                NombreAutor = pub.Profesional?.NombreCompleto ?? pub.AdminCreador?.NombreCompleto ?? "Desconocido",
                ImagenAutorURL = _mediaServicio.ObtenerFotoFavorita(rolAutor, autorId)?.Url,
                UrlsMedia = pub.ListaMedia?.Select(m => m.Url).ToList() ?? new(),
                Medias = pub.ListaMedia
            };
            }
        private ComentarioDTO MapearComentario(Comentario c)
        {
            var tipo = c.Profesional != null ? Enum_TipoEntidad.Profesional :
                       c.Cliente != null ? Enum_TipoEntidad.Cliente :
                       Enum_TipoEntidad.Admin;

            var autorId = c.ProfesionalId ?? c.ClienteId ?? c.AdminId ?? 0;

            ComentarioDTO salida = new ComentarioDTO
            {
                ComentarioId = c.ComentarioId,
                Contenido = c.Contenido,
                FechaCreacion = c.FechaCreacion,
                FechaEdicion = c.FechaEdicion,
                EstaActivo = c.EstaActivo,
                AutorId = autorId,
                AutorNombre = c.Profesional?.NombreCompleto ?? c.Cliente?.NombreCompleto ?? c.Admin?.NombreCompleto ?? "Desconocido",
                RolAutor = tipo.ToString(),
                ImagenAutor = _mediaServicio.ObtenerFotoFavorita(tipo, autorId),
                ComentarioPadreId = c.ComentarioPadreId,
                CantLikes = _comentarioServicio.ContarLikesComentario(c.ComentarioId),
                Respuestas = c.Respuestas?
                    .Where(r => r.EstaActivo)
                    .Select(r => MapearComentario(r))
                    .ToList() ?? new()
            };
            if (salida.ImagenAutor == null)
            {
                salida.ImagenAutor = new Media { Url = "/MediaWeb/Default/perfil_default.jpg" };
            }
            return salida;
        }

        public void OcultarComentario(int comentarioId)
        {
            //obtengo comentario
            Comentario comentario = _comentarioServicio.ObtenerComentarioId(comentarioId);
            if (comentario.EstaActivo)
            {
                comentario.EstaActivo = false;
            }
            else
            {
                comentario.EstaActivo = true;
            }
            _comentarioServicio.Actualizar(comentario);
        }

       
    }
}
