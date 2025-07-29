using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.EntityFrameworkCore;

namespace LogicaApp.Servicios
{
    public class ServicioProfesional : IProfesionalServicio

    {
        private readonly IRepositorioProfesional _repoProfesional;
        private readonly IExtraServicio _extraServicio;
        public ServicioProfesional (IRepositorioProfesional repoProfesional, IExtraServicio extraServicio)
        {
            _repoProfesional = repoProfesional;
            _extraServicio = extraServicio;
        }
        public void ActualizarProfesional(Profesional profesional)
        {
            //Mando el nuevo profesional a la base
            _repoProfesional.Actualizar(profesional);
        }
        public void AgregarEspecialidad(Especialidad especialidad, Profesional profesional)
        {
            //Verifico que no tenga la especialidad
            if (!profesional.Especialidades.Any(e => e.Id == especialidad.Id))
            {
                //La agrego y actualizo
                profesional.Especialidades.Add(especialidad);
                _repoProfesional.Actualizar(profesional);
            }
        }
        public void EliminarEspecialidad(Especialidad especialidad, Profesional profesional)
        {
            //Obtengo la especialidad
            var existente = profesional.Especialidades.FirstOrDefault(e => e.Id == especialidad.Id);
            //Verifico que la tenga asignada
            if (existente != null)
            {
                //La elimino
                profesional.Especialidades.Remove(existente);
                _repoProfesional.Actualizar(profesional);
            }
        }

        public void EnviarPublicacion(Publicacion publicacion)
        {
            throw new NotImplementedException();
        }

        public void GenerarCita(Cita cita)
        {
            throw new NotImplementedException();
        }

        public List<int> ObtenerEspecialidadesProfesional(int profesionalId)
        {
            //Obtengo pro
            Profesional pro = _repoProfesional.ObtenerPorId(profesionalId);
            return pro.Especialidades.Select(x => x.Id).ToList();
        }

        public Profesional ObtenerProfesional(int id)
        {
            return _repoProfesional.ObtenerPorId(id);
        }

        public List<Profesional> ObtenerTodos()
        {
            return _repoProfesional.ObtenerTodos().ToList();
        }

        public void RechazarCita(Cita cita)
        {
            throw new NotImplementedException();
        }

        public void RegistrarProfesional(Profesional profesional)
        {
            throw new NotImplementedException();
        }
        public void AsignarTiposAtencion(int profesionalId, List<int> tipoAtencionIds)
        {
            var profesional = ObtenerProfesional(profesionalId);
            if (profesional == null) throw new Exception("Profesional no encontrado.");

            var tipos = _extraServicio.ObtenerTiposAtencionPorIds(tipoAtencionIds);
            profesional.TiposAtencion = tipos;

            _repoProfesional.Actualizar(profesional);
        }
        public void AgregarTipoAtencion(TipoAtencion tipo, Profesional profesional)
        {
            if (!profesional.TiposAtencion.Contains(tipo))
            {
                profesional.TiposAtencion.Add(tipo);
                _repoProfesional.Actualizar(profesional);
            }
        }
        public void EliminarTipoAtencion(int profesionalId, int tipoAtencionId)
        {
            var profesional = ObtenerProfesional(profesionalId);
            if (profesional == null) throw new Exception("Profesional no encontrado.");

            var tipo = profesional.TiposAtencion.FirstOrDefault(t => t.Id == tipoAtencionId);
            if (tipo != null)
            {
                profesional.TiposAtencion.Remove(tipo);
                _repoProfesional.Actualizar(profesional);
            }
        }
    }
}
