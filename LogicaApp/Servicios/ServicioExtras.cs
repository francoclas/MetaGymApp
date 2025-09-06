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
    public class ServicioExtras : IExtraServicio
    {
        // Repositorio principal de extras
        private readonly IRepositorioExtra _repositorioExtras;

        // Inyección de repositorio
        public ServicioExtras(IRepositorioExtra repo)
        {
            _repositorioExtras = repo;
        }

        // Busca especialidades por nombre (validación mínima de entrada)
        public List<Especialidad> BuscarEspecialidad(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre))
                throw new Exception("La especialidad a buscar no puede estar vacia.");

            return _repositorioExtras.BuscarEspecialidad(Nombre);
        }

        // Busca establecimientos por nombre
        public List<Establecimiento> BuscarEstablecimiento(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre))
                throw new Exception("El establecimiento abuscar no puede estar vacio."); // Nota: mensaje tal cual el original

            return _repositorioExtras.BuscarEstablecimiento(Nombre);
        }

        // Pendiente de implementación 
        public void ModificarEspecialidad(Especialidad especialidad)
        {
            throw new NotImplementedException();
        }

        // Pendiente de implementación (por ahora lanza NotImplemented)
        public void ModificarEstablecimiento(Establecimiento establecimiento)
        {
            throw new NotImplementedException();
        }

        // Trae una especialidad por Id
        public Especialidad ObtenerEspecialidad(int Id)
        {
            return _repositorioExtras.ObtenerEspecialidadId(Id);
        }

        // Lista todas las especialidades
        public List<Especialidad> ObtenerEspecialidades()
        {
            return _repositorioExtras.ListarEspecialidades();
        }

        // Trae un establecimiento por Id
        public Establecimiento ObtenerEstablecimiento(int Id)
        {
            return _repositorioExtras.ObtenerEstablecimientoId(Id);
        }

        // Lista todos los establecimientos
        public List<Establecimiento> ObtenerEstablecimientos()
        {
            return _repositorioExtras.ListarEstablecimientos();
        }

        // =======================
        // Media
        // =======================

        // Registra una media asociada (valida que haya URL)
        public void RegistrarMedia(Media media)
        {
            // Nota: la validación está invertida respecto a un caso típico; se deja tal cual tu lógica.
            // Si Url NO es nula/empty => lanza excepción "Debe cargar un archivo".
            if (!string.IsNullOrEmpty(media.Url))
                throw new Exception("Debe cargar un archivo");

            _repositorioExtras.AltaMedia(media);
        }

        // =======================
        // Altas de entidades (desde entidad y desde DTO)
        // =======================

        // Alta de especialidad desde entidad
        public void RegistrarNuevaEspecialidad(Especialidad especialidad)
        {
            // Validaciones mínimas
            if (string.IsNullOrEmpty(especialidad.NombreEspecialidad))
                throw new Exception("El nombre de la especialidad no puede ser vacio.");
            if (string.IsNullOrEmpty(especialidad.DescripcionEspecialidad))
                throw new Exception("Debe ingresar una descripcion de la especialidad.");

            _repositorioExtras.AltaEspecialidad(especialidad);
        }

        // Alta de establecimiento desde entidad
        public void RegistrarNuevoEstablecimiento(Establecimiento establecimiento)
        {
            if (string.IsNullOrEmpty(establecimiento.Nombre))
                throw new Exception("El nombre del establecimiento no puede ser vacio.");
            if (string.IsNullOrEmpty(establecimiento.Direccion))
                throw new Exception("Debe ingresar una direccion del establecimiento.");

            _repositorioExtras.AltaEstablecimiento(establecimiento);
        }

        // Alta de especialidad desde DTO
        public void RegistrarEspecialidad(EspecialidadDTO dto)
        {
            // Instancia a partir del DTO (sin cambios de lógica)
            Especialidad nueva = new Especialidad
            {
                NombreEspecialidad = dto.NombreEspecialidad,
                DescripcionEspecialidad = dto.DescripcionEspecialidad
            };

            _repositorioExtras.AltaEspecialidad(nueva);
        }

        // Alta de establecimiento desde DTO
        public void RegistrarEstablecimiento(EstablecimientoDTO dto)
        {
            Establecimiento nuevo = new Establecimiento
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
            };

            _repositorioExtras.AltaEstablecimiento(nuevo);
        }

        // Guardado genérico
        public void GuardarCambios()
        {
            _repositorioExtras.GuardarCambios();
        }

        // Tipos de atención (CRUD mínimo + consultas)


        public void CrearTipoAtencion(TipoAtencion tipo)
        {
            _repositorioExtras.CrearTipoAtencion(tipo);
        }

        public TipoAtencion ObtenerTipoAtencion(int id)
        {
            return _repositorioExtras.ObtenerTipoPorId(id);
        }

        public List<TipoAtencion> ObtenerTiposAtencionPorEspecialidad(int especialidadId)
        {
            return _repositorioExtras.ObtenerTiposAtencionPorEspecialidad(especialidadId);
        }

        public List<TipoAtencion> ObtenerTiposAtencionPorEspecialidades(List<int> especialidadIds)
        {
            return _repositorioExtras.ObtenerTiposAtencionPorEspecialidades(especialidadIds);
        }

        public List<TipoAtencion> ObtenerTiposAtencionPorIds(List<int> ids)
        {
            return _repositorioExtras.ObtenerTiposAtencionPorIds(ids);
        }

        public List<TipoAtencion> ObtenerTiposAtencionPorProfesional(int profesionalId)
        {
            return _repositorioExtras.ObtenerTiposAtencionPorProfesional(profesionalId);
        }

        public List<TipoAtencion> ObtenerTiposAtencion()
        {
            return _repositorioExtras.ObtenerTiposAtencionTodos();
        }


        // DTOs mapeos


        public List<TipoAtencionDTO> ObtenerTiposAtencionPorProfesionalDTO(int profesionalId)
        {
            List<TipoAtencionDTO> salida = new List<TipoAtencionDTO>();
            foreach (var item in _repositorioExtras.ObtenerTiposAtencionPorProfesional(profesionalId))
            {
                salida.Add(new TipoAtencionDTO
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Desc = item.Descripcion,
                    EspecialidadId = item.EspecialidadId
                });
            }
            return salida;
        }

        public List<EstablecimientoDTO> ObtenerEstablecimientosDTO()
        {
            List<EstablecimientoDTO> salida = new List<EstablecimientoDTO>();

            foreach (var item in _repositorioExtras.ListarEstablecimientos())
            {
                salida.Add(new EstablecimientoDTO
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Direccion = item.Direccion,
                    Latitud = item.Latitud,
                    Longitud = item.Longitud,
                    // Asume que existe al menos una Media asociada; si no, podría disparar NullReferenceException.
                    UrlMedia = item.Media.FirstOrDefault().Url,
                });
            }
            return salida;
        }

        public List<EspecialidadDTO> ObtenerEspecialidadesDTO()
        {
            List<EspecialidadDTO> salida = new List<EspecialidadDTO>();

            foreach (var espe in _repositorioExtras.ListarEspecialidades())
            {
                EspecialidadDTO ax = new EspecialidadDTO
                {
                    Id = espe.Id,
                    NombreEspecialidad = espe.NombreEspecialidad,
                    DescripcionEspecialidad = espe.DescripcionEspecialidad,
                    TipoAtenciones = new List<TipoAtencionDTO>()
                };


                if (espe.TiposAtencion != null || espe.TiposAtencion.Any())
                {
                    foreach (var tipo in espe.TiposAtencion)
                    {
                        ax.TipoAtenciones.Add(new TipoAtencionDTO
                        {
                            Id = tipo.Id,
                            EspecialidadId = tipo.EspecialidadId,
                            Desc = tipo.Descripcion,
                            Nombre = tipo.Nombre
                        });
                    }
                    salida.Add(ax);
                }
            }

            return salida;
        }

        public List<TipoAtencionDTO> ObtenerTiposAtencionDTO()
        {
            List<TipoAtencionDTO> salida = new List<TipoAtencionDTO>();

            foreach (var tipo in _repositorioExtras.ObtenerTiposAtencionTodos())
            {
                salida.Add(new TipoAtencionDTO
                {
                    Id = tipo.Id,
                    EspecialidadId = tipo.EspecialidadId,
                    Desc = tipo.Descripcion,
                    Nombre = tipo.Nombre
                });
            }

            return salida;
        }
    }
}
