using System.Globalization;
using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Dominio.Precios;

namespace Huellitas.Citas.Aplicacion;

public sealed class ServicioAgenda
{
    private readonly IRepositorioCitas _repositorio;
    private readonly INotificador _notificador;
    private readonly IReloj _reloj;

    public ServicioAgenda(IRepositorioCitas repositorio, INotificador notificador, IReloj reloj)
    {
        _repositorio = repositorio;
        _notificador = notificador;
        _reloj = reloj;
    }

    public ResultadoAgenda Agendar(SolicitudCita solicitud)
    {
        var duracion = solicitud.Servicio.Duracion();

        var motivo = HorarioAtencion.MotivoDeRechazo(solicitud.Inicio, duracion);
        if (motivo is not null) return ResultadoAgenda.Rechazada(motivo);
        if (solicitud.Inicio <= _reloj.Ahora) return ResultadoAgenda.Rechazada("La fecha debe ser futura.");

        var citasDelDia = _repositorio.DelDia(DateOnly.FromDateTime(solicitud.Inicio));
        if (citasDelDia.Any(c => c.SeTraslapaCon(solicitud.Inicio, duracion)))
            return ResultadoAgenda.Rechazada("Ese horario ya está ocupado.");

        var citasPrevias = _repositorio.ContarDelTelefono(solicitud.Dueno.Telefono);
        var tarifa = SelectorDeTarifa.Para(citasPrevias);
        var precio = tarifa.Aplicar(TablaDePrecios.PrecioBase(solicitud.Servicio, solicitud.Mascota.Tamano));
        var cita = new Cita(Guid.NewGuid(), solicitud.Dueno, solicitud.Mascota, solicitud.Servicio, solicitud.Inicio, precio);

        _repositorio.Agregar(cita);
        _notificador.Notificar(cita.Dueno, Confirmacion(cita));

        return ResultadoAgenda.Agendada(cita);
    }

    public IReadOnlyList<Cita> CitasDelDia(DateOnly dia) =>
        _repositorio.DelDia(dia).OrderBy(c => c.Inicio).ToList();

    private static string Confirmacion(Cita cita) =>
        $"Hola {cita.Dueno.Nombre}, la cita de {cita.Mascota.Nombre} es el " +
        $"{cita.Inicio.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}. " +
        $"Total: L {cita.Precio.ToString("N2", CultureInfo.InvariantCulture)}";
}
