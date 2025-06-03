using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.Clases
{
    public interface IComparable<T>
    {
        int CompareTo(T otro);
    }
}
