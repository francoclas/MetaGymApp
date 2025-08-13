using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Clases
{
    public class AgendaProfesional
    {
        public int Id { get; set; }

        public int ProfesionalId { get; set; }
        public Profesional Profesional { get; set; }
        public Enum_DiaSemana Dia { get; set; } 
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public bool Activo { get; set; } = true;
    }
}
