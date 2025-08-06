using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IExtraServicio
    {
        void RegistrarEspecialidad(EspecialidadDTO dto);
        void RegistrarEstablecimiento(EstablecimientoDTO dto);
        void RegistrarNuevaEspecialidad(Especialidad especialidad);
        void ModificarEspecialidad(Especialidad especialidad);
        void RegistrarNuevoEstablecimiento(Establecimiento establecimiento);
        void ModificarEstablecimiento(Establecimiento establecimiento);
        List<Establecimiento> BuscarEstablecimiento(string Nombre);
        List<Especialidad> BuscarEspecialidad(string Nombre);
        Establecimiento ObtenerEstablecimiento(int Id);
        Especialidad ObtenerEspecialidad(int Id);
        List<Establecimiento> ObtenerEstablecimientos();
        List<Especialidad> ObtenerEspecialidades();
        void RegistrarMedia(Media media);
        public void GuardarCambios();
        void CrearTipoAtencion(TipoAtencion tipo);
        public TipoAtencion ObtenerTipoAtencion(int id);
        List<TipoAtencion> ObtenerTiposAtencionPorEspecialidad(int especialidadId);
        List<TipoAtencion> ObtenerTiposAtencionPorEspecialidades(List<int> especialidadIds);
        List<TipoAtencion> ObtenerTiposAtencionPorIds(List<int> ids);
        List<TipoAtencion> ObtenerTiposAtencionPorProfesional(int profesionalId);
        List<TipoAtencion> ObtenerTiposAtencion();
        List<TipoAtencionDTO> ObtenerTiposAtencionPorProfesionalDTO(int profesionalId);
        List<EstablecimientoDTO> ObtenerEstablecimientosDTO();
        List<EspecialidadDTO> ObtenerEspecialidadesDTO();
        List<TipoAtencionDTO> ObtenerTiposAtencionDTO();
    }
}
