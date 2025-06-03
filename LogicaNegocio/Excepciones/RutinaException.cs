using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Excepciones
{
    public class RutinaException : Exception
    {
        public RutinaException(string mensaje) : base(mensaje) { }

    }
}
