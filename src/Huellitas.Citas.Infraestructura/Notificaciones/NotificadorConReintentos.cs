using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Infraestructura.Notificaciones;

public sealed class NotificadorConReintentos : INotificador
{
    private readonly INotificador _interno;
    private readonly int _intentos;

    public NotificadorConReintentos(INotificador interno, int intentos)
    {
        if (intentos < 1) throw new ArgumentOutOfRangeException(nameof(intentos), "Debe haber al menos un intento.");

        _interno = interno;
        _intentos = intentos;
    }

    public bool Notificar(Dueno dueno, string mensaje)
    {
        for (var intento = 1; intento <= _intentos; intento++)
            if (_interno.Notificar(dueno, mensaje)) return true;

        return false;
    }
}
