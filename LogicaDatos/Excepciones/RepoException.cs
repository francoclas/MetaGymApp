using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDatos.Excepciones
{
    public class RepoException : Exception
    {
    public RepoException(string mensaje) : base(mensaje) { }
    }
}
