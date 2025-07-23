using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class ValorMedicion
    {
        public int Id { get; set; }

        public int EjercicioRealizadoId { get; set; }
        public EjercicioRealizado EjercicioRealizado { get; set; }

        public int MedicionId { get; set; }
        public Medicion Medicion { get; set; }

        public string Valor { get; set; }
    }
}
