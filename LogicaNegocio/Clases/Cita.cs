using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Excepciones;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.Clases;

namespace LogicaNegocio.Clases
{
    public class Cita: Interfaces.Clases.IComparable<Cita>,IValidable
    {
        public int Id { get; set; }

        //Profesional
        public int? ProfesionalId { get; set; }
        public Profesional? Profesional { get; set; }

        //Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        //Relaciones Especialidad
        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }
        //Relaciones Establecimiento
        public int EstablecimientoId { get; set; }
        public Establecimiento Establecimiento { get; set; }
        public EstadoCita Estado { get; set; } = EstadoCita.EnEspera;
        public string Descripcion { get; set; } = "";
        public string? Conclusion { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAsistencia { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public Cita() { }

        public Cita (int ClienteId,
            Especialidad especialidad,
            Establecimiento establecimiento,
            string descripcion,DateTime fechaAsistencia)
        {
            this.ClienteId = ClienteId;
            this.FechaCreacion = DateTime.Now;
            this.Especialidad = especialidad;
            this.Establecimiento = establecimiento;
            this.Descripcion = descripcion;
            this.FechaAsistencia = fechaAsistencia;
        }
        
        //Funciones
        /*
         Flujo de citas

         -> En espera: Cuando se registra una nueva cita, y esta en espera de ser aprobada, es el estado inicial de las citas.

         -> Aceptada: La cita fue aceptada por un profesional y esta en espera de ser efectuada en la fecha y establecimiento indicados. 

         -> Rechazada: La cita fue rechazada por un profesional, se plantea como conclusion el motivo de porque se rechazo, para en todo caso poder
                       coordinar para otra cita.
         -> Cancelada: Se cancela la cita por parte del cliente o del profesional, justificando en la conclusion el motivo.

         -> Finalizada: Se asiste a la cita y se realiza todo sin complicaciones, se agrega a la conclusion el trabajo realizado.

         -> NoAsistio: Caso particular donde el cliente no logro cumplir con la asistencia en la cita.

         */

        public void AceptarCita(Profesional profesional,DateTime fechaAsistencia)
        {
            //Verifico datos

            //Cambio estado
        }

        public void AsignarEstablecimiento(Establecimiento E) {
            if (Establecimiento != null)
            {
                this.Establecimiento = E;
            }
            throw new CitaException("El establecimiento no puede ser null");
        }

        public void CancelarCita (string MotivoCancelacion) { 
            if (Estado != EstadoCita.EnEspera || Estado != EstadoCita.Aceptada)
            {
                throw new CitaException("La cita no se puede cancelar si no esta en espera, o no fue aceptada.");
            }
            //Se agrega el motivo de cancelacion a la conclusion
            this.Conclusion = MotivoCancelacion;
            this.Estado = EstadoCita.Cancelada;
        }
        public void FinalizarCita(string conclusion) { 
            if (Estado != EstadoCita.Aceptada)
            {
                throw new CitaException("La cita no se puede finalizar porque no esta en estado aceptada.");
            }
            if (string.IsNullOrEmpty(conclusion)) { 
                throw new CitaException("Debe ingresar una conclusion descriptiva a la cita.");
            }
            this.Conclusion = conclusion;
            this.Estado = EstadoCita.Finalizada;
        }
        public void RechazarCita(string MotivoRechazo)
        {
            if (Estado != EstadoCita.EnEspera)
            {
                throw new CitaException("La cita no se puede cancelar si no esta en espera, o no fue aceptada.");
            }
            if (string.IsNullOrEmpty(MotivoRechazo))
            {
                throw new CitaException("Debe ingresar un motivo del rechazo descriptiva a la cita.");
            }
            //Se agrega el motivo de rechazo a la conclusion
            this.Conclusion = MotivoRechazo;
            this.Estado = EstadoCita.Rechazada;
        }
        public void ClienteNoAsiste()
        {
            if (Estado != EstadoCita.Aceptada)
            {
                throw new CitaException("La cita no se puede finalizar porque no esta en estado aceptada.");
            }
            this.Conclusion = "Cliente " + this.Cliente.NombreUsuario + " no logro asistir a la cita.";
            this.Estado = EstadoCita.NoAsistio;
        }
        public int CompareTo(Cita? cita)
        {
            return this.Id.CompareTo(cita.Id);
        }

        public void Validar()
        {
            if (string.IsNullOrEmpty(this.Descripcion))
            {
                throw new CitaException("La descripcion no puede estar vacia.");
            }
        }
    }
}
