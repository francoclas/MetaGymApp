using LogicaNegocio.Interfaces.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace MetaGymWebApp.Filtros
{
    public class AutorizacionRol : ActionFilterAttribute
    {
        private readonly string[] _rolesPermitidos;

        public AutorizacionRol(params string[] roles)
        {
            _rolesPermitidos = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sesionJson = context.HttpContext.Session.GetString("SesionUsuario");

            if (string.IsNullOrEmpty(sesionJson))
            {
                // Redirige al login si no hay sesión
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            var sesion = JsonSerializer.Deserialize<SesionDTO>(sesionJson);

            if (sesion == null || !_rolesPermitidos.Contains(sesion.Rol))
            {
                // Manda un mensaje de error y redirige a Inicio
                var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();
                var tempData = factory.GetTempData(context.HttpContext);

                tempData["Mensaje"] = "No tenés permisos para acceder a esta sección.";
                tempData["TipoMensaje"] = "danger";

                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
