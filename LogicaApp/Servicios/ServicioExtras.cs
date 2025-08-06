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
        private readonly IRepositorioExtra repoExtras;
        public ServicioExtras(IRepositorioExtra repo)
        {
            repoExtras = repo;
        }

        public List<Especialidad> BuscarEspecialidad(string Nombre)
        {
            //Valido especialidad
            if (string.IsNullOrEmpty(Nombre))
            {
                throw new Exception("La especialidad a buscar no puede estar vacia.");
            }
            //Devuelvo desde repo
            return repoExtras.BuscarEspecialidad(Nombre);
        }

        public List<Establecimiento> BuscarEstablecimiento(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                throw new Exception("El establecimiento abuscar no puede estar vacio.");
            }
            return repoExtras.BuscarEstablecimiento(Nombre);
        }

        public void ModificarEspecialidad(Especialidad especialidad)
        {
            throw new NotImplementedException();
        }

        public void ModificarEstablecimiento(Establecimiento establecimiento)
        {
            throw new NotImplementedException();
        }

        public Especialidad ObtenerEspecialidad(int Id)
        {
            return repoExtras.ObtenerEspecialidadId(Id);
        }

        public List<Especialidad> ObtenerEspecialidades()
        {
            return repoExtras.ListarEspecialidades();
        }

        public Establecimiento ObtenerEstablecimiento(int Id)
        {
            
            return repoExtras.ObtenerEstablecimientoId(Id);
        }

        public List<Establecimiento> ObtenerEstablecimientos()
        {
            return repoExtras.ListarEstablecimientos();
        }

        public void RegistrarMedia(Media media)
        {
            //Valido datos
            if (!string.IsNullOrEmpty(media.Url))
            {
                throw new Exception("Debe cargar un archivo");
            }
            //Mando a sistema
            repoExtras.AltaMedia(media);
        }

        public void RegistrarNuevaEspecialidad(Especialidad especialidad)
        {
            //Valido datos
            if (string.IsNullOrEmpty(especialidad.NombreEspecialidad))
            {
                throw new Exception("El nombre de la especialidad no puede ser vacio.");
            }
            if (string.IsNullOrEmpty(especialidad.DescripcionEspecialidad))
            {
                throw new Exception("Debe ingresar una descripcion de la especialidad.");
            }
            //Envio al repositorio
            repoExtras.AltaEspecialidad(especialidad);
        }

        public void RegistrarNuevoEstablecimiento(Establecimiento establecimiento)
        {
            //Valido datos
            if (string.IsNullOrEmpty(establecimiento.Nombre))
            {
                throw new Exception("El nombre del establecimiento no puede ser vacio.");
            }
            if (string.IsNullOrEmpty(establecimiento.Direccion))
            {
                throw new Exception("Debe ingresar una direccion del establecimiento.");
            }
            //Envio al repositorio
            repoExtras.AltaEstablecimiento(establecimiento);
        }
        //registros solo con dtos
        public void RegistrarEspecialidad(EspecialidadDTO dto)
        {
            //Instancio especialidad desde el dto
            Especialidad nueva = new Especialidad
            {
                NombreEspecialidad = dto.NombreEspecialidad,
                DescripcionEspecialidad = dto.DescripcionEspecialidad
            };
            //Mando al repo
            repoExtras.AltaEspecialidad(nueva);
         
        }

        public void RegistrarEstablecimiento(EstablecimientoDTO dto)
        {
            //Instancio nuevo establecimiento desde DTO
            Establecimiento nuevo = new Establecimiento
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
            };
            //Mando al repo
                repoExtras.AltaEstablecimiento(nuevo);
        }

        public void GuardarCambios()
        {
            repoExtras.GuardarCambios();
        }
        public void CrearTipoAtencion(TipoAtencion tipo)
        {
            repoExtras.CrearTipoAtencion(tipo);
        }
        public TipoAtencion ObtenerTipoAtencion(int id)
        {
            return repoExtras.ObtenerTipoPorId(id);
        }
        public List<TipoAtencion> ObtenerTiposAtencionPorEspecialidad(int especialidadId)
        {
            return repoExtras.ObtenerTiposAtencionPorEspecialidad(especialidadId);
        }
        public List<TipoAtencion> ObtenerTiposAtencionPorEspecialidades(List<int> especialidadIds)
        {
            return repoExtras.ObtenerTiposAtencionPorEspecialidades(especialidadIds);
        }

        public List<TipoAtencion> ObtenerTiposAtencionPorIds(List<int> ids)
        {
            return repoExtras.ObtenerTiposAtencionPorIds(ids);
        }
        public List<TipoAtencion> ObtenerTiposAtencionPorProfesional(int profesionalId)
        {
            return repoExtras.ObtenerTiposAtencionPorProfesional(profesionalId);
        }
        public List<TipoAtencion> ObtenerTiposAtencion()
        {
            return repoExtras.ObtenerTiposAtencionTodos();
        }

        public List<TipoAtencionDTO> ObtenerTiposAtencionPorProfesionalDTO(int profesionalId)
        {
            List<TipoAtencionDTO> salida = new List<TipoAtencionDTO> ();
            foreach (var item in repoExtras.ObtenerTiposAtencionPorProfesional(profesionalId))
            {
                salida.Add(new TipoAtencionDTO
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Desc = item.Descripcion,
                });
            }
            return salida;
        }

        public List<EstablecimientoDTO> ObtenerEstablecimientosDTO()
        {
            List<EstablecimientoDTO> salida = new List<EstablecimientoDTO>();
            foreach (var item in repoExtras.ListarEstablecimientos()) {
                salida.Add(new EstablecimientoDTO
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Direccion = item.Direccion,
                    Latitud = item.Latitud,
                    Longitud = item.Longitud,
                    UrlMedia = item.Media.FirstOrDefault().Url,
                });
            }
            return salida;
        }

        public List<EspecialidadDTO> ObtenerEspecialidadesDTO()
        {
            List<EspecialidadDTO> salida = new List<EspecialidadDTO>();
            foreach (var espe in repoExtras.ListarEspecialidades())
            {
                EspecialidadDTO ax = new EspecialidadDTO
                {
                    Id = espe.Id,
                    NombreEspecialidad = espe.NombreEspecialidad,
                    DescripcionEspecialidad = espe.DescripcionEspecialidad,
                    TipoAtenciones = new List<TipoAtencionDTO>()
                };
                if(espe.TiposAtencion != null || espe.TiposAtencion.Any())
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
            List<TipoAtencionDTO> salida = new List<TipoAtencionDTO> ();
            foreach (var tipo in repoExtras.ObtenerTiposAtencionTodos())
            {
                salida.Add(
                    new TipoAtencionDTO
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
