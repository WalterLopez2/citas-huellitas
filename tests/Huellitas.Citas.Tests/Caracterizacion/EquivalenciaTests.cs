using Huellitas.Citas.Tests.Utilidades;
using ProgramaDespues = Huellitas.Citas.Consola.Program;

namespace Huellitas.Citas.Tests.Caracterizacion;

[Collection(ConsolaSimulada.Nombre)]
public class EquivalenciaTests : IDisposable
{
    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"citas-{Guid.NewGuid()}.json");

    public void Dispose()
    {
        File.Delete(_archivo);
        File.Delete(Path.ChangeExtension(_archivo, ".log"));
    }

    [Fact]
    public void Main_GuionDeDemo_RespondeIgualQueLaVersionAnterior()
    {
        var respuestas = ConsolaSimulada.Ejecutar(() => ProgramaDespues.Main([_archivo]), ConsolaSimulada.EntradaDemo());

        Assert.Equal(ProgramaAntesTests.RespuestasDemo, respuestas);
    }

    [Fact]
    public void Main_SextaCitaDelMismoTelefono_Descuenta10PorCiento()
    {
        var entrada = string.Concat(Enumerable.Range(8, 6)
            .Select(hora => ConsolaSimulada.Agendar("Ana Reyes", "9999-0001", "Firulais", "P", "1", $"2027-01-13 {hora:00}:00")));

        var respuestas = ConsolaSimulada.Ejecutar(() => ProgramaDespues.Main([_archivo]), entrada + "0\n");

        Assert.EndsWith("Total: L 150.00", respuestas[^3]);
        Assert.EndsWith("Total: L 135.00", respuestas[^1]);
    }
}
