using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Clases
{
        //Dentro del historico de las rutinas cada ejercicio
    public class EjercicioRealizado
    {
        public int Id { get; set; }

        public int SesionRutinaId { get; set; }
        public SesionRutina SesionRutina { get; set; }

        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }

        public int? Orden { get; set; } // opcional si querés preservar el orden
        public bool SeRealizo { get; set; }
        public string? Observaciones { get; set; }

        public List<SerieRealizada> Series { get; set; } = new();
    }
}
