using APIClienteMetaGym.DTO;
using LogicaNegocio.Interfaces.DTOS;

namespace APIClienteMetaGym.Extra
{
    public class MapeadorUsuario
    {
        public ClienteDTOAPI MapearCiente(UsuarioGenericoDTO usuario)
        {
            ClienteDTOAPI salida = new ClienteDTOAPI {
                Id = usuario.Id,
                Ci = usuario.Ci,
                NombreUsuario = usuario.Usuario,
                NombreCompleto = usuario.Nombre,
                Telefono = usuario.Telefono,
                Correo = usuario.Correo
            };
            if (usuario.Perfil != null)
                salida.ImagenPerfilURL = usuario.Perfil.Url;
            else
                salida.ImagenPerfilURL = "/mediaweb/default/perfil_default.jpg";
                return salida;

        }
    }
}
