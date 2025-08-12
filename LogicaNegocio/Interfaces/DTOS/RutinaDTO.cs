using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class RutinaDTO
    {
        public int Id { get; set; }
        public string NombreRutina { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimaModificacion { get; set; }
        public List<EjercicioDTO> Ejercicios { get; set; }
    }
}
