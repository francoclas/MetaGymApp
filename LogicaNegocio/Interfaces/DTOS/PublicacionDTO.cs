using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class PublicacionDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public Enum_EstadoPublicacion Estado { get; set; }
        public bool EsPrivada { get; set; }
        public int Vistas { get; set; }
        public int CantLikes { get; set; }

        public string? NombreAutor { get; set; }
        public string RolAutor { get; set; }

        public string? NombreAprobador { get; set; }
        public string? NombreCreadorAdmin { get; set; }

        public List<string> UrlsMedia { get; set; }
        public List<ComentarioDTO> Comentarios { get; set; }
    }
}
