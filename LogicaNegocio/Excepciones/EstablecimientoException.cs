using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Excepciones
{
    public class EstablecimientoException : Exception
    {
        public EstablecimientoException(string mensaje) : base(mensaje) { }

    }
}
