using Huellitas.Citas.Aplicacion;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura.Persistencia;
using Huellitas.Citas.Tests.Dobles;

namespace Huellitas.Citas.Tests.Aplicacion;

public class ServicioAgendaTests
{
    private static readonly DateTime Hoy = new(2027, 1, 11, 12, 0, 0);
    private static readonly DateTime MartesALas9 = new(2027, 1, 12, 9, 0, 0);

    private readonly RepositorioCitasEnMemoria _repositorio = new();
    private readonly NotificadorEspia _notificador = new();
    private readonly ServicioAgenda _agenda;

    public ServicioAgendaTests() => _agenda = new ServicioAgenda(_repositorio, _notificador, new RelojFijo(Hoy));

    private static SolicitudCita Solicitud(DateTime inicio, string telefono = "9999-0001", TipoServicio servicio = TipoServicio.Bano) =>
        new(new Dueno("Ana Reyes", telefono), new Mascota("Firulais", Tamano.Mediano), servicio, inicio);

    [Fact]
    public void Agendar_HorarioLibre_GuardaLaCitaYAvisaAlDueno()
    {
        var resultado = _agenda.Agendar(Solicitud(MartesALas9));

        Assert.True(resultado.Exitoso);
        Assert.Single(_repositorio.DelDia(new DateOnly(2027, 1, 12)));
        var (dueno, mensaje) = Assert.Single(_notificador.Enviados);
        Assert.Equal("9999-0001", dueno.Telefono);
        Assert.Equal("Hola Ana Reyes, la cita de Firulais es el 12/01/2027 09:00. Total: L 200.00", mensaje);
    }

    [Fact]
    public void Agendar_FechaAnteriorAlRelojDelSistema_LaRechaza()
    {
        var resultado = _agenda.Agendar(Solicitud(Hoy.AddHours(-2)));

        Assert.Equal("La fecha debe ser futura.", resultado.Motivo);
        Assert.Empty(_notificador.Enviados);
    }

    [Fact]
    public void Agendar_HorarioQueSeCruzaConOtraCita_LaRechaza()
    {
        _agenda.Agendar(Solicitud(MartesALas9, servicio: TipoServicio.BanoYCorte));

        var resultado = _agenda.Agendar(Solicitud(MartesALas9.AddMinutes(60), telefono: "9999-0002"));

        Assert.Equal("Ese horario ya está ocupado.", resultado.Motivo);
    }

    [Fact]
    public void Agendar_SextaCitaDelMismoTelefono_AplicaDescuentoDeClienteFrecuente()
    {
        for (var hora = 0; hora < 5; hora++)
            _agenda.Agendar(Solicitud(MartesALas9.AddHours(hora)));

        var resultado = _agenda.Agendar(Solicitud(MartesALas9.AddDays(1)));

        Assert.Equal(180m, resultado.Cita!.Precio);
    }

    [Fact]
    public void CitasDelDia_VariasCitas_LasDevuelveOrdenadasPorHora()
    {
        _agenda.Agendar(Solicitud(MartesALas9.AddHours(3)));
        _agenda.Agendar(Solicitud(MartesALas9, telefono: "9999-0002"));

        var citas = _agenda.CitasDelDia(new DateOnly(2027, 1, 12));

        Assert.Equal([MartesALas9, MartesALas9.AddHours(3)], citas.Select(c => c.Inicio));
    }
}
