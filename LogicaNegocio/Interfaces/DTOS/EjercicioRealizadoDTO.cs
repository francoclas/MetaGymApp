using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class EjercicioRealizadoDTO
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string GrupoMuscular { get; set; }
        public string? ImagenURL { get; set; }
        public List<SerieDTO> Series { get; set; }
        public List<MedicionDTO> Mediciones { get; set; }
    }
}
