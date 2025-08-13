using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    //Es el ejercicio base
    public class Ejercicio
    {
        public Ejercicio() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Tipo { get; set; }
        public string? GrupoMuscular { get; set; }
        public string? Instrucciones { get; set; }
        //Profesional
        public int ProfesionalId { get; set; }
        public Profesional Profesional { get; set; }
        public List<Media> Medias { get; set; } = new List<Media>();
        public List<RutinaEjercicio> RutinaEjercicios { get; set; } = new();
        //Medicion
        public List<Medicion> Mediciones { get; set; } = new();
    }
}
