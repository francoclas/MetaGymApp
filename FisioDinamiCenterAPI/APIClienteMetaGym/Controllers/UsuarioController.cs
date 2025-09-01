using System.Security.Claims;
using APIClienteMetaGym.DTO;
using APIClienteMetaGym.Extra;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.DTOS.API;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIClienteMetaGym.Controllers;

/// <summary>
/// Endpoints de autenticación de clientes.
/// </summary>
[ApiController]
[Route("api/usuario")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioServicio _usuarioServicio;
    private readonly IConfiguration _configuration;
    public UsuarioController(IUsuarioServicio usuarioServicio, IConfiguration configuration)
    {
        _usuarioServicio = usuarioServicio;
        _configuration = configuration;
    }

    /// <summary>
    /// Inicia sesión de un cliente con usuario o correo y contraseña.
    /// </summary>
    /// <param name="loginDto">DTO con usuario/correo y contraseña.</param>
    /// <returns>Sesión del cliente con token JWT si las credenciales son válidas.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(RespuestaApi<SesionDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginDTO loginDto)
    {
        try
        {
            var usuario = _usuarioServicio.IniciarSesionCliente(loginDto);

            if (usuario == null)
                return Unauthorized(RespuestaApi<string>.Unauthorized("Credenciales inválidas."));

            var clave = _configuration["Jwt:Key"];
            var token = GestionJWT.GenerarToken(usuario, clave);
            usuario.Token = token;

            return Ok(RespuestaApi<SesionDTO>.Ok(usuario));
        }
        catch (Exception e)
        {
            return Unauthorized(RespuestaApi<string>.Unauthorized(e.Message));
        }
        
    }
    private bool EsCliente(string rol) =>
        string.Equals(rol, "Cliente", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Obtiene la información básica del cliente autenticado.
    /// </summary>
    [Authorize]
    [HttpGet("perfil")]
    [ProducesResponseType(typeof(RespuestaApi<UsuarioGenericoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult ObtenerPerfil(int usuarioId, string rol)
    {
        if (!EsCliente(rol))
            return StatusCode(403, RespuestaApi<string>.Forbidden());
        try
        {
            var dto = _usuarioServicio.ObtenerUsuarioGenericoDTO(usuarioId,"Cliente");
            return Ok(RespuestaApi<ClienteDTOAPI>.Ok(new MapeadorUsuario().MapearCiente(dto)));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest("Usuario no existe."));
        }
    }
    /// <summary>
    /// Cambia el teléfono del cliente.
    /// </summary>
    [Authorize]
    [HttpPatch("{id}/telefono")]
    public IActionResult CambiarTelefono(int id, string telefonoNuevo)
    {
        // Usuario autenticado desde el JWT
        var usuarioIdToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nombreUsuarioToken = User.FindFirst(ClaimTypes.Name)?.Value;

        if (usuarioIdToken == null || usuarioIdToken != id.ToString())
            return StatusCode(403, RespuestaApi<string>.Forbidden("No puede modificar datos de otro usuario."));

        try
        {
            _usuarioServicio.CambiarTelefono(id, nombreUsuarioToken, telefonoNuevo);
            return Ok(RespuestaApi<string>.Ok("Teléfono actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest(ex.Message));
        }
    }

    /// <summary>
    /// Cambia el correo del cliente.
    /// </summary>
    [Authorize]
    [HttpPatch("{id}/correo")]
    public IActionResult CambiarCorreo(int id, string correoNuevo)
    {
        var usuarioIdToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nombreUsuarioToken = User.FindFirst(ClaimTypes.Name)?.Value;

        if (usuarioIdToken == null || usuarioIdToken != id.ToString())
            return StatusCode(403, RespuestaApi<string>.Forbidden("No puede modificar datos de otro usuario."));

        try
        {
            _usuarioServicio.CambiarCorreo(id, nombreUsuarioToken, correoNuevo);
            return Ok(RespuestaApi<string>.Ok("Correo actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest(ex.Message));
        }
    }

    /// <summary>
    /// Cambia el nombre del cliente.
    /// </summary>
    [Authorize]
    [HttpPatch("{id}/nombre")]
    public IActionResult CambiarNombre(int id, string nombreNuevo)
    {
        var usuarioIdToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nombreUsuarioToken = User.FindFirst(ClaimTypes.Name)?.Value;

        if (usuarioIdToken == null || usuarioIdToken != id.ToString())
            return StatusCode(403, RespuestaApi<string>.Forbidden("No puede modificar datos de otro usuario."));

        try
        {
            _usuarioServicio.CambiarNombre(id, nombreUsuarioToken, nombreNuevo);
            return Ok(RespuestaApi<string>.Ok("Nombre actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest(ex.Message));
        }
    }

    /// <summary>
    /// Cambia la contraseña del cliente.
    /// </summary>
    [Authorize]
    [HttpPatch("{id}/password")]
    public IActionResult CambiarPass(int id, string nuevaPassword, string confPassword)
    {
        var usuarioIdToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nombreUsuarioToken = User.FindFirst(ClaimTypes.Name)?.Value;

        if (usuarioIdToken == null || usuarioIdToken != id.ToString())
            return StatusCode(403, RespuestaApi<string>.Forbidden("No puede modificar datos de otro usuario."));

        try
        {
            _usuarioServicio.CambiarPass(id, nombreUsuarioToken, nuevaPassword, confPassword);
            return Ok(RespuestaApi<string>.Ok("Contraseña actualizada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest(ex.Message));
        }
    }

}
