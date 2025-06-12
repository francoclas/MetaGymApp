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
        public DateTime Fecha { get; set; }

        public List<Media> ListaMedia { get; set; } = new List<Media>();
        //Referencia Profesional
        [ForeignKey("Profesional")]
        [InverseProperty("PublicacionesCreadasProfesional")]
        public int ProfesionalId { get; set; }
        public Profesional Profesional { get; set; }

        //Referencia admin
        [ForeignKey("Admin")]
        [InverseProperty("PublicacionesAceptadasAdmin")]
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
