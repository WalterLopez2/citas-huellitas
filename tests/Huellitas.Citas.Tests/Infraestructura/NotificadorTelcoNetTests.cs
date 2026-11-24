using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura.Notificaciones;
using Huellitas.Citas.Tests.Utilidades;

namespace Huellitas.Citas.Tests.Infraestructura;

[Collection(ConsolaSimulada.Nombre)]
public class NotificadorTelcoNetTests
{
    private readonly NotificadorTelcoNet _notificador = new();

    [Fact]
    public void Notificar_TelefonoLocal_LoEnviaConElFormatoQuePideTelcoNet()
    {
        var enviado = false;

        var salida = ConsolaSimulada.Ejecutar(() => enviado = _notificador.Notificar(new Dueno("Ana", "9999-0001"), "Hola"), string.Empty);

        Assert.True(enviado);
        Assert.Equal(["[TelcoNet] SMS a +50499990001: Hola"], salida);
    }

    [Fact]
    public void Notificar_TelefonoIncompleto_DevuelveFalsoEnVezDeUnCodigoDeError()
    {
        var enviado = _notificador.Notificar(new Dueno("Ana", "123"), "Hola");

        Assert.False(enviado);
    }
}
