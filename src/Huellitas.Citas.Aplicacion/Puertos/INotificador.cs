using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Aplicacion.Puertos;

public interface INotificador
{
    bool Notificar(Dueno dueno, string mensaje);
}
