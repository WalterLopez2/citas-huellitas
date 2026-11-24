using System.Globalization;
using Huellitas.Citas.Aplicacion.Puertos;

namespace Huellitas.Citas.Aplicacion.Eventos;

public sealed class AvisarAlDueno : IObservadorDeCitas
{
    private readonly INotificador _notificador;

    public AvisarAlDueno(INotificador notificador) => _notificador = notificador;

    public void AlAgendar(CitaAgendada evento)
    {
        var cita = evento.Cita;
        _notificador.Notificar(cita.Dueno,
            $"Hola {cita.Dueno.Nombre}, la cita de {cita.Mascota.Nombre} es el " +
            $"{cita.Inicio.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}. " +
            $"Total: L {cita.Precio.ToString("N2", CultureInfo.InvariantCulture)}");
    }
}
