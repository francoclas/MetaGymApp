using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class SesionRutina
    {

        public SesionRutina() { }
        public int Id { get; set; }
        
        //Referencia Rutina original
        public int RutinaId { get; set; }
        public Rutina Rutina { get; set; }
       
        //Conexion cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        //Datos
        public DateTime FechaRealizada { get; set; }
        public int? DuracionMin { get; set; }

        //Ejercicios
        public List<EjercicioRealizado> EjericiosRealizados { get; set; }




    }
}
