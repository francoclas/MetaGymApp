using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;
using MetaGymWebApp.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaGymWebApp.Controllers
{
    [AutorizacionRol("Profesional")]
    public class AgendaController : Controller
    {
        private readonly IAgendaServicio _agendaServicio;

        public AgendaController(IAgendaServicio agendaServicio)
        {
            _agendaServicio = agendaServicio;
        }

        public IActionResult MisAgendas()
        {
            int profesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
            var agendas = _agendaServicio.ObtenerAgendaDelProfesional(profesionalId);
            return View(agendas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(AgendaProfesional agenda)
        {
            try
            {
                agenda.ProfesionalId = GestionSesion.ObtenerUsuarioId(HttpContext);
                _agendaServicio.RegistrarAgenda(agenda);
                TempData["Mensaje"] = "Jornada registrada correctamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("MisAgendas");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("MisAgendas");
            }
        }

        public IActionResult Eliminar(int id)
        {
            try
            {
                _agendaServicio.EliminarAgenda(id);
                TempData["Mensaje"] = "Agenda eliminada.";
                TempData["TipoMensaje"] = "success";
            }
            catch
            {
                TempData["Mensaje"] = "No se pudo eliminar la agenda.";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction("MisAgendas");
        }
        [HttpPost]
        public IActionResult MarcarActivo(int id, bool activo)
        {
            try
            {
                AgendaProfesional agenda = _agendaServicio.ObtenerPorId(id);
                if (agenda == null)
                    throw new Exception("No se encontró la agenda.");

                agenda.Activo = activo;
                _agendaServicio.ActualizarAgenda(agenda);

                TempData["Mensaje"] = "Estado de la agenda actualizado.";
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction("MisAgendas");
        }

    }
}
