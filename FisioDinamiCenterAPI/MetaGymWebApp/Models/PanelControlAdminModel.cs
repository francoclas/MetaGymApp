using LogicaNegocio.Clases;

namespace MetaGymWebApp.Models
{
    public class PanelControlAdminModel
    {
        public List<Establecimiento> Establecimientos { get; set; } = new();
        public List<Especialidad> Especialidades { get; set; } = new();
    }
}
