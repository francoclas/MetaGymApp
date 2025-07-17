using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.DTOS.API;
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
        [HttpPost("registro")]
        public IActionResult registro(RegistroUsuarioDTO registro)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(registro.Correo) ||
                string.IsNullOrWhiteSpace(registro.Usuario) ||
                string.IsNullOrWhiteSpace(registro.Pass) ||
                string.IsNullOrWhiteSpace(registro.Confirmacion))
            {
                return BadRequest(new { success = false, error = "Faltan campos obligatorios." });
            }

            if (registro.Pass != registro.Confirmacion)
            {
                return BadRequest(new { success = false, error = "Las contraseñas no coinciden." });
            }

            // Llamar al servicio
            //falta implementar
            SesionDTO sesion = null;//_usuarioServicio.RegistrarCliente(registro);

            if (sesion == null)
            {
                return Conflict(new { success = false, error = "El correo o nombre de usuario ya está en uso." });
            }

            // Generar token
            var token = GestionJWT.GenerarToken(sesion, _configuration["Jwt:Key"]);
            sesion.Token = token;

            return Ok(new { success = true, data = sesion });
        }
    }
}
