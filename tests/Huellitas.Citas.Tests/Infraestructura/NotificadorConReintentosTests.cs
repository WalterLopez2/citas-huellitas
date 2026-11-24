using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura.Notificaciones;

namespace Huellitas.Citas.Tests.Infraestructura;

public class NotificadorConReintentosTests
{
    private static readonly Dueno Ana = new("Ana", "9999-0001");

    [Fact]
    public void Notificar_ElProveedorFallaDosVeces_AlTercerIntentoLoLogra()
    {
        var proveedor = new NotificadorQueFalla(fallosAntesDeFuncionar: 2);
        var notificador = new NotificadorConReintentos(proveedor, intentos: 3);

        var enviado = notificador.Notificar(Ana, "Hola");

        Assert.True(enviado);
        Assert.Equal(3, proveedor.Llamadas);
    }

    [Fact]
    public void Notificar_ElProveedorSiempreFalla_SeRindeTrasLosIntentosConfigurados()
    {
        var proveedor = new NotificadorQueFalla(fallosAntesDeFuncionar: int.MaxValue);
        var notificador = new NotificadorConReintentos(proveedor, intentos: 3);

        var enviado = notificador.Notificar(Ana, "Hola");

        Assert.False(enviado);
        Assert.Equal(3, proveedor.Llamadas);
    }

    [Fact]
    public void Notificar_ElProveedorFuncionaALaPrimera_NoReintenta()
    {
        var proveedor = new NotificadorQueFalla(fallosAntesDeFuncionar: 0);

        new NotificadorConReintentos(proveedor, intentos: 3).Notificar(Ana, "Hola");

        Assert.Equal(1, proveedor.Llamadas);
    }

    private sealed class NotificadorQueFalla(int fallosAntesDeFuncionar) : INotificador
    {
        public int Llamadas { get; private set; }

        public bool Notificar(Dueno dueno, string mensaje) => ++Llamadas > fallosAntesDeFuncionar;
    }
}
