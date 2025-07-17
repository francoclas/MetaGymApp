using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class ComentarioDTO
    {   
        public int PublicacionId { get; set; }
        public int ComentarioId { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }

        public int AutorId { get; set; }
        public string AutorNombre { get; set; }
        public string RolAutor { get; set; }
        public Media ImagenAutor { get; set; }
        public int CantLikes { get; set; }

        public int? ComentarioPadreId { get; set; }
        public List<ComentarioDTO> Respuestas { get; set; } = new();
    }
}
