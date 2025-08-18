using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS.API
{
    public class HorarioDisponibleDTO
    {
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public bool Ocupado { get; set; } 
    }
}
