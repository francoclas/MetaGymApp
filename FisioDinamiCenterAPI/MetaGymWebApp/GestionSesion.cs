using System.Text.Json;
using LogicaNegocio.Interfaces.DTOS;

namespace MetaGymWebApp
{
    // Clase estática para manejar la sesión del usuario en la web
    public static class GestionSesion
    {
        private const string CLAVE_SESION = "SesionUsuario";

        // Setea la sesión guardando el objeto SesionDTO en formato JSON
        public static void SetearSesion(HttpContext httpContext, SesionDTO sesion)
        {
            var json = JsonSerializer.Serialize(sesion);
            httpContext.Session.SetString(CLAVE_SESION, json);
        }

        // Cierra sesión borrando la clave de sesión del contexto
        public static void CerrarSesion(HttpContext httpContext)
        {
            httpContext.Session.Remove(CLAVE_SESION);
        }

        // Obtiene la sesión actual deserializando el JSON guardado
        public static SesionDTO ObtenerSesion(HttpContext httpContext)
        {
            var json = httpContext.Session.GetString(CLAVE_SESION);

            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<SesionDTO>(json);
        }

        // Devuelve el Id de usuario actual (o 0 si no hay sesión)
        public static int ObtenerUsuarioId(HttpContext httpContext)
        {
            return ObtenerSesion(httpContext)?.UsuarioId ?? 0;
        }

        // Devuelve el rol del usuario actual
        public static string ObtenerRol(HttpContext httpContext)
        {
            return ObtenerSesion(httpContext)?.Rol;
        }

        // Indica si hay sesión activa
        public static bool EstaLogueado(HttpContext httpContext)
        {
            return ObtenerSesion(httpContext) != null;
        }
    }
}