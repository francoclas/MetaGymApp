using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Interfaces.DTOS;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IProfesionalServicio
    { 
        //Funciones basicas
            void RegistrarProfesional(Profesional profesional);
            void AgregarEspecialidad (Especialidad especialidad, Profesional profesional);
            void AsignarTiposAtencion(int profesionalId, List<int> tipoAtencionIds);
            void AgregarTipoAtencion(TipoAtencion tipo, Profesional profesional);
            void EliminarTipoAtencion(int profesionalId, int tipoAtencionId);
            void EliminarEspecialidad(Especialidad especialidad, Profesional profesional);
            List<int> ObtenerEspecialidadesProfesional(int profesionalId);
            List<Profesional> ObtenerTodos();
            Profesional ObtenerProfesional(int id);
            void ActualizarProfesional(Profesional profesional);
        //Relacionado a gestion de citas
            //Generacion
            void GenerarCita(Cita cita);
            void RechazarCita(Cita cita);

           
        //Relacionado a publicacion
            void EnviarPublicacion (Publicacion publicacion);
    }
}
