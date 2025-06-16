using LogicaNegocio.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class EjercicioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string GrupoMuscular { get; set; }
        public string Instrucciones { get; set; }
        public Media? Media { get; set; }
    }
}
