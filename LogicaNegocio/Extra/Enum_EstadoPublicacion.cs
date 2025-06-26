using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Extra
{
    public enum Enum_EstadoPublicacion
    {
        Pendiente = 0,     // Creada por el profesional, esperando aprobación
        Aprobada = 1,      // Visible para todos
        Rechazada = 2,     // Rechazada por el admin
        Programada = 3     // A publicar en una fecha futura}
    }
}
