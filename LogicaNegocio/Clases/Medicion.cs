using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class Medicion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public bool EsObligatoria { get; set; }

        // Relación con el ejercicio
        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }
    }
}
