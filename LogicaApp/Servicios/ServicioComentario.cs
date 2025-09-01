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
    public class ServicioComentario: IComentarioServicio
    {
        private readonly IRepositorioComentario _repositorioComentario;
        private readonly Lazy<INotificacionServicio> _notificacionServicio;
        private readonly Lazy<IPublicacionServicio> _publicacionServicio;
        public ServicioComentario(IRepositorioComentario repo, Lazy<INotificacionServicio> notificacionServicio, Lazy<IPublicacionServicio> publicacionServicio)
        {
            _repositorioComentario = repo;
            this._notificacionServicio = notificacionServicio;
            this._publicacionServicio = publicacionServicio;
        }

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

        public ComentarioDTO AgregarComentario(ComentarioDTO dto)
        {
            //Instancio nuevo comentario
            Comentario nuevo = new Comentario
            {
                Contenido = dto.Contenido,
                PublicacionId = dto.PublicacionId,
                ComentarioPadreId = dto.ComentarioPadreId,
                //Asigno rol para manejo dentro de bd y luego busqueda
                ProfesionalId = dto.RolAutor == "Profesional" ? dto.AutorId : null,
                ClienteId = dto.RolAutor == "Cliente" ? dto.AutorId : null,
                AdminId = dto.RolAutor == "Admin" ? dto.AutorId : null
            };
            //valido que exista publi
            PublicacionDTO publicacion = _publicacionServicio.Value.ObtenerPorId(dto.PublicacionId);
            if(publicacion == null)
            {
                throw new Exception("No existe publicacion.");
            }
            //Lo mando al repo
            _repositorioComentario.Agregar(nuevo);

            // Notificaciones
            if (dto.ComentarioPadreId == null)
            {
                // Comentario sobre una publicación se notifica al autor de la publicación

                if (publicacion != null)
                {
                    _notificacionServicio.Value.NotificarComentario(publicacion.AutorId, publicacion.RolAutor, publicacion.Id, dto.Contenido);
                }
            }
            else
            {
                // Respuesta a otro comentario para notificar al autor del comentario padre
                var padre = _repositorioComentario.ObtenerPorId((int)dto.ComentarioPadreId);

                if (padre != null)
                {
                    int? receptorId = null;
                    string rolReceptor = "";

                    //obtengo el id y el rol
                    if (padre.ClienteId.HasValue)
                    {
                        receptorId = padre.ClienteId;
                        rolReceptor = "Cliente";
                    }
                    else if (padre.ProfesionalId.HasValue)
                    {
                        receptorId = padre.ProfesionalId;
                        rolReceptor = "Profesional";
                    }
                    else if (padre.AdminId.HasValue)
                    {
                        receptorId = padre.AdminId;
                        rolReceptor = "Admin";
                    }
                    //genero notificacion para el autor del comentario padre
                    if (receptorId != null && !(rolReceptor == dto.RolAutor && receptorId == dto.AutorId))
                    {
                        _notificacionServicio.Value.NotificarInteraccionComentario((int)receptorId, rolReceptor, padre.ComentarioId,"Comentaron: " + dto.Contenido);
                            };
                }
            }
            return ConstruirDTO(ObtenerComentarioId(nuevo.ComentarioId));
            }
        //Se utiliza unicamente en webapi
        public void EditarComentario(int comentarioId, string nuevoContenido, int usuarioId, string rol)
        {
            //valido datos
            if (String.IsNullOrEmpty(nuevoContenido)) throw new Exception("El comentario no puede estar vacio");
            //obtengo comentario
            Comentario comentario = _repositorioComentario.ObtenerPorId(comentarioId);
            //verifico que exista
            if (comentario == null) throw new Exception("El comentario no existe o fue eliminado");
            //valido que sea el autor
            switch (rol)
            {
                case "Cliente":
                    if (comentario.ClienteId != usuarioId)
                        throw new Exception("No es el autor del comentario");
                    break;
                case "Admin":
                    if (comentario.AdminId != usuarioId)
                        throw new Exception("No es el autor del comentario");
                    break ;
                case "Profesional":
                    if (comentario.ProfesionalId != usuarioId)
                        throw new Exception("No es el autor del comentario");
                    break;
                default:
                    throw new Exception("Tipo de usuario no valido.");
            }
            //valido si son diferentes lo actualizo
            if (!String.Equals(nuevoContenido, comentario.Contenido)) {
                _repositorioComentario.ActualizarContenido(comentarioId, nuevoContenido);
            }
        }

        public void EliminarComentario(int comentarioId, int usuarioId, string rol)
        {
            //este metodo es solo para que se eliminen los comentarios de parte de su autor
            //obtengo comentario
            Comentario comentario = _repositorioComentario.ObtenerPorId(comentarioId);
            _repositorioComentario.Desactivar(comentarioId);
        }
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


        private ComentarioDTO ConstruirDTO(Comentario c)
        {
            ComentarioDTO aux = new ComentarioDTO
            {
                PublicacionId = c.Publicacion.Id,
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
                    .Select(r => ConstruirDTO(r))
                    .ToList() ?? new List<ComentarioDTO>()
            };

            if(c.Profesional != null)
            {
                aux.ImagenAutor = c.Profesional.FotosPerfil.FirstOrDefault(p => p.EsFavorito);
            }else if (c.Cliente != null)
            {
                aux.ImagenAutor = c.Cliente.FotosPerfil.FirstOrDefault(p => p.EsFavorito);
            }else
            {
                aux.ImagenAutor = c.Admin.FotosPerfil.FirstOrDefault(p => p.EsFavorito);
            }

            return aux;
        }
        public int ContarLikesComentario(int id)
        {
           return _repositorioComentario.ContarLikes(id);    
        }
        public Comentario ObtenerComentarioId(int ComentarioId)
        {
            return _repositorioComentario.ObtenerPorId(ComentarioId);
        }
        public void Actualizar(Comentario comentario)
        {
            _repositorioComentario.Actualizar(comentario);
        }
    }
}
