using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Excepciones
{
    public class EjercicioException : Exception
    {
        public EjercicioException(string mensaje) : base(mensaje) { }

    }
}
