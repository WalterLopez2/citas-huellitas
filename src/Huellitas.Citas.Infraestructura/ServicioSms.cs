using TelcoNet.Sms;

namespace Huellitas.Citas.Infraestructura;

public sealed class ServicioSms
{
    public void Enviar(string telefono, string mensaje) =>
        TelcoNetClient.SendMessage("504" + telefono.Replace("-", ""), mensaje);
}
