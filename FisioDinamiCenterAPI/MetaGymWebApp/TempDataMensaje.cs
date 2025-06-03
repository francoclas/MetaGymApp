using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp
{
    public static class TempDataMensaje
    {
        public static void SetMensaje(Controller controller, string mensaje, string tipo = "info")
        {
            controller.TempData["Mensaje"] = mensaje;
            controller.TempData["TipoMensaje"] = tipo;
        }
    }
}
