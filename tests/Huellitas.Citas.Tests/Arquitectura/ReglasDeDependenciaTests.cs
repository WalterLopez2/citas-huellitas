using System.Reflection;
using Huellitas.Citas.Aplicacion;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura;

namespace Huellitas.Citas.Tests.Arquitectura;

public class ReglasDeDependenciaTests
{
    private static IReadOnlyList<string> ReferenciasDe(Assembly ensamblado) =>
        ensamblado.GetReferencedAssemblies().Select(r => r.Name!).Where(n => n.StartsWith("Huellitas") || n.StartsWith("TelcoNet")).ToList();

    [Fact]
    public void Dominio_NoDependeDeNingunOtroProyecto()
    {
        var referencias = ReferenciasDe(typeof(Cita).Assembly);

        Assert.Empty(referencias);
    }

    [Fact]
    public void Aplicacion_SoloDependeDelDominio()
    {
        var referencias = ReferenciasDe(typeof(ServicioAgenda).Assembly);

        Assert.Equal(["Huellitas.Citas.Dominio"], referencias);
    }

    [Fact]
    public void Infraestructura_NoConoceLaConsola()
    {
        var referencias = ReferenciasDe(typeof(RelojDelSistema).Assembly);

        Assert.DoesNotContain("Huellitas.Citas.Consola", referencias);
    }
}
