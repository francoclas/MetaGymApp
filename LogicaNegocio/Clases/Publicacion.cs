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
        public bool MostrarEnNoticiasPublicas {  get; set; } = false;
        public bool EsPrivada { get; set; }
        public int Vistas { get; set; }
        public int CantLikes { get; set; }
        public List<Media> ListaMedia { get; set; } = new List<Media>();
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        //Interacciones
        public List<LikePublicacion> Likes { get; set; }
        // Quien la crea (opcional: Profesional o Admin)
        public int? ProfesionalId { get; set; }
        public Profesional? Profesional { get; set; }

        public int? AdminCreadorId { get; set; }
        public Admin? AdminCreador { get; set; }

        // Quien la aprueba (solo si fue creada por profesional)
        public int? AdminAprobadorId { get; set; }
        public Admin? AdminAprobador { get; set; }
    }
}
