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
        public Rutina() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NombreRutina { get; set; }

        [ForeignKey("Profesional")]
        [InverseProperty("RutinasCreadasProfesional")]
        public int ProfesionalId { get; set; }
        public Profesional Profesional { get; set; }

        public string Tipo { get; set; }

        public List<SesionRutina> RutinaEjercicios { get; set; }

        public List<Cliente> Asignados = new List<Cliente>();

    }
}
