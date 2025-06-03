using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaApp.DTOS
{
    public class CitaDTO
    {
        public CitaDTO() { }
        public int ClienteId {  get; set; }
        public int EspecialidadId { get; set; }
        public int EstablecimientoId { get; set; }
        public string? Descripcion {  get; set; }
        public DateTime FechaAsistencia { get; set; }

        public DateTime? FechaCreacion { get; set; }
        public int? ProfesionalId { get; set; }
        public string? Conclusion { get; set; }
    }
}
