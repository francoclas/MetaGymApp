using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS.API
{
    public class RegistroUsuarioDTO
    {
        public string Correo {  get; set; }
        public string Usuario { get; set;}
        public string Pass { get; set; }
        public string Confirmacion { get; set; }

    }
}
