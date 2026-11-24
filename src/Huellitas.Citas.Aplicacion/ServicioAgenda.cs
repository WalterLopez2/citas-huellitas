using System.Globalization;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura;

namespace Huellitas.Citas.Aplicacion;

public sealed class ServicioAgenda
{
    private readonly RepositorioCitasArchivo _repositorio;
    private readonly ServicioSms _sms = new();

    public ServicioAgenda(string rutaArchivo) => _repositorio = new RepositorioCitasArchivo(rutaArchivo);

    public ResultadoAgenda Agendar(SolicitudCita solicitud)
    {
        var duracion = solicitud.Servicio.Duracion();

        var motivo = HorarioAtencion.MotivoDeRechazo(solicitud.Inicio, duracion);
        if (motivo is not null) return ResultadoAgenda.Rechazada(motivo);
        if (solicitud.Inicio <= DateTime.Now) return ResultadoAgenda.Rechazada("La fecha debe ser futura.");

        var citas = _repositorio.Todas();
        if (citas.Any(c => c.SeTraslapaCon(solicitud.Inicio, duracion)))
            return ResultadoAgenda.Rechazada("Ese horario ya está ocupado.");

        var citasPrevias = citas.Count(c => c.Dueno.Telefono == solicitud.Dueno.Telefono);
        var precio = CalculadoraPrecio.Calcular(solicitud.Servicio, solicitud.Mascota.Tamano, citasPrevias);
        var cita = new Cita(Guid.NewGuid(), solicitud.Dueno, solicitud.Mascota, solicitud.Servicio, solicitud.Inicio, precio);

        _repositorio.Guardar(cita);
        _sms.Enviar(cita.Dueno.Telefono, Confirmacion(cita));

        return ResultadoAgenda.Agendada(cita);
    }

    public IReadOnlyList<Cita> CitasDelDia(DateOnly dia) =>
        _repositorio.Todas()
            .Where(c => DateOnly.FromDateTime(c.Inicio) == dia)
            .OrderBy(c => c.Inicio)
            .ToList();

    private static string Confirmacion(Cita cita) =>
        $"Hola {cita.Dueno.Nombre}, la cita de {cita.Mascota.Nombre} es el " +
        $"{cita.Inicio.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}. " +
        $"Total: L {cita.Precio.ToString("N2", CultureInfo.InvariantCulture)}";
}
