using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace APIClienteMetaGym.Controllers
{
    [ApiController]
    [Route("api/Usuario")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IConfiguration _configuration;

        public UsuarioController(IUsuarioServicio usuarioServicio, IConfiguration configuration)
        {
            _usuarioServicio = usuarioServicio;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            SesionDTO usuario = _usuarioServicio.IniciarSesionCliente(loginDto);
            //Verifico si esta bien
            if(usuario == null)
            {
                return Unauthorized(new { success = false, error = "Verificar credenciales" });
            }
            // Obtener la clave JWT desde appsettings.json
            var clave = _configuration["Jwt:Key"];
            var token = GestionJWT.GenerarToken(usuario, clave);

            usuario.Token = token;

            return Ok(new { success = true, data = usuario });
        }

    }
}
