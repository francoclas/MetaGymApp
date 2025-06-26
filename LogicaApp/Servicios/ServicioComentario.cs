using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioComentario: IComentarioServicio
    {
        private readonly IRepositorioComentario _repo;

        public ServicioComentario(IRepositorioComentario repo)
        {
            _repo = repo;
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

        public void AgregarComentario(ComentarioDTO dto)
        {
            var nuevo = new Comentario
            {
                Contenido = dto.Contenido,
                PublicacionId = dto.ComentarioPadreId == null ? dto.ComentarioId : 0, // asegura validación externa
                ComentarioPadreId = dto.ComentarioPadreId,
                ProfesionalId = dto.RolAutor == "Profesional" ? dto.AutorId : null,
                ClienteId = dto.RolAutor == "Cliente" ? dto.AutorId : null,
                AdminId = dto.RolAutor == "Admin" ? dto.AutorId : null
            };

            _repo.Agregar(nuevo);
        }

        public void EditarComentario(int comentarioId, string nuevoContenido)
        {
            _repo.ActualizarContenido(comentarioId, nuevoContenido);
        }

        public void EliminarComentario(int comentarioId)
        {
            _repo.Desactivar(comentarioId);
        }

        public void DarLike(int comentarioId)
        {
            _repo.IncrementarLikes(comentarioId);
        }

        public void QuitarLike(int comentarioId)
        {
            _repo.DecrementarLikes(comentarioId);
        }

        private ComentarioDTO ConstruirDTO(Comentario c)
        {
            return new ComentarioDTO
            {
                ComentarioId = c.ComentarioId,
                Contenido = c.Contenido,
                FechaCreacion = c.FechaCreacion,
                FechaEdicion = c.FechaEdicion,
                AutorId = c.ProfesionalId ?? c.ClienteId ?? c.AdminId ?? 0,
                AutorNombre = c.Profesional?.NombreCompleto ?? c.Cliente?.NombreCompleto ?? c.Admin?.NombreCompleto ?? "Desconocido",
                RolAutor = c.Profesional != null ? "Profesional" : c.Cliente != null ? "Cliente" : "Admin",
                CantLikes = c.CantLikes,
                ComentarioPadreId = c.ComentarioPadreId,
                Respuestas = c.Respuestas?
                    .Where(r => r.EstaActivo)
                    .Select(r => ConstruirDTO(r))
                    .ToList() ?? new List<ComentarioDTO>()
            };
        }
    }
}
