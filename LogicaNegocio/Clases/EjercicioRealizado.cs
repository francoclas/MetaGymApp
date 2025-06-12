using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Clases
{
    public class EjercicioRealizado
    {
        public int Id {  get; set; }

        //Conexion RutinaEjercicio
        public int RutinaEjercicioId {  get; set; }
        public SesionRutina RutinaEjercicio { get; set; }

        //Conexion Ejercicio
        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }

        //Informacoin 
        public List<SerieRealizada> Series { get; set; } = new();
        public string? Observaciones { get; set; }
        public bool SeRealizo { get; set; } = false;

    }
}
