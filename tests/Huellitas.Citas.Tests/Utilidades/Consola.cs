namespace Huellitas.Citas.Tests.Utilidades;

[CollectionDefinition(Nombre, DisableParallelization = true)]
public sealed class Consola
{
    public const string Nombre = "Consola";

    private static readonly string[] Indicaciones =
    [
        "Dueño: ", "Teléfono: ", "Mascota: ", "Tamaño (P/M/G): ",
        "Servicio (1 = Baño, 2 = Baño y corte): ", "Fecha y hora (yyyy-MM-dd HH:mm): ", "Fecha (yyyy-MM-dd): "
    ];

    private const string Menu = "1) Agendar cita  2) Ver citas del día  0) Salir";

    public static IReadOnlyList<string> Ejecutar(Action programa, string entrada)
    {
        var entradaOriginal = Console.In;
        var salidaOriginal = Console.Out;
        var salida = new StringWriter();
        try
        {
            Console.SetIn(new StringReader(entrada));
            Console.SetOut(salida);
            programa();
        }
        finally
        {
            Console.SetIn(entradaOriginal);
            Console.SetOut(salidaOriginal);
        }

        return Respuestas(salida.ToString());
    }

    public static string EntradaDemo() =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "entrada-demo.txt"));

    public static string Agendar(string dueno, string telefono, string mascota, string tamano, string servicio, string fecha) =>
        string.Join('\n', "1", dueno, telefono, mascota, tamano, servicio, fecha) + "\n";

    private static IReadOnlyList<string> Respuestas(string salida)
    {
        foreach (var indicacion in Indicaciones)
            salida = salida.Replace(indicacion, string.Empty);

        return salida.Split('\n')
            .Select(linea => linea.TrimEnd('\r'))
            .Where(linea => linea.Length > 0 && linea != Menu)
            .ToList();
    }
}
