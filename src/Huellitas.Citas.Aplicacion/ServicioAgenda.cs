using Huellitas.Citas.Aplicacion.Eventos;
using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Dominio.Precios;

namespace Huellitas.Citas.Aplicacion;

public sealed class ServicioAgenda
{
    private readonly IRepositorioCitas _repositorio;
    private readonly IReloj _reloj;
    private readonly PublicadorDeEventos _eventos;

    public ServicioAgenda(IRepositorioCitas repositorio, IReloj reloj, PublicadorDeEventos eventos)
    {
        _repositorio = repositorio;
        _reloj = reloj;
        _eventos = eventos;
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

        var tarifa = SelectorDeTarifa.Para(_repositorio.ContarDelTelefono(solicitud.Dueno.Telefono));
        var precio = tarifa.Aplicar(TablaDePrecios.PrecioBase(solicitud.Servicio, solicitud.Mascota.Tamano));
        var cita = new Cita(Guid.NewGuid(), solicitud.Dueno, solicitud.Mascota, solicitud.Servicio, solicitud.Inicio, precio);

        _repositorio.Agregar(cita);
        _eventos.Publicar(new CitaAgendada(cita));

        return ResultadoAgenda.Agendada(cita);
    }

    public IReadOnlyList<Cita> CitasDelDia(DateOnly dia) =>
        _repositorio.DelDia(dia).OrderBy(c => c.Inicio).ToList();
}
