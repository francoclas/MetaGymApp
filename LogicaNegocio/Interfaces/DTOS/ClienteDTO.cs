using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class ClienteDTO 
    {
        public string Ci {  get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Password { get; set; }
        public string ConfPass { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public ClienteDTO() { }
       
    }
}
