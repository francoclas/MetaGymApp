using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Excepciones
{
    public class PublicacionException : Exception
    {
        public PublicacionException(string mensaje) : base(mensaje) { }
    }
}
