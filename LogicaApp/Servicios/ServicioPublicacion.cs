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

        // Inyección de dependencias
        public ServicioPublicacion(
            IRepositorioPublicacion repo,
            IMediaServicio mediaServicio,
            IComentarioServicio comentarioServicio,
            INotificacionServicio notificacion)
        {
            _repositorioPublicacion = repo;
            _mediaServicio = mediaServicio;
            _comentarioServicio = comentarioServicio;
            _notificacionServicio = notificacion;
        }

        // Listados / obtenciones
        // Publicaciones aprobadas y públicas
        public List<PublicacionDTO> ObtenerPublicaciones()
        {
            var lista = _repositorioPublicacion.ObtenerAprobadasPublicas();
            var result = new List<PublicacionDTO>();

            foreach (var pub in lista)
                result.Add(ConvertirAPublicacionDTO(pub));

            return result;
        }

        // Una publicación específica por Id
        public PublicacionDTO ObtenerPorId(int id)
        {
            var pub = _repositorioPublicacion.ObtenerPorId(id);
            if (pub == null) return null;

            return ConvertirAPublicacionDTO(pub);
        }

        // Altas

        // Alta de publicación desde DTO (autor profesional)
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

        // Alta directa de Admin
        public void CrearPublicacionAdmin(Publicacion publicacion)
        {
            _repositorioPublicacion.Crear(publicacion);
        }

        // Alta genérica con imágenes 
        public void CrearPublicacionImagenes(Publicacion publicacion)
        {
            _repositorioPublicacion.Crear(publicacion);
        }

        // Moderación

        public void ModerarPublicacion(ModerarPublicacionDTO dto)
        {
            var nuevoEstado = dto.Aprobar ? Enum_EstadoPublicacion.Aprobada : Enum_EstadoPublicacion.Rechazada;
            _repositorioPublicacion.ActualizarEstado(dto.PublicacionId, nuevoEstado, dto.AdminId, dto.MotivoRechazo);
        }

        public void AprobarPublicacion(int publicacionId, int adminId)
        {
            // Obtengo la publicación y valido estado
            Publicacion publicacion = _repositorioPublicacion.ObtenerPorId(publicacionId);
            if (publicacion == null || publicacion.Estado != Enum_EstadoPublicacion.Pendiente)
                throw new Exception("La publicación no existe o ya fue revisada.");

            // Actualizo campos de aprobación
            publicacion.Estado = Enum_EstadoPublicacion.Aprobada;
            publicacion.FechaModificacion = DateTime.Now;
            publicacion.AdminAprobadorId = adminId;
            _repositorioPublicacion.Actualizar(publicacion);

            // Notifico al profesional autor (se mantiene la lógica exacta)
            _notificacionServicio.NotificacionPersonalizada((int)publicacion.ProfesionalId, "Profesional",
                new Notificacion
                {
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

            // Actualizo estado y motivo
            publicacion.Estado = Enum_EstadoPublicacion.Rechazada;
            publicacion.MotivoRechazo = motivoRechazo;
            publicacion.FechaModificacion = DateTime.Now;
            publicacion.AdminAprobadorId = adminId;
            _repositorioPublicacion.Actualizar(publicacion);

            // Notificación de rechazo
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

        // Listados para paneles

        public List<PublicacionDTO> ObtenerPendientes()
        {
            var lista = _repositorioPublicacion.ObtenerPendientes();
            var result = new List<PublicacionDTO>();

            foreach (var pub in lista)
                result.Add(ConvertirAPublicacionDTO(pub));

            return result;
        }

        public List<PublicacionDTO> ObtenerTodas()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            var lista = _repositorioPublicacion.ObtenerTodas();

            foreach (var o in lista)
                salida.Add(ConvertirAPublicacionDTO(o));

            return salida;
        }

        public List<PublicacionDTO> ObtenerPorProfesionalId(int profesionalId)
        {
            // Filtra en memoria las publicaciones del profesional
            var publicaciones = _repositorioPublicacion.ObtenerTodas()
                .Where(p => p.ProfesionalId == profesionalId)
                .ToList();

            var result = new List<PublicacionDTO>();
            foreach (var pub in publicaciones)
                result.Add(ConvertirAPublicacionDTO(pub));

            return result;
        }

        public List<PublicacionDTO> ObtenerCreadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerCreadasAdmin(adminId);

            foreach (var o in lista)
                salida.Add(ConvertirAPublicacionDTO(o));

            return salida;
        }

        public List<PublicacionDTO> ObtenerAutorizadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerAprobadasAdmin(adminId);

            foreach (var o in lista)
                salida.Add(ConvertirAPublicacionDTO(o));

            return salida;
        }

        public List<PublicacionDTO> ObtenerRechazadasPorAdmin(int adminId)
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerRechazadasAdmin(adminId);

            foreach (var o in lista)
                salida.Add(ConvertirAPublicacionDTO(o));

            return salida;
        }

        // Novedades (mapea con DTO "noticia" sin ids ni conteo de likes)
        public List<PublicacionDTO> ObtenerNovedades()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> lista = _repositorioPublicacion.ObtenerNovedades();

            foreach (Publicacion item in lista)
                salida.Add(ConvertirNoticia(item));

            return salida;
        }

        // Inicio (todas las aprobadas públicas ordenadas por fecha de creación)
        public List<PublicacionDTO> ObtenerPublicacionesInicio()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> list = _repositorioPublicacion.ObtenerAprobadasPublicas()
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();

            foreach (var item in list)
                salida.Add(ConvertirAPublicacionDTO(item));

            return salida;
        }

        // Inicio API (igual que Inicio pero filtrando comentarios inactivos en raíz)
        public List<PublicacionDTO> ObtenerPublicacionesInicioAPI()
        {
            List<PublicacionDTO> salida = new List<PublicacionDTO>();
            List<Publicacion> list = _repositorioPublicacion.ObtenerAprobadasPublicas()
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();

            foreach (var item in list)
                salida.Add(ConvertirAPublicacionDTOFiltroComentario(item));

            return salida;
        }

        // Likes

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

        // =======================
        // Actualizaciones
        // =======================

        public void ActualizarPublicacion(PublicacionDTO pubActualizada)
        {
            // Carga original para mantener relaciones e Ids
            Publicacion original = _repositorioPublicacion.ObtenerPorId(pubActualizada.Id);
            if (original == null) throw new Exception("La publicación no existe.");

            // Campos editables
            original.Titulo = pubActualizada.Titulo;
            original.Descripcion = pubActualizada.Descripcion;
            original.EsPrivada = pubActualizada.EsPrivada;
            original.MostrarEnNoticiasPublicas = pubActualizada.MostrarEnNoticiasPublicas;
            original.FechaModificacion = DateTime.Now;
            original.Estado = pubActualizada.Estado;

            _repositorioPublicacion.Actualizar(original);
        }

        // =======================
        // Conversores / mapeos a DTO
        // =======================

        // DTO completo (incluye comentarios raíz activos / inactivos)
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

                // Foto favorita del autor (si no hay, MapearComentario maneja default para comentarios;
                // aquí, si no existe, queda null y lo resuelve la vista)
                ImagenAutorURL = _mediaServicio.ObtenerFotoFavorita(rolAutor, autorId)?.Url,

                // Medias y URLs
                UrlsMedia = pub.ListaMedia?.Select(m => m.Url).ToList() ?? new(),
                Medias = pub.ListaMedia,

                // Conteo de likes
                CantLikes = _repositorioPublicacion.ContarLikes(pub.Id),

                // Comentarios de primer nivel (sin filtrar por estado)
                Comentarios = pub.Comentarios?
                    .Where(c => c.ComentarioPadreId == null)
                    .Select(c => MapearComentario(c))
                    .ToList() ?? new(),

                // Moderación
                MotivoRechazo = pub.MotivoRechazo,
                NombreAprobador = pub.AdminAprobador?.NombreCompleto,
                NombreCreadorAdmin = pub.AdminCreador?.NombreCompleto
            };
        }

        // Igual que el anterior pero filtra comentarios raíz inactivos
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

        // Mapeo noticia para novedades (sin ids/likes)
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

        // =======================
        // Comentarios (map recursivo)
        // =======================

        private ComentarioDTO MapearComentario(Comentario c)
        {
            // Determina el tipo/rol del autor del comentario
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
                AutorNombre = c.Profesional?.NombreCompleto
                              ?? c.Cliente?.NombreCompleto
                              ?? c.Admin?.NombreCompleto
                              ?? "Desconocido",
                RolAutor = tipo.ToString(),

                // Imagen de autor (foto favorita según rol). Si no hay, se setea default más abajo.
                ImagenAutor = _mediaServicio.ObtenerFotoFavorita(tipo, autorId),

                ComentarioPadreId = c.ComentarioPadreId,

                // Likes del comentario (usa servicio de comentarios)
                CantLikes = _comentarioServicio.ContarLikesComentario(c.ComentarioId),

                // Respuestas activas (map recursivo)
                Respuestas = c.Respuestas?
                    .Where(r => r.EstaActivo)
                    .Select(r => MapearComentario(r))
                    .ToList() ?? new()
            };

            // Fallback de imagen si no existe favorita
            if (salida.ImagenAutor == null)
            {
                salida.ImagenAutor = new Media { Url = "/MediaWeb/Default/perfil_default.jpg" };
            }

            return salida;
        }

        // =======================
        // Moderación de comentarios - Solo admin
        // =======================

        public void OcultarComentario(int comentarioId)
        {
            // Obtengo comentario y alterno su estado activo/inactivo
            Comentario comentario = _comentarioServicio.ObtenerComentarioId(comentarioId);

            if (comentario.EstaActivo)
                comentario.EstaActivo = false;
            else
                comentario.EstaActivo = true;

            _comentarioServicio.Actualizar(comentario);
        }
    }
}
