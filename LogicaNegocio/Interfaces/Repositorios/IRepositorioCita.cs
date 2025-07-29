using LogicaNegocio.Clases;
using LogicaNegocio.Extra;

namespace LogicaDatos.Interfaces.Repos
{
    public interface IRepositorioCita : IRepository<Cita>
    {
        
        public bool ExisteCita(Cita cita);
        //Consultas
        public List<Cita> ObtenerPorCliente(int clienteId);
        public List<Cita> ObtenerPorProfesional(int profesionalId);
        public List<Cita> ObtenerHabilitadasParaProfesional(int profesionalid);
        public List<Cita> ObtenerPorEstado(EstadoCita estado);
        public List<Cita> ObtenerEntreFechas(DateTime desde, DateTime hasta);
        public List<Cita> ObtenerPorClienteYEstado(int clienteId, EstadoCita estado);
        public List<Cita> ObtenerPorProfesionalYEstado(int profesionalId, EstadoCita estado);
        public List<Cita> BuscarPorTextoDescripcion(string texto);
        public List<Cita> BuscarPorTextoConclusion(string texto);
        public List<Cita> BuscarPorEstado(EstadoCita estado);

    }
}
