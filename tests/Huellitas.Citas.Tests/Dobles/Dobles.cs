using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Tests.Dobles;

internal sealed class RelojFijo(DateTime ahora) : IReloj
{
    public DateTime Ahora { get; } = ahora;
}

internal sealed class NotificadorEspia : INotificador
{
    private readonly List<(Dueno Dueno, string Mensaje)> _enviados = [];

    public IReadOnlyList<(Dueno Dueno, string Mensaje)> Enviados => _enviados;

    public bool Notificar(Dueno dueno, string mensaje)
    {
        _enviados.Add((dueno, mensaje));
        return true;
    }
}
