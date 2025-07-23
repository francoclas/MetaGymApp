using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class Establecimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public List<Media> Media { get; set; }
        //Relacion de citas en ese establecimiento

        public List<Cita> Citas { get; set; } = new List<Cita>();

        public Establecimiento() { }
        public Establecimiento(string Nombre,string Direccion)
        {
            this.Nombre = Nombre;
            this.Direccion = Direccion;
        }
    }
}
