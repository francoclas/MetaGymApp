using APIClienteMetaGym.DTO;
using LogicaNegocio.Interfaces.DTOS;

namespace APIClienteMetaGym.Extra
{
    public class MapeadorUsuario
    {
        public ClienteDTOAPI MapearCiente(UsuarioGenericoDTO usuario)
        {
            return new ClienteDTOAPI 
            {
                Id = usuario.Id,
                Ci = usuario.Ci,
                NombreUsuario = usuario.Usuario,
                NombreCompleto = usuario.Nombre,
                Telefono = usuario.Telefono,
                Correo = usuario.Correo,
                ImagenPerfilURL = usuario.Perfil.Url,
            };

        }
    }
}
