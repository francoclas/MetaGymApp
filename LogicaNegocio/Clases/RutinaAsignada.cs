using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    //Para cuando se le asigna una rutina a un cliente
    public class RutinaAsignada
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int RutinaId { get; set; }
        public Rutina Rutina { get; set; }

        public DateTime FechaAsignacion { get; set; }

        public List<SesionRutina> Sesiones { get; set; } = new();
    }
}
