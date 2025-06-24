using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Extra;
using LogicaNegocio.Interfaces;
using LogicaNegocio.Interfaces.Clases;

namespace LogicaNegocio.Clases 
{
    public class Cliente : IValidable
    {
        public int Id { get; set; }
        public string CI { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Pass { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public Cliente() { }

        public List<Cita> Citas { get; set; } = new List<Cita>();
        public List<Rutina> Rutinas { get; set; } = new List<Rutina>();
        public List<SesionRutina> SesionesEntrenadas { get; set; } = new List<SesionRutina> { };
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();

        //Constructor registro
        public Cliente(string ci, string nombreusuario, string nombreCompleto, string pass, string correo, string telefono)
        {
            this.NombreUsuario = nombreusuario;
            this.NombreCompleto = nombreCompleto;
            this.Pass = pass;
            this.Correo = correo;
            this.CI = ci;
            this.Telefono = telefono;
        }

        public void Validar()
        {
            FuncionesAuxiliares.ValidarDatosUsuario(this.CI, this.Pass, this.Correo);
        }
    }
}
