using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.DTOS
{
    public class TipoAtencionDTO
    {
        public int Id { get; set; }
        public int EspecialidadId { get; set; }
        public string Nombre { get; set; }
        public string Desc { get; set; }
    }
}
