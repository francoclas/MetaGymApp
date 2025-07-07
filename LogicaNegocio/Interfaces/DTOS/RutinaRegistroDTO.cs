using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class RutinaRegistroDTO
    {
        public int Id { get; set; }
        public string NombreRutina { get; set; }
        public string Tipo { get; set; }
        public List<int> IdsEjerciciosSeleccionados { get; set; } = new();
        public List<int> IdsClientesAsignados { get; set; } = new();
        
        // Para la vista (no se postean)
        public List<EjercicioDTO> EjerciciosDisponibles { get; set; } = new();
        public List<ClienteDTO> ClientesDisponibles { get; set; } = new();
    }
}
