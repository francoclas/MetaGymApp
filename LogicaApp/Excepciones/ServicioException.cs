using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaApp.Excepciones
{
    public class ServicioException : Exception
    {
        public ServicioException(string mensaje) : base(mensaje) { }

    }
}
