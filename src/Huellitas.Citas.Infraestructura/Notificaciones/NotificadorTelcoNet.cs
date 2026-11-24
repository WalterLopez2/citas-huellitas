using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;
using TelcoNet.Sms;

namespace Huellitas.Citas.Infraestructura.Notificaciones;

public sealed class NotificadorTelcoNet : INotificador
{
    private const string CodigoDePais = "504";

    public bool Notificar(Dueno dueno, string mensaje) =>
        TelcoNetClient.SendMessage(AMsisdn(dueno.Telefono), mensaje) == TelcoNetClient.Ok;

    private static string AMsisdn(string telefono) => CodigoDePais + telefono.Replace("-", "");
}
