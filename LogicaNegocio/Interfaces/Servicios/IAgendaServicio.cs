using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IAgendaServicio
    {
        void RegistrarAgenda(AgendaProfesional agenda);
        List<AgendaProfesional> ObtenerAgendaDelProfesional(int profesionalId);
        void EliminarAgenda(int agendaId);
        AgendaProfesional ObtenerPorId(int id);
        void ActualizarAgenda(AgendaProfesional agenda);
        bool ExisteAgendaEnHorario(int profesionalId, Enum_DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin);
        List<AgendaProfesional> BuscarAgendaPorDia(int profesionalId, Enum_DiaSemana dia);
    }
}
