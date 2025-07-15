using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class LikeComentario
    {
        public int Id { get; set; }

        public int ComentarioId { get; set; }
        public Comentario Comentario { get; set; }

        public int UsuarioId { get; set; }
        public string TipoUsuario { get; set; }
        public DateTime Fecha { get; set; }
    }

}
