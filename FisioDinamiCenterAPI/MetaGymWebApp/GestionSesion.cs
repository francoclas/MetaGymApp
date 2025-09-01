using System.Text.Json;
using LogicaNegocio.Interfaces.DTOS;

namespace MetaGymWebApp
{
    public static class GestionSesion
    {
        private const string CLAVE_SESION = "SesionUsuario";
        public static void SetearSesion(HttpContext httpContext, SesionDTO sesion)
        {
            var json = JsonSerializer.Serialize(sesion);
            httpContext.Session.SetString(CLAVE_SESION, json);
        }
        public static void CerrarSesion(HttpContext httpContext)
        {
            httpContext.Session.Remove(CLAVE_SESION);
        }
        public static SesionDTO ObtenerSesion(HttpContext httpContext)
        {
            var json = httpContext.Session.GetString("SesionUsuario");

            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<SesionDTO>(json);
        }
        public static int ObtenerUsuarioId(HttpContext httpContext)
        {
            return ObtenerSesion(httpContext)?.UsuarioId ?? 0;
        }

        public static string ObtenerRol(HttpContext httpContext)
        {
            return ObtenerSesion(httpContext)?.Rol;
        }

        public static bool EstaLogueado(HttpContext httpContext)
        {
            return ObtenerSesion(httpContext) != null;
        }
    }
}
