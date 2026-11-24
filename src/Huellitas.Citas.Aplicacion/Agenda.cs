using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Aplicacion;

public sealed record SolicitudCita(Dueno Dueno, Mascota Mascota, TipoServicio Servicio, DateTime Inicio);

public sealed record ResultadoAgenda(Cita? Cita, string? Motivo)
{
    public bool Exitoso => Cita is not null;

    public static ResultadoAgenda Agendada(Cita cita) => new(cita, null);

    public static ResultadoAgenda Rechazada(string motivo) => new(null, motivo);
}
