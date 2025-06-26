using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class ModerarPublicacionDTO
    {
        public int PublicacionId { get; set; }
        public bool Aprobar { get; set; }
        public int AdminId { get; set; }
        public string? MotivoRechazo { get; set; } // obligatorio si Aprobar = false
    }
}
