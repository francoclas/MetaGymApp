using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaApp.DTOS
{
    public class CitaDTO
    {
        public CitaDTO() { }
        public int CitaId {  get; set; }
        public int ClienteId {  get; set; }
        public Cliente Cliente { get; set; }
        public EstadoCita Estado { get; set; }
        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }
        public int? TipoAtencionId { get; set; }
        public TipoAtencion TipoAtencion { get; set; }
        public int EstablecimientoId { get; set; }
        public Establecimiento Establecimiento { get; set; }
        public string? Descripcion {  get; set; }
        public DateTime FechaAsistencia { get; set; }

        public DateTime? FechaCreacion { get; set; }
        public int? ProfesionalId { get; set; }
        public string? Conclusion { get; set; }
    }
}
