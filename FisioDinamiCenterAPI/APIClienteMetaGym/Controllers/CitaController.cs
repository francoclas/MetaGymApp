using APIClienteMetaGym.DTO;
using APIClienteMetaGym.Extra;
using LogicaApp.DTOS;
using LogicaNegocio.Interfaces.DTOS.API;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIClienteMetaGym.Controllers;

/// <summary>
/// Endpoints de gestión de citas para clientes.
/// </summary>
[ApiController]
[Route("api/cita")]
public class CitaController : ControllerBase
{
    private readonly ICitaServicio _citaServicio;

    public CitaController(ICitaServicio citaServicio)
    {
        _citaServicio = citaServicio;
    }

    private bool EsCliente(string rol) =>
        string.Equals(rol, "Cliente", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Obtiene las citas del cliente autenticado, opcionalmente filtradas por estado.
    /// </summary>
    [Authorize]
    [HttpGet("cliente")]
    [ProducesResponseType(typeof(RespuestaApi<List<CitaAPIDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult ObtenerCitasCliente(int clienteId, int estadoEnum, string rol)
    {
        if (!EsCliente(rol))
            return StatusCode(403, RespuestaApi<string>.Forbidden());

        try
        {
            List<CitaDTO> citas = _citaServicio.ObtenerCitasClientes(clienteId, estadoEnum);
            List<CitaAPIDTO> salida = new List<CitaAPIDTO>();
            MapeadorCitas aux = new MapeadorCitas();
            foreach (var citaDTO in citas) {
                salida.Add(aux.MapearCitaAPIDTO(citaDTO));
            }
            return Ok(RespuestaApi<List<CitaAPIDTO>>.Ok(salida));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest(ex.Message));
        }
    }

    /// <summary>
    /// Obtiene los detalles de una cita específica.
    /// </summary>
    [Authorize]
    [HttpGet("{citaId}/detalles")]
    [ProducesResponseType(typeof(RespuestaApi<CitaAPIDetallesDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult ObtenerDetallesCita(int citaId, string rol)
    {
        if (!EsCliente(rol))
            return StatusCode(403, RespuestaApi<string>.Forbidden());

        try
        {
            CitaDTO detalles = _citaServicio.ObtenerDetallesCita(citaId);
            return Ok(RespuestaApi<CitaAPIDetallesDTO>.Ok(new MapeadorCitas().MapearCitaAPIDetallesDTO(detalles)));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest(ex.Message));
        }
    }

    /// <summary>
    /// Obtiene la lista de estados posibles para las citas.
    /// </summary>
    [HttpGet("estados")]
    [ProducesResponseType(typeof(RespuestaApi<List<object>>), StatusCodes.Status200OK)]
    public IActionResult ObtenerEstadosCitas()
    {
        try
        {
            return Ok(RespuestaApi<IEnumerable<object>>.Ok(new MapeadorCitas().MapearEstadosCita()));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaApi<string>.BadRequest(ex.Message));
        }
    }
}
