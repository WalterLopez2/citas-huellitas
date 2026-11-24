using System.Globalization;
using Huellitas.Citas.Aplicacion;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura;
using Huellitas.Citas.Infraestructura.Notificaciones;
using Huellitas.Citas.Infraestructura.Persistencia;

namespace Huellitas.Citas.Consola;

public static class Program
{
    private const string FormatoFechaHora = "yyyy-MM-dd HH:mm";

    public static void Main(string[] args)
    {
        var agenda = new ServicioAgenda(
            new RepositorioCitasArchivo(args.Length > 0 ? args[0] : "citas.json"),
            new NotificadorConReintentos(new NotificadorTelcoNet(), intentos: 3),
            new RelojDelSistema());

        while (true)
        {
            Console.WriteLine("1) Agendar cita  2) Ver citas del día  0) Salir");
            switch (Console.ReadLine())
            {
                case "1": Agendar(agenda); break;
                case "2": MostrarCitasDelDia(agenda); break;
                case "0" or null: return;
            }
        }
    }

    private static void Agendar(ServicioAgenda agenda)
    {
        var dueno = new Dueno(Preguntar("Dueño: "), Preguntar("Teléfono: "));
        var nombreMascota = Preguntar("Mascota: ");
        var tamano = LeerTamano(Preguntar("Tamaño (P/M/G): "));
        var servicio = Preguntar("Servicio (1 = Baño, 2 = Baño y corte): ") == "2" ? TipoServicio.BanoYCorte : TipoServicio.Bano;
        var textoFecha = Preguntar("Fecha y hora (yyyy-MM-dd HH:mm): ");

        if (!DateTime.TryParseExact(textoFecha, FormatoFechaHora, CultureInfo.InvariantCulture, DateTimeStyles.None, out var inicio))
        {
            Console.WriteLine("Fecha inválida.");
            return;
        }

        var resultado = agenda.Agendar(new SolicitudCita(dueno, new Mascota(nombreMascota, tamano), servicio, inicio));

        Console.WriteLine(resultado.Cita is { } cita
            ? $"Cita agendada para {cita.Mascota.Nombre} el {cita.Inicio.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}. " +
              $"Total: L {cita.Precio.ToString("N2", CultureInfo.InvariantCulture)}"
            : resultado.Motivo);
    }

    private static void MostrarCitasDelDia(ServicioAgenda agenda)
    {
        var texto = Preguntar("Fecha (yyyy-MM-dd): ");
        if (!DateOnly.TryParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dia))
        {
            Console.WriteLine("Fecha inválida.");
            return;
        }

        var citas = agenda.CitasDelDia(dia);
        if (citas.Count == 0) Console.WriteLine("No hay citas para ese día.");

        foreach (var cita in citas)
            Console.WriteLine($"{cita.Inicio:HH:mm} - {cita.Mascota.Nombre} ({cita.Servicio.Nombre()}) - {cita.Dueno.Nombre}");
    }

    private static Tamano LeerTamano(string texto) => texto switch
    {
        "P" => Tamano.Pequeno,
        "M" => Tamano.Mediano,
        _ => Tamano.Grande
    };

    private static string Preguntar(string indicacion)
    {
        Console.Write(indicacion);
        return Console.ReadLine() ?? string.Empty;
    }
}
