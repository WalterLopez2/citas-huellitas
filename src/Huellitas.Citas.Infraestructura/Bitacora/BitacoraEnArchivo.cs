using System.Globalization;
using Huellitas.Citas.Aplicacion.Eventos;
using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Infraestructura.Bitacora;

public sealed class BitacoraEnArchivo : IObservadorDeCitas
{
    private readonly string _ruta;

    public BitacoraEnArchivo(string ruta) => _ruta = ruta;

    public void AlAgendar(CitaAgendada evento)
    {
        var cita = evento.Cita;
        var linea = string.Join(" | ",
            cita.Inicio.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            cita.Mascota.Nombre,
            cita.Servicio.Nombre(),
            "L " + cita.Precio.ToString("N2", CultureInfo.InvariantCulture),
            cita.Dueno.Nombre);

        File.AppendAllText(_ruta, linea + Environment.NewLine);
    }
}
