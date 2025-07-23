using APIClienteMetaGym.DTO;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.DTOS.API;
using LogicaNegocio.Interfaces.Servicios;
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
    /// <response code="200">Login exitoso.</response>
    /// <response code="401">Credenciales incorrectas.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(RespuestaApi<SesionDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginDTO loginDto)
    {
        var usuario = _usuarioServicio.IniciarSesionCliente(loginDto);

        if (usuario == null)
            return Unauthorized(RespuestaApi<string>.Unauthorized("Credenciales inválidas."));

        var clave = _configuration["Jwt:Key"];
        var token = GestionJWT.GenerarToken(usuario, clave);
        usuario.Token = token;

        return Ok(RespuestaApi<SesionDTO>.Ok(usuario));
    }

    /// <summary>
    /// Registra un nuevo cliente y devuelve su sesión con token.
    /// </summary>
    /// <param name="registro">Datos del nuevo cliente.</param>
    /// <returns>Sesión con token si el registro fue exitoso.</returns>
    /// <response code="200">Registro exitoso.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="409">Usuario o correo ya registrado.</response>
    [HttpPost("registro")]
    [ProducesResponseType(typeof(RespuestaApi<SesionDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status409Conflict)]
    public IActionResult Registro([FromBody] RegistroUsuarioDTO registro)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(registro.Correo) ||
            string.IsNullOrWhiteSpace(registro.Usuario) ||
            string.IsNullOrWhiteSpace(registro.Pass) ||
            string.IsNullOrWhiteSpace(registro.Confirmacion))
        {
            return BadRequest(RespuestaApi<string>.BadRequest("Faltan campos obligatorios."));
        }

        if (registro.Pass != registro.Confirmacion)
        {
            return BadRequest(RespuestaApi<string>.BadRequest("Las contraseñas no coinciden."));
        }

        // Llamar al servicio
        SesionDTO sesion = null; // _usuarioServicio.RegistrarCliente(registro);

        //falta implementar

        if (sesion == null)
        {
            return BadRequest(RespuestaApi<string>.BadRequest("El correo o nombre de usuario ya está en uso."));
        }

        var token = GestionJWT.GenerarToken(sesion, _configuration["Jwt:Key"]);
        sesion.Token = token;

        return Ok(RespuestaApi<SesionDTO>.Ok(sesion, "Registro exitoso."));
    }
}
