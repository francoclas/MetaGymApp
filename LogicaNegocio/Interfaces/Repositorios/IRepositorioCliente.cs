using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaDatos.Interfaces.Repos
{
    public interface IRepositorioCliente : IRepository<Cliente>
    {
        bool ExisteUsuario(string usuario);
        
        Cliente VerificarCredenciales(string usuario,string pass);
        List<Cliente> BuscarClienteNombre(string Nombre);
        List<Cliente> BuscarClienteCI(string CI);
        //Citas
        List<Cita> ObtenerHistorial(int ClienteID);
        List<Cita> ObtenerCitasPorEstado(int ClienteID,EstadoCita Estado);
        List<Cita> ObtenerCitasEntreFechas(int ClienteID,DateTime FechaInicial,DateTime FechaFinal);
        //Rutinas




    }
}
