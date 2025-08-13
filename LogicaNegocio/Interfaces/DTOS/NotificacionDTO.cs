using LogicaNegocio.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class NotificacionDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leido { get; set; }
        public Enum_TipoNotificacion Tipo { get; set; }
        public int UsuarioId {  get; set; }
        public string RolUsuario { get; set; }

        public int? RutinaId { get; set; }
        public int? CitaId { get; set; }
        public int? PublicacionId { get; set; }
        public int? ComentarioId { get; set; }
    }
}
