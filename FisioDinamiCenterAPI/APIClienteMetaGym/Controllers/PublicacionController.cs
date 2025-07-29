using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LogicaNegocio.Interfaces.Servicios;
using LogicaNegocio.Interfaces.DTOS;



using APIClienteMetaGym.DTO.PublicacionAPI;
using APIClienteMetaGym.DTO;
using APIClienteMetaGym.Extra;

namespace APIClienteMetaGym.Controllers;

/// <summary>
/// Endpoints para ver e interactuar con publicaciones.
/// </summary>
[ApiController]
[Route("api/publicaciones")]
[Authorize]

public class PublicacionController : ControllerBase
{
    private readonly IPublicacionServicio _publicacionServicio;
    private readonly IConfiguration _configuration;

    public PublicacionController(IPublicacionServicio publicacionServicio, IConfiguration configuration)
    {
        _publicacionServicio = publicacionServicio;
        _configuration = configuration;
    }

    /// <summary>
    /// Lista las publicaciones públicas visibles para el cliente.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(RespuestaApi<List<PublicacionVistaDTO>>), StatusCodes.Status200OK)]
    public IActionResult ObtenerPublicacionesInicio()
    {
        var publicaciones = _publicacionServicio.ObtenerPublicacionesInicio();
        var baseUrl = _configuration["BaseUrl"];
        var salida = new MapeadorPublicaciones(baseUrl).MapearLista(publicaciones);

        return Ok(RespuestaApi<List<PublicacionVistaDTO>>.Ok(salida));
    }

    /// <summary>
    /// Lista las publicaciones destacadas o novedades para mostrar en el inicio.
    /// </summary>
    [HttpGet("novedades")]
    [ProducesResponseType(typeof(RespuestaApi<List<PublicacionVistaDTO>>), StatusCodes.Status200OK)]
    public IActionResult ObtenerNovedades()
    {
        var publicaciones = _publicacionServicio.ObtenerNovedades();
        var baseUrl = _configuration["BaseUrl"];
        var salida = new MapeadorPublicaciones(baseUrl).MapearLista(publicaciones);

        return Ok(RespuestaApi<List<PublicacionVistaDTO>>.Ok(salida));
    }

    /// <summary>
    /// Alterna el estado de like del cliente sobre la publicación.
    /// </summary>
    [Authorize]
    [HttpPatch("{id}/like")]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult AlternarLike(int id, int usuarioId, string rol)
    {
        if (!EsCliente(rol))
            return StatusCode(403, RespuestaApi<string>.Forbidden());

        var yaDioLike = _publicacionServicio.UsuarioYaDioLikePublicacion(id, usuarioId, rol);

        if (yaDioLike)
        {
            _publicacionServicio.QuitarLikePublicacion(id, usuarioId, rol);
            return Ok(RespuestaApi<string>.Ok("Like quitado."));
        }
        else
        {
            _publicacionServicio.DarLikePublicacion(id, usuarioId, rol);
            return Ok(RespuestaApi<string>.Ok("Like agregado."));
        }
    }

    /// <summary>
    /// Verifica si el cliente ya dio like a una publicación.
    /// </summary>
    [Authorize]
    [HttpGet("{id}/ya-dio-like")]
    [ProducesResponseType(typeof(RespuestaApi<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespuestaApi<string>), StatusCodes.Status403Forbidden)]
    public IActionResult YaDioLike(int id, int usuarioId, string rol)
    {
        if (!EsCliente(rol))
            return StatusCode(403, RespuestaApi<string>.Forbidden());

        var resultado = _publicacionServicio.UsuarioYaDioLikePublicacion(id, usuarioId, rol);
        return Ok(RespuestaApi<bool>.Ok(resultado));
    }

    private bool EsCliente(string rol) =>
        string.Equals(rol, "Cliente", StringComparison.OrdinalIgnoreCase);
}
