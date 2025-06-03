using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Extra
{
    public enum EstadoCita
    {
        EnEspera,          // 1 Cuando se registra una nueva cita y esta en espera de ser aprobada/rechazada
        Aceptada,          // 2 Profesinal acepta la cita, y agenda el resto de detalles
        Rechazada,         // 3 Profesional rechaza la cita y define motivo
        Cancelada,         // 4 Puede ser cancelada por el cliente o por el profesional
        Finalizada,        // 5 Se realiza cita correctamente
        NoAsistio          // 6 Cliente no asiste a cita
    }
}
