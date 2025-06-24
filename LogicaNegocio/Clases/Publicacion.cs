using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool EsPrivada { get; set; }
        public bool EstaActiva { get; set; } = true;
        public int Vistas { get; set; }


        //Relaciones
        //Imagenes
        public List<Media> ListaMedia { get; set; } = new List<Media>();
        public int? ProfesionalId { get; set; }
        public Profesional? Profesional { get; set; }
        //Comentarios
        public List<Comentario> Comentarios { get; set; }
        //Referencia admin
        [ForeignKey("Admin")]
        [InverseProperty("PublicacionesAceptadasAdmin")]
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
