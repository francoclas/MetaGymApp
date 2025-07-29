using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Clases
{
    public class TipoAtencion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int DuracionMin { get; set; } = 30;

        //relacion especialidad
        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }
        
        //relacion inversa de citas
        public List<Cita> Citas { get; set; }
        //relacion profesionales
        public List<Profesional> Profesionales { get; set; } = new List<Profesional>();

    }
}
