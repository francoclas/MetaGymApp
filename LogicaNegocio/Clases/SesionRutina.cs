using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    //Para gestionar el historico del cliente
    public class SesionRutina
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int? RutinaAsignadaId { get; set; }
        public RutinaAsignada RutinaAsignada { get; set; }

        public DateTime FechaRealizada { get; set; }
        public int? DuracionMin { get; set; }

        // Datos históricos de la rutina
        public string NombreRutinaHistorial { get; set; }
        public string TipoRutinaHistorial { get; set; }

        public List<EjercicioRealizado> EjerciciosRealizados { get; set; }
    }
}
