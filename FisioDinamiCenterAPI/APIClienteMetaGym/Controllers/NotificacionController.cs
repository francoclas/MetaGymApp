using Microsoft.AspNetCore.Mvc;
using LogicaNegocio.Interfaces.Servicios;
using APIClienteMetaGym.DTO;
using LogicaNegocio.Interfaces.DTOS;

namespace ApiClienteMetaGym.Controllers;

/// <summary>
/// Endpoints para acceder y gestionar notificaciones del cliente.
/// </summary>
[ApiController]
[Route("api/notificaciones")]
public class NotificacionController : ControllerBase
{
    private readonly INotificacionServicio _notificacionServicio;

    public NotificacionController(INotificacionServicio notificacionServicio)
    {
        _notificacionServicio = notificacionServicio;
    }

    [HttpGet("no-leidas")]
    [ProducesResponseType(typeof(RespuestaApi<List<NotificacionDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult ObtenerNoLeidas(int usuarioId, string rol)
    {
        if (!EsCliente(rol)) return Forbid();

        var resultado = _notificacionServicio.ObtenerNoLeidasUsuario(usuarioId, rol);
        return Ok(RespuestaApi<List<NotificacionDTO>>.Ok(resultado));
    }

    [HttpGet("leidas")]
    [ProducesResponseType(typeof(RespuestaApi<List<NotificacionDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult ObtenerLeidas(int usuarioId, string rol)
    {
        if (!EsCliente(rol)) return Forbid();

        var resultado = _notificacionServicio.ObtenerLeidasUsuario(usuarioId, rol);
        return Ok(RespuestaApi<List<NotificacionDTO>>.Ok(resultado));
    }

    [HttpGet("no-leidas/contar")]
    [ProducesResponseType(typeof(RespuestaApi<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult ContarNoLeidas(int usuarioId, string rol)
    {
        if (!EsCliente(rol)) return Forbid();

        var cantidad = _notificacionServicio.ContarNoLeidas(usuarioId, rol);
        return Ok(RespuestaApi<int>.Ok(cantidad));
    }

    [HttpPatch("{id}/leer")]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult MarcarComoLeida(int id, int usuarioId, string rol)
    {
        if (!EsCliente(rol)) return Forbid();

        _notificacionServicio.MarcarComoLeida(id);
        return Ok(RespuestaApi<string>.NoContent("Notificación marcada como leída."));
    }

    [HttpPatch("leer-todas")]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult MarcarTodasComoLeidas(int usuarioId, string rol)
    {
        if (!EsCliente(rol)) return Forbid();

        _notificacionServicio.MarcarTodasComoLeidas(usuarioId, rol);
        return Ok(RespuestaApi<string>.NoContent("Todas las notificaciones fueron marcadas como leídas."));
    }

    [HttpGet("ultimas")]
    [ProducesResponseType(typeof(RespuestaApi<List<NotificacionDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult ObtenerUltimas(int usuarioId, string rol, int cantidad = 5)
    {
        if (!EsCliente(rol)) return Forbid();

        var resultado = _notificacionServicio.ObtenerUltimas(usuarioId, rol, cantidad);
        return Ok(RespuestaApi<List<NotificacionDTO>>.Ok(resultado));
    }

    // Función común para validar el rol
    private bool EsCliente(string rol) =>
        string.Equals(rol, "Cliente", StringComparison.OrdinalIgnoreCase);
}
