using Huellitas.Citas.Aplicacion.Eventos;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura.Bitacora;
using Huellitas.Citas.Tests.Dobles;

namespace Huellitas.Citas.Tests.Aplicacion;

public class PublicadorDeEventosTests
{
    private static readonly CitaAgendada Evento = new(new Cita(Guid.NewGuid(), new Dueno("Ana Reyes", "9999-0001"),
        new Mascota("Firulais", Tamano.Mediano), TipoServicio.BanoYCorte, new DateTime(2027, 1, 12, 9, 0, 0), 320m));

    [Fact]
    public void Publicar_VariosObservadores_TodosRecibenElEvento()
    {
        var primero = new ObservadorEspia();
        var segundo = new ObservadorEspia();
        var publicador = new PublicadorDeEventos().Suscribir(primero).Suscribir(segundo);

        publicador.Publicar(Evento);

        Assert.Equal([Evento], primero.Recibidos);
        Assert.Equal([Evento], segundo.Recibidos);
    }

    [Fact]
    public void Publicar_SinObservadores_NoFalla()
    {
        var publicar = () => new PublicadorDeEventos().Publicar(Evento);

        var error = Record.Exception(publicar);

        Assert.Null(error);
    }

    [Fact]
    public void AvisarAlDueno_AlAgendar_MandaLaConfirmacionAlTelefonoDelDueno()
    {
        var notificador = new NotificadorEspia();

        new AvisarAlDueno(notificador).AlAgendar(Evento);

        var (dueno, mensaje) = Assert.Single(notificador.Enviados);
        Assert.Equal("9999-0001", dueno.Telefono);
        Assert.Equal("Hola Ana Reyes, la cita de Firulais es el 12/01/2027 09:00. Total: L 320.00", mensaje);
    }

    [Fact]
    public void BitacoraEnArchivo_AlAgendar_AgregaUnaLineaPorCita()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"bitacora-{Guid.NewGuid()}.log");
        try
        {
            new BitacoraEnArchivo(ruta).AlAgendar(Evento);

            Assert.Equal(["2027-01-12 09:00 | Firulais | Baño y corte | L 320.00 | Ana Reyes"], File.ReadAllLines(ruta));
        }
        finally
        {
            File.Delete(ruta);
        }
    }
}
