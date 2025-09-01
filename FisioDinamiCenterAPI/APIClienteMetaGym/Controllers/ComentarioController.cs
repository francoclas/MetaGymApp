using System.Security.Claims;
using APIClienteMetaGym.DTO;
using APIClienteMetaGym.DTO.PublicacionAPI;
using APIClienteMetaGym.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIClienteMetaGym.Controllers
{
    [ApiController]
    [Route("api/comentarios")]
    [Authorize]
    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioServicio _comentarioServicio;

        public ComentarioController(IComentarioServicio comentarioServicio)
        {
            _comentarioServicio = comentarioServicio;
        }

        /// <summary>
        /// Agrega un comentario o respuesta a una publicación.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RespuestaApi<ComentarioVistaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status400BadRequest)]
        public IActionResult AgregarComentario([FromBody] ComentarioCrearDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Contenido) || dto.PublicacionId <= 0 || dto.AutorId <= 0)
                return BadRequest(RespuestaApi<string>.Error("Datos incompletos para crear el comentario."));

            if (dto.RolAutor != "Cliente")
                return StatusCode(403, RespuestaApi<string>.Forbidden());

            // Guardamos el comentario
            var nuevo = MapearCrearDTO(dto);
            try
            {
                var comentarioCreado = _comentarioServicio.AgregarComentario(nuevo);
                if (comentarioCreado == null)
                    return BadRequest(RespuestaApi<string>.Error("No se pudo crear el comentario."));
                // Mapear al DTO de vista
                var mapeador = new MapeadorPublicaciones();
                var dtoVista = mapeador.MapearComentario(comentarioCreado);
                return Ok(RespuestaApi<ComentarioVistaDTO>.Ok(dtoVista));
            }
            catch (Exception e)
            {
                return NotFound(RespuestaApi<string>.NotFound(e.Message));
            }
        }


        /// <summary>
        /// Edita el contenido de un comentario existente.
        /// </summary>
        [HttpPut("{comentarioId}")]
        [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
        public IActionResult EditarComentario(int comentarioId, [FromQuery] string nuevoContenido)
        {
            if (string.IsNullOrWhiteSpace(nuevoContenido))
                return BadRequest(RespuestaApi<string>.Error("El contenido no puede estar vacío."));

            // Obtener claims del token
            var usuarioIdToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rolToken = User.FindFirst(ClaimTypes.Role)?.Value ?? "Cliente";

            if (usuarioIdToken == null || rolToken != "Cliente")
                return StatusCode(403, RespuestaApi<string>.Forbidden("No autorizado."));

            try
            {
                // Verificar que el comentario pertenece al usuario autenticado
                int usuarioId = int.Parse(usuarioIdToken);
                _comentarioServicio.EditarComentario(comentarioId, nuevoContenido, usuarioId, rolToken);

                return Ok(RespuestaApi<string>.Ok("Comentario editado correctamente."));
            }
            catch (Exception e)
            {
                return BadRequest(RespuestaApi<string>.Error(e.Message));
            }
        }

        /// <summary>
        /// Alterna el like del cliente sobre un comentario.
        /// </summary>
        [HttpPatch("{comentarioId}/like")]
        [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
        public IActionResult AlternarLikeComentario(int comentarioId, [FromQuery] int usuarioId, [FromQuery] string rol)
        {
            if (rol != "Cliente")
                return StatusCode(403, RespuestaApi<string>.Forbidden());

            var yaDioLike = _comentarioServicio.UsuarioYaDioLikeComentario(comentarioId, usuarioId, rol);

            if (yaDioLike)
            {
                _comentarioServicio.QuitarLikeComentario(comentarioId, usuarioId, rol);
                return Ok(RespuestaApi<string>.Ok("Like quitado del comentario."));
            }
            else
            {
                _comentarioServicio.DarLikeComentario(comentarioId, usuarioId, rol);
                return Ok(RespuestaApi<string>.Ok("Like agregado al comentario."));
            }
        }

        // Mapeo interno
        private ComentarioDTO MapearCrearDTO(ComentarioCrearDTO dto)
        {
            return new ComentarioDTO
            {
                PublicacionId = dto.PublicacionId,
                Contenido = dto.Contenido,
                AutorId = dto.AutorId,
                RolAutor = "Cliente",
                ComentarioPadreId = dto.ComentarioPadreId,
                FechaCreacion = DateTime.Now
            };
        }
    }
}
