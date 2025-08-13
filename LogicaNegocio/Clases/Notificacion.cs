using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class Notificacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leido { get; set; }
        //Enum clasificacion
        public Enum_TipoNotificacion Tipo {  get; set; }
        // Asociación con entidades relacionadas
        public int? RutinaId { get; set; }
        public int? CitaId { get; set; }
        public int? PublicacionId { get; set; }
        public int? ComentarioId { get; set; }
        //Usuario
        //Conexion cliente
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        //Conexion Profesional
        public int? ProfesionalId { get; set; }
        public Profesional? Profesional { get; set; }
        //Conexoin admin
        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }
    }
}
