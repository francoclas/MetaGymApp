using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Clases
{
    public class Media
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? Descripcion { get; set; }
        public Enum_TipoMedia Tipo { get; set; }
        //Conexion Publicacion
        public int? PublicacionId { get; set; }
        public Publicacion? Publicacion { get; set; }
        //Conexion Ejercicio
        public int? EjercicioId { get; set; }
        public Ejercicio? Ejercicio { get; set; }
        //Conexion Establecimiento
        public int? EstablecimientoId { get; set; }
        public Establecimiento Establecimiento { get;set; }

        public Media() { }
    }
}
