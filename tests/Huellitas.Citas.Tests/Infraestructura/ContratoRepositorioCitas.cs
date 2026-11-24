using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;
using Huellitas.Citas.Infraestructura.Persistencia;

namespace Huellitas.Citas.Tests.Infraestructura;

public abstract class ContratoRepositorioCitas
{
    protected abstract IRepositorioCitas Crear();

    private static Cita CitaEl(DateTime inicio, string telefono = "9999-0001") =>
        new(Guid.NewGuid(), new Dueno("Ana Reyes", telefono), new Mascota("Firulais", Tamano.Pequeno), TipoServicio.Bano, inicio, 150m);

    [Fact]
    public void DelDia_DevuelveSoloLasCitasDeEseDia()
    {
        var repositorio = Crear();
        var martes = CitaEl(new DateTime(2027, 1, 12, 9, 0, 0));
        repositorio.Agregar(martes);
        repositorio.Agregar(CitaEl(new DateTime(2027, 1, 13, 9, 0, 0)));

        var citas = repositorio.DelDia(new DateOnly(2027, 1, 12));

        Assert.Equal([martes], citas);
    }

    [Fact]
    public void ContarDelTelefono_CuentaTodasLasCitasDeEseTelefono()
    {
        var repositorio = Crear();
        repositorio.Agregar(CitaEl(new DateTime(2027, 1, 12, 9, 0, 0)));
        repositorio.Agregar(CitaEl(new DateTime(2027, 2, 12, 9, 0, 0)));
        repositorio.Agregar(CitaEl(new DateTime(2027, 1, 12, 11, 0, 0), telefono: "9999-0002"));

        var total = repositorio.ContarDelTelefono("9999-0001");

        Assert.Equal(2, total);
    }

    [Fact]
    public void DelDia_SinCitas_DevuelveListaVacia()
    {
        var citas = Crear().DelDia(new DateOnly(2027, 1, 12));

        Assert.Empty(citas);
    }
}

public sealed class RepositorioCitasEnMemoriaTests : ContratoRepositorioCitas
{
    protected override IRepositorioCitas Crear() => new RepositorioCitasEnMemoria();
}

public sealed class RepositorioCitasArchivoTests : ContratoRepositorioCitas, IDisposable
{
    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"citas-{Guid.NewGuid()}.json");

    protected override IRepositorioCitas Crear() => new RepositorioCitasArchivo(_archivo);

    public void Dispose() => File.Delete(_archivo);

    [Fact]
    public void Agregar_OtraInstanciaSobreElMismoArchivo_VeLasCitasGuardadas()
    {
        var cita = new Cita(Guid.NewGuid(), new Dueno("Ana Reyes", "9999-0001"), new Mascota("Firulais", Tamano.Grande),
            TipoServicio.BanoYCorte, new DateTime(2027, 1, 12, 9, 0, 0), 420m);
        Crear().Agregar(cita);

        var leidas = new RepositorioCitasArchivo(_archivo).DelDia(new DateOnly(2027, 1, 12));

        Assert.Equal([cita], leidas);
    }
}
