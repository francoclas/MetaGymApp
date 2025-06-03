using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Excepciones
{
    public class CitaException : Exception
    {
        public CitaException(string mensaje) : base(mensaje) { }

    }
}
