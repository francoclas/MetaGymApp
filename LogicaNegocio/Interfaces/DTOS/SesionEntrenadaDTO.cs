using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class SesionEntrenadaDTO
    {
        public string NombreRutina { get; set; }
        public DateTime FechaRealizada { get; set; }
        public int? DuracionMin { get; set; }
        public List<EjercicioRealizadoDTO> Ejercicios { get; set; }
    }
}
