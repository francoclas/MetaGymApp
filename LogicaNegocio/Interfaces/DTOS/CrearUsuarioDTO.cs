using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class CrearUsuarioDTO
    {
        public string Ci { get; set; }
        public string Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
    }
}
