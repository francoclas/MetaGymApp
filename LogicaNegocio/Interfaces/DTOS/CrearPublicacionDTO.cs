using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class CrearPublicacionDTO
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public bool MostrarEnNoticiasPublicas { get; set; }
        public bool EsPrivada { get; set; }
        public List<IFormFile> ArchivosMedia { get; set; } = new();
        public int? ProfesionalId { get; set; }
        public int? AdminId { get; set; }
    }
}
