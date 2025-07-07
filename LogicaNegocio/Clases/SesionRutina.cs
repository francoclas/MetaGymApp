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
        public SesionRutina() { }
        public int Id { get; set; }
       

        public int RutinaAsignadaId { get; set; }
        public RutinaAsignada RutinaAsignada { get; set; }

        public DateTime FechaRealizada { get; set; }
        public int? DuracionMin { get; set; }

        //Ejercicios
        public List<EjercicioRealizado> EjerciciosRealizados { get; set; }
    }
}
