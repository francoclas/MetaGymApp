using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;
using LogicaNegocio.Interfaces.Servicios;
using Microsoft.EntityFrameworkCore;

namespace LogicaApp.Servicios
{
    public class ServicioProfesional : IProfesionalServicio

    {
        private readonly IRepositorioProfesional _repositorioProfesional;
        private readonly IExtraServicio _extraServicio;
        public ServicioProfesional (IRepositorioProfesional repoProfesional, IExtraServicio extraServicio)
        {
            _repositorioProfesional = repoProfesional;
            _extraServicio = extraServicio;
        }
        public void ActualizarProfesional(Profesional profesional)
        {
            //Mando el nuevo profesional a la base
            _repositorioProfesional.Actualizar(profesional);
        }
        public void AgregarEspecialidad(Especialidad especialidad, Profesional profesional)
        {
            //Verifico que no tenga la especialidad
            if (!profesional.Especialidades.Any(e => e.Id == especialidad.Id))
            {
                //La agrego y actualizo
                profesional.Especialidades.Add(especialidad);
                _repositorioProfesional.Actualizar(profesional);
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
                _repositorioProfesional.Actualizar(profesional);
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
            //Obtengo profesional
            Profesional pro = _repositorioProfesional.ObtenerPorId(profesionalId);
            // mando las ids
            return pro.Especialidades.Select(x => x.Id).ToList();
        }

        public Profesional ObtenerProfesional(int id)
        {
            return _repositorioProfesional.ObtenerPorId(id);
        }

        public List<Profesional> ObtenerTodos()
        {
            return _repositorioProfesional.ObtenerTodos().ToList();
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

            _repositorioProfesional.Actualizar(profesional);
        }
        public void AgregarTipoAtencion(TipoAtencion tipo, Profesional profesional)
        {
            if (!profesional.TiposAtencion.Contains(tipo))
            {
                profesional.TiposAtencion.Add(tipo);
                _repositorioProfesional.Actualizar(profesional);
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
                _repositorioProfesional.Actualizar(profesional);
            }
        }
        public List<int> ObtenerTiposAtencionProfesional(int profesionalId)
        {
            Profesional profe = _repositorioProfesional.ObtenerPorId(profesionalId);

            if (profe == null)
                throw new Exception("No se encontró el profesional.");

            return profe.TiposAtencion.Select(ta => ta.Id).ToList();
        }

        public List<EspecialidadDTO> ObtenerEspecialidadesProfesionalDTO(int profesionalId)
        {
            Profesional profe = _repositorioProfesional.ObtenerPorId(profesionalId);

            List<EspecialidadDTO> salida = new List<EspecialidadDTO>();
            foreach (var item in profe.Especialidades)
            {
                EspecialidadDTO aux = new EspecialidadDTO
                {
                    Id = item.Id,
                    DescripcionEspecialidad = item.DescripcionEspecialidad,
                    NombreEspecialidad = item.NombreEspecialidad,
                    TipoAtenciones = new List<TipoAtencionDTO>()
                };
                if(item.TiposAtencion != null)
                {
                    foreach (TipoAtencion tp in item.TiposAtencion)
                    {
                        aux.TipoAtenciones.Add(new TipoAtencionDTO
                        {
                            EspecialidadId = aux.Id,
                            Desc = tp.Descripcion,
                            Nombre = tp.Nombre,
                            Id = tp.Id
                        });
                    }
                }
                
                salida.Add(aux);
            }
            return salida;
        }
    }
}
