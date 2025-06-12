using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Extra
{
    public class SerieRealizada
    {
        public int Id { get; set; }
        public int EjercicioRealizadoId { get; set; }
        public EjercicioRealizado EjercicioRealizado { get; set; }

        public int NumeroSerie { get; set; }
        public int Repeticiones { get; set; }
        public float? PesoUtilizado { get; set; }
    }
}
