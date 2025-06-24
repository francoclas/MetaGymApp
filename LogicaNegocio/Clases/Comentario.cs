using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class Comentario
    {
        public Comentario() { }
        public int ComentarioId { get; set; }
        public string Contenido { get; set; }
        public bool EstaActivo {  get; set; }

        public DateTime FechaCreacion { get; set; }

        //relaciones
        public int PublicacionId { get; set; }
        public Publicacion Publicacion { get; set; }
        public int? ProfesionalId { get; set; }
        public Profesional? Profesional { get; set; }

        public int? ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int? AdminId { get; set; }
        public Admin Admin { get; set; }    
    }
}
