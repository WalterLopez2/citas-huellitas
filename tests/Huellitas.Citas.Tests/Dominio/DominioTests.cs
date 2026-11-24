using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Tests.Dominio;

public class HorarioAtencionTests
{
    [Theory]
    [InlineData("2027-01-11 08:00", 60)]
    [InlineData("2027-01-16 15:30", 90)]
    public void MotivoDeRechazo_DentroDelHorario_NoHayMotivo(string inicio, int minutos)
    {
        var motivo = HorarioAtencion.MotivoDeRechazo(DateTime.Parse(inicio), TimeSpan.FromMinutes(minutos));

        Assert.Null(motivo);
    }

    [Theory]
    [InlineData("2027-01-11 07:30", 60, "Fuera del horario de atención (8:00 a 17:00).")]
    [InlineData("2027-01-11 16:30", 60, "Fuera del horario de atención (8:00 a 17:00).")]
    [InlineData("2027-01-17 10:00", 60, "No atendemos los domingos.")]
    public void MotivoDeRechazo_FueraDelHorario_ExplicaElMotivo(string inicio, int minutos, string esperado)
    {
        var motivo = HorarioAtencion.MotivoDeRechazo(DateTime.Parse(inicio), TimeSpan.FromMinutes(minutos));

        Assert.Equal(esperado, motivo);
    }
}

public class CitaTests
{
    private static readonly Cita BanoDeLas9 = new(Guid.NewGuid(), new Dueno("Ana", "9999-0001"),
        new Mascota("Firulais", Tamano.Mediano), TipoServicio.Bano, new DateTime(2027, 1, 12, 9, 0, 0), 200m);

    [Theory]
    [InlineData(9, 30, true)]
    [InlineData(8, 30, true)]
    [InlineData(10, 0, false)]
    [InlineData(8, 0, false)]
    public void SeTraslapaCon_OtraCitaDeUnaHora_DetectaElCruce(int hora, int minuto, bool esperado)
    {
        var traslapa = BanoDeLas9.SeTraslapaCon(new DateTime(2027, 1, 12, hora, minuto, 0), TimeSpan.FromMinutes(60));

        Assert.Equal(esperado, traslapa);
    }

    [Fact]
    public void Fin_BanoYCorte_DuraNoventaMinutos()
    {
        var cita = BanoDeLas9 with { Servicio = TipoServicio.BanoYCorte };

        Assert.Equal(new DateTime(2027, 1, 12, 10, 30, 0), cita.Fin);
    }
}

public class CalculadoraPrecioTests
{
    [Theory]
    [InlineData(TipoServicio.Bano, Tamano.Pequeno, 150)]
    [InlineData(TipoServicio.Bano, Tamano.Grande, 280)]
    [InlineData(TipoServicio.BanoYCorte, Tamano.Mediano, 320)]
    public void Calcular_ClienteNuevo_CobraElPrecioDeLaTabla(TipoServicio servicio, Tamano tamano, decimal esperado)
    {
        var precio = CalculadoraPrecio.Calcular(servicio, tamano, citasPrevias: 0);

        Assert.Equal(esperado, precio);
    }

    [Theory]
    [InlineData(4, 200)]
    [InlineData(5, 180)]
    public void Calcular_DesdeLaQuintaCitaPrevia_Descuenta10PorCiento(int citasPrevias, decimal esperado)
    {
        var precio = CalculadoraPrecio.Calcular(TipoServicio.Bano, Tamano.Mediano, citasPrevias);

        Assert.Equal(esperado, precio);
    }
}
