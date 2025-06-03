using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class RutinaEjercicio
    {
        public RutinaEjercicio() { }
        public int RutinaId { get; set; }
        public Rutina Rutina { get; set; }
        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }
        public int Orden { get; set; }
        public int Repeticiones { get; set; }
        public int Series { get; set; }


    }
}
