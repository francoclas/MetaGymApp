using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class Comentario
    {

        [Key]
        public int ComentarioId { get; set; }
        public string Contenido { get; set; }
        public bool EstaActivo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public List<LikeComentario> Likes { get; set; }
        // Relaciones
        public int PublicacionId { get; set; }
        public Publicacion Publicacion { get; set; }

        public int? ProfesionalId { get; set; }
        public Profesional? Profesional { get; set; }

        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        // Respuestas de comentarios
        public int? ComentarioPadreId { get; set; }
        public Comentario? ComentarioPadre { get; set; }
        public List<Comentario> Respuestas { get; set; } = new List<Comentario>();
    }
}
