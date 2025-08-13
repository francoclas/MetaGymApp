using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class LikePublicacion
    {
        public int Id { get; set; }

        public int PublicacionId { get; set; }
        public Publicacion Publicacion { get; set; }

        public int UsuarioId { get; set; }
        public string TipoUsuario {  get; set; }
        public DateTime Fecha { get; set; }
    }

}
