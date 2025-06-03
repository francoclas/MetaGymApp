using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class Especialidad
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public string NombreEspecialidad { get; set; }
        public string DescripcionEspecialidad { get; set; }

        //Referencia a profesionales
        public List<Profesional> Profesionales { get; set; } = new List<Profesional>();
        //Referencia a citas
        public List<Cita> Citas { get; set; } = new List<Cita>();

        public Especialidad() { }
        public Especialidad(string Nombre,string DescripcionEspecialidad)
        {
            this.NombreEspecialidad = Nombre;
            this.DescripcionEspecialidad = DescripcionEspecialidad;
        }
    }
}
