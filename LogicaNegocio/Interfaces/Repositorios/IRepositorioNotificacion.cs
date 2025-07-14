using LogicaNegocio.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Interfaces.Repositorios
{
    public interface IRepositorioNotificacion
    {
        void Crear(Notificacion n);
        void MarcarLeida(int id);
        List<Notificacion> ObtenerPorUsuario(int usuarioId);
    }
}
