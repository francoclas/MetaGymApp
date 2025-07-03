using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    //Rutina a modo de plantilla
    public class RutinaEjercicio
    {
        public int Id { get; set; }

        public int RutinaId { get; set; }
        public Rutina Rutina { get; set; }

        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }

        public int Orden { get; set; } 
    }
}
