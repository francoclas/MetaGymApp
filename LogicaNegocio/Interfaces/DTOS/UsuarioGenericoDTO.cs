using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class UsuarioGenericoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Pass {  get; set; }
        public string Telefono { get; set; }
        public Media Perfil { get; set; }
        public List<Especialidad> Especialidades { get; set; } = new List<Especialidad>();
        public string Rol { get; set; } // "Cliente", "Profesional", "Admin"
    }
}
