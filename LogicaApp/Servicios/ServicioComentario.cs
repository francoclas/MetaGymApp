using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ServicioComentario : IComentarioServicio
    {
        // Repositorio de persistencia de Comentario
        private readonly IRepositorioComentario _repositorioComentario;

        // Servicios diferidos para evitar ciclos o instanciación temprana
        private readonly Lazy<INotificacionServicio> _notificacionServicio;
        private readonly Lazy<IPublicacionServicio> _publicacionServicio;

        // Constructor (inyección de dependencias)
        public ServicioComentario(
            IRepositorioComentario repo,
            Lazy<INotificacionServicio> notificacionServicio,
            Lazy<IPublicacionServicio> publicacionServicio)
        {
            _repositorioComentario = repo;
            _notificacionServicio = notificacionServicio;
            _publicacionServicio = publicacionServicio;
        }


        // Comentarios por publicación (incluye respuestas activas)

        public List<ComentarioDTO> ObtenerPorPublicacion(int publicacionId)
        {
            var comentarios = _repositorioComentario.ObtenerPorPublicacion(publicacionId);
            var result = new List<ComentarioDTO>();

            foreach (var c in comentarios)
            {
                result.Add(ConstruirDTO(c));
            }

            return result;
        }

        // Alta de comentario o respuesta + notificación

        public ComentarioDTO AgregarComentario(ComentarioDTO dto)
        {
            // Instancia base: asigno el autor según el rol informado
            Comentario nuevo = new Comentario
            {
                Contenido = dto.Contenido,
                PublicacionId = dto.PublicacionId,
                ComentarioPadreId = dto.ComentarioPadreId,
                ProfesionalId = dto.RolAutor == "Profesional" ? dto.AutorId : null,
                ClienteId = dto.RolAutor == "Cliente" ? dto.AutorId : null,
                AdminId = dto.RolAutor == "Admin" ? dto.AutorId : null
            };

            // Valido existencia de la publicación
            PublicacionDTO publicacion = _publicacionServicio.Value.ObtenerPorId(dto.PublicacionId);
            if (publicacion == null) throw new Exception("No existe publicacion.");

            // Persisto comentario (el repo setea ComentarioId)
            _repositorioComentario.Agregar(nuevo);

            // Notificaciones
            if (dto.ComentarioPadreId == null)
            {
                // Comentario directo sobre publicación => notifica al autor de la publicación
                _notificacionServicio.Value.NotificarComentario(
                    publicacion.AutorId,
                    publicacion.RolAutor,
                    publicacion.Id,
                    dto.Contenido
                );
            }
            else
            {
                // Respuesta a otro comentario => notifica al autor del comentario padre
                var padre = _repositorioComentario.ObtenerPorId((int)dto.ComentarioPadreId);

                if (padre != null)
                {
                    int? receptorId = null;
                    string rolReceptor = "";

                    // Determino receptor según quién escribió el comentario padre
                    if (padre.ClienteId.HasValue) { receptorId = padre.ClienteId; rolReceptor = "Cliente"; }
                    else if (padre.ProfesionalId.HasValue) { receptorId = padre.ProfesionalId; rolReceptor = "Profesional"; }
                    else if (padre.AdminId.HasValue) { receptorId = padre.AdminId; rolReceptor = "Admin"; }

                    // Evito auto-notificarse (mismo rol y mismo autor)
                    if (receptorId != null && !(rolReceptor == dto.RolAutor && receptorId == dto.AutorId))
                    {
                        _notificacionServicio.Value.NotificarInteraccionComentario(
                            (int)receptorId,
                            rolReceptor,
                            padre.ComentarioId,
                            "Comentaron: " + dto.Contenido
                        );
                    }
                }
            }

            // Devuelvo el DTO del comentario ya persistido
            return ConstruirDTO(ObtenerComentarioId(nuevo.ComentarioId));
        }
        // Edición de comentario (solo su autor)

        public void EditarComentario(int comentarioId, string nuevoContenido, int usuarioId, string rol)
        {
            // Validación de datos
            if (string.IsNullOrEmpty(nuevoContenido)) throw new Exception("El comentario no puede estar vacio");

            // Obtengo el comentario y verifico existencia
            Comentario comentario = _repositorioComentario.ObtenerPorId(comentarioId);
            if (comentario == null) throw new Exception("El comentario no existe o fue eliminado");

            // Seguridad: solo el autor, según rol
            switch (rol)
            {
                case "Cliente":
                    if (comentario.ClienteId != usuarioId) throw new Exception("No es el autor del comentario");
                    break;
                case "Admin":
                    if (comentario.AdminId != usuarioId) throw new Exception("No es el autor del comentario");
                    break;
                case "Profesional":
                    if (comentario.ProfesionalId != usuarioId) throw new Exception("No es el autor del comentario");
                    break;
                default:
                    throw new Exception("Tipo de usuario no valido.");
            }

            // Actualizo solo si realmente cambió
            if (!string.Equals(nuevoContenido, comentario.Contenido))
            {
                _repositorioComentario.ActualizarContenido(comentarioId, nuevoContenido);
            }
        }
        // Borrado lógico del comentario

        public void EliminarComentario(int comentarioId, int usuarioId, string rol)
        {
            // Obtengo (por si en el futuro se valida propietario/rol)
            Comentario comentario = _repositorioComentario.ObtenerPorId(comentarioId);

            // Desactivación (borrado lógico)
            _repositorioComentario.Desactivar(comentarioId);
        }

        // ----------------------------------------------------
        // Likes
        // ----------------------------------------------------
        public void DarLikeComentario(int comentarioId, int usuarioId, string rol)
        {
            _repositorioComentario.DarLike(comentarioId, usuarioId, rol);
        }

        public void QuitarLikeComentario(int comentarioId, int usuarioId, string rol)
        {
            _repositorioComentario.QuitarLike(comentarioId, usuarioId, rol);
        }

        public bool UsuarioYaDioLikeComentario(int comentarioId, int usuarioId, string rol)
        {
            return _repositorioComentario.UsuarioYaDioLike(comentarioId, usuarioId, rol);
        }

        // Construye un DTO recursivamente, filtrando respuestas inactivas
        private ComentarioDTO ConstruirDTO(Comentario c)
        {
            ComentarioDTO aux = new ComentarioDTO
            {
                PublicacionId = c.Publicacion.Id,
                ComentarioId = c.ComentarioId,
                Contenido = c.Contenido,
                FechaCreacion = c.FechaCreacion,
                FechaEdicion = c.FechaEdicion,

                // Prioridad de autor: Profesional -> Cliente -> Admin
                AutorId = c.ProfesionalId ?? c.ClienteId ?? c.AdminId ?? 0,
                AutorNombre = c.Profesional?.NombreCompleto
                              ?? c.Cliente?.NombreCompleto
                              ?? c.Admin?.NombreCompleto
                              ?? "Desconocido",
                RolAutor = c.Profesional != null ? "Profesional"
                          : c.Cliente != null ? "Cliente"
                          : "Admin",

                ComentarioPadreId = c.ComentarioPadreId,

                // Solo respuestas activas (map recursivo)
                Respuestas = c.Respuestas?
                    .Where(r => r.EstaActivo)
                    .Select(r => ConstruirDTO(r))
                    .ToList() ?? new List<ComentarioDTO>()
            };

            // Imagen del autor (foto de perfil marcada como favorita)
            if (c.Profesional != null)
            {
                aux.ImagenAutor = c.Profesional.FotosPerfil.FirstOrDefault(p => p.EsFavorito);
            }
            else if (c.Cliente != null)
            {
                aux.ImagenAutor = c.Cliente.FotosPerfil.FirstOrDefault(p => p.EsFavorito);
            }
            else
            {
                aux.ImagenAutor = c.Admin.FotosPerfil.FirstOrDefault(p => p.EsFavorito);
            }

            return aux;
        }

        // Conteo de likes
        public int ContarLikesComentario(int id)
        {
            return _repositorioComentario.ContarLikes(id);
        }

        // Obtener entidad Comentario por Id
        public Comentario ObtenerComentarioId(int ComentarioId)
        {
            return _repositorioComentario.ObtenerPorId(ComentarioId);
        }

        // Actualiza entidad Comentario completa
        public void Actualizar(Comentario comentario)
        {
            _repositorioComentario.Actualizar(comentario);
        }
    }
}
