using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Excepciones
{
    public class EspecialidadException : Exception
    {
        public EspecialidadException(string mensaje) : base(mensaje) { }

    }
}
