using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaNegocio.Interfaces.Servicios;

namespace LogicaApp.Servicios
{
    public class ServicioAgenda : IAgendaServicio
    {
        private readonly IRepositorioAgenda _repositorioAgenda;

        // Inyecta el repositorio de Agenda 
        public ServicioAgenda(IRepositorioAgenda repo)
        {
            _repositorioAgenda = repo;
        }

        // Alta de una agenda (jornada laboral) validando solapamientos por día y rango horario.
        public void RegistrarAgenda(AgendaProfesional agenda)
        {
            // Chequeo mismo profesional, mismo día y un horario que se pise con uno existente no se permite.
            if (_repositorioAgenda.ExisteAgendaEnHorario(agenda.ProfesionalId, agenda.Dia, agenda.HoraInicio, agenda.HoraFin))
                throw new Exception("Ya existe una agenda en ese horario para ese día.");

            // Si no hay solapamiento, registro la agenda.
            _repositorioAgenda.RegistrarAgenda(agenda);
        }

        // Todas las agendas (jornadas) del profesional.
        public List<AgendaProfesional> ObtenerAgendaDelProfesional(int profesionalId)
        {
            return _repositorioAgenda.ObtenerAgendaDelProfesional(profesionalId);
        }

        // Baja de una agenda por Id (si existe).
        public void EliminarAgenda(int agendaId)
        {
            _repositorioAgenda.EliminarAgenda(agendaId);
        }

        // Trae una agenda puntual por Id (o null si no existe).
        public AgendaProfesional ObtenerPorId(int id)
        {
            return _repositorioAgenda.ObtenerPorId(id);
        }

        // Actualizar directo de una agenda ya existente.
        public void ActualizarAgenda(AgendaProfesional agenda)
        {
            _repositorioAgenda.ActualizarAgenda(agenda);
        }

        // Funcion extra para validar si ya hay una agenda que se pisa con el rango dado.
        public bool ExisteAgendaEnHorario(int profesionalId, Enum_DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return _repositorioAgenda.ExisteAgendaEnHorario(profesionalId, dia, horaInicio, horaFin);
        }

        // Devuelve todas las agendas del profesional en un día concreto.
        public List<AgendaProfesional> BuscarAgendaPorDia(int profesionalId, Enum_DiaSemana dia)
        {
            return _repositorioAgenda.BuscarAgendaPorDia(profesionalId, dia);
        }
    }
}