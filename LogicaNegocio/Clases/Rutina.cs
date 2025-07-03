using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class Rutina
    {
        [Key]
        public int Id { get; set; }
        public string NombreRutina { get; set; }
        public int ProfesionalId { get; set; }
        public Profesional Profesional { get; set; }
        public string Tipo { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public List<RutinaEjercicio> Ejercicios { get; set; } = new();
        public List<RutinaAsignada> Asignaciones { get; set; } = new();
    }
}
