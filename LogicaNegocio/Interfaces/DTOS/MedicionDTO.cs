using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class MedicionDTO
    {
        public int MedicionId { get; set; }
        public string Nombre { get; set; }
        public string Desc { get; set; }
        public string Unidad { get; set; }
        public string Valor { get; set; }
    }
}
