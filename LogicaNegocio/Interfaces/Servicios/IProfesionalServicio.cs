using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;

namespace LogicaNegocio.Interfaces.Servicios
{
    public interface IProfesionalServicio
    {
        //Funciones basicas
            void RegistrarProfesional(Profesional profesional);
            void AgregarEspecialidad (Especialidad especialidad, Profesional profesional);
            void EliminarEspecialidad(Especialidad especialidad, Profesional profesional);

        //Relacionado a gestion de citas
            //Generacion
            void GenerarCita(Cita cita);
            void RechazarCita(Cita cita);

           
        //Relacionado a publicacion
            void EnviarPublicacion (Publicacion publicacion);
            

    }
}
