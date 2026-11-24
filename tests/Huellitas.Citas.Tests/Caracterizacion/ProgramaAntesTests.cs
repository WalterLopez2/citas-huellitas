using Huellitas.Citas.Tests.Utilidades;
using ProgramaAntes = Huellitas.Citas.Antes.Program;

namespace Huellitas.Citas.Tests.Caracterizacion;

[Collection(Consola.Nombre)]
public class ProgramaAntesTests : IDisposable
{
    public static readonly string[] RespuestasDemo =
    [
        "[TelcoNet] SMS a +50499990001: Hola Ana Reyes, la cita de Firulais es el 12/01/2027 09:00. Total: L 320.00",
        "Cita agendada para Firulais el 12/01/2027 09:00. Total: L 320.00",
        "[TelcoNet] SMS a +50499990002: Hola Luis Castro, la cita de Michi es el 12/01/2027 10:30. Total: L 150.00",
        "Cita agendada para Michi el 12/01/2027 10:30. Total: L 150.00",
        "Ese horario ya está ocupado.",
        "Fuera del horario de atención (8:00 a 17:00).",
        "No atendemos los domingos.",
        "09:00 - Firulais (Baño y corte) - Ana Reyes",
        "10:30 - Michi (Baño) - Luis Castro"
    ];

    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"citas-{Guid.NewGuid()}.txt");

    public ProgramaAntesTests() => ProgramaAntes.archivo = _archivo;

    public void Dispose() => File.Delete(_archivo);

    [Fact]
    public void Main_GuionDeDemo_RespondeLoMismoQueHoy()
    {
        var respuestas = Consola.Ejecutar(ProgramaAntes.Main, Consola.EntradaDemo());

        Assert.Equal(RespuestasDemo, respuestas);
    }

    [Fact]
    public void Main_SextaCitaDelMismoTelefono_Descuenta10PorCiento()
    {
        var entrada = string.Concat(Enumerable.Range(8, 6)
            .Select(hora => Consola.Agendar("Ana Reyes", "9999-0001", "Firulais", "P", "1", $"2027-01-13 {hora:00}:00")));

        var respuestas = Consola.Ejecutar(ProgramaAntes.Main, entrada + "0\n");

        Assert.EndsWith("Total: L 150.00", respuestas[^3]);
        Assert.EndsWith("Total: L 135.00", respuestas[^1]);
    }

    [Theory]
    [InlineData("2020-01-06 10:00", "La fecha debe ser futura.")]
    [InlineData("mañana", "Fecha inválida.")]
    public void Main_FechaNoValida_RechazaLaCita(string fecha, string esperado)
    {
        var entrada = Consola.Agendar("Ana Reyes", "9999-0001", "Firulais", "P", "1", fecha) + "0\n";

        var respuestas = Consola.Ejecutar(ProgramaAntes.Main, entrada);

        Assert.Equal([esperado], respuestas);
    }

    [Fact]
    public void Main_TamanoDesconocido_LoCobraComoGrande()
    {
        var entrada = Consola.Agendar("Ana Reyes", "9999-0001", "Firulais", "X", "1", "2027-01-13 08:00") + "0\n";

        var respuestas = Consola.Ejecutar(ProgramaAntes.Main, entrada);

        Assert.EndsWith("Total: L 280.00", respuestas[^1]);
    }
}
