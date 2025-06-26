using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Clases
{
    public class Publicacion
    {
        public Publicacion() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public Enum_EstadoPublicacion Estado { get; set; }
        public string? MotivoRechazo { get; set; }

        public bool EsPrivada { get; set; }
        public int Vistas { get; set; }
        public int CantLikes { get; set; }
        public List<Media> ListaMedia { get; set; } = new();
        public List<Comentario> Comentarios { get; set; } = new();

        public int? ProfesionalId { get; set; }
        public Profesional? Profesional { get; set; }

        [ForeignKey("Admin")]
        [InverseProperty("PublicacionesAceptadasAdmin")]
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
