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
        private readonly IRepositorioComentario _repo;
        private readonly Lazy<INotificacionServicio> notificacionServicio;
        private readonly Lazy<IPublicacionServicio> publicacionServicio;
        public ServicioComentario(IRepositorioComentario repo, Lazy<INotificacionServicio> notificacionServicio, Lazy<IPublicacionServicio> publicacionServicio)
        {
            _repo = repo;
            this.notificacionServicio = notificacionServicio;
            this.publicacionServicio = publicacionServicio;
        }

        public List<ComentarioDTO> ObtenerPorPublicacion(int publicacionId)
        {
            var comentarios = _repo.ObtenerPorPublicacion(publicacionId);
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
            //Lo mando al repo
            _repo.Agregar(nuevo);

            // Notificaciones
            if (dto.ComentarioPadreId == null)
            {
                // Comentario sobre una publicación se notifica al autor de la publicación
                PublicacionDTO publicacion = publicacionServicio.Value.ObtenerPorId(dto.PublicacionId);

                if (publicacion != null)
                {
                    notificacionServicio.Value.NotificarComentario(publicacion.AutorId, publicacion.RolAutor, publicacion.Id, dto.Contenido);
                }
            }
            else
            {
                // Respuesta a otro comentario para notificar al autor del comentario padre
                var padre = _repo.ObtenerPorId((int)dto.ComentarioPadreId);

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
                        notificacionServicio.Value.NotificarInteraccionComentario((int)receptorId, rolReceptor, padre.ComentarioId,"Comentaron: " + dto.Contenido);
                            };
                }
            }
            return ConstruirDTO(ObtenerComentarioId(nuevo.ComentarioId));
            }

        public void EditarComentario(int comentarioId, string nuevoContenido, int usuarioId)
        {
            //valido datos
            if (String.IsNullOrEmpty(nuevoContenido)) throw new Exception("El comentario no puede estar vacio");
            //obtengo comentario
            Comentario comentario = _repo.ObtenerPorId(comentarioId);
            //verifico que existga
            if (comentario == null) throw new Exception("El comentario no existe o fue eliminado");
            //valido si son diferentes lo actualizo
            if (!String.Equals(nuevoContenido, comentario.Contenido)) {
                _repo.ActualizarContenido(comentarioId, nuevoContenido);
            }

        }

        public void EliminarComentario(int comentarioId, int usuarioId, string rol)
        {
            //este metodo es solo para que se eliminen los comentarios de parte de su autor
            //obtengo comentario
            Comentario comentario = _repo.ObtenerPorId(comentarioId);
            _repo.Desactivar(comentarioId);
        }
        public void DarLikeComentario(int comentarioId, int usuarioId, string rol)
        {
            _repo.DarLike(comentarioId, usuarioId, rol);
        }

        public void QuitarLikeComentario(int comentarioId, int usuarioId, string rol)
        {
            _repo.QuitarLike(comentarioId, usuarioId, rol);
        }

        public bool UsuarioYaDioLikeComentario(int comentarioId, int usuarioId, string rol)
        {
            return _repo.UsuarioYaDioLike(comentarioId, usuarioId, rol);
        }


        private ComentarioDTO ConstruirDTO(Comentario c)
        {
            ComentarioDTO aux = new ComentarioDTO
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
           return _repo.ContarLikes(id);    
        }
        public Comentario ObtenerComentarioId(int ComentarioId)
        {
            return _repo.ObtenerPorId(ComentarioId);
        }
        public void Actualizar(Comentario comentario)
        {
            _repo.Actualizar(comentario);
        }
    }
}
