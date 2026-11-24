namespace Huellitas.Citas.Dominio;

public static class CalculadoraPrecio
{
    private const int CitasPreviasParaSerFrecuente = 5;
    private const decimal DescuentoClienteFrecuente = 0.10m;

    private static readonly IReadOnlyDictionary<(TipoServicio, Tamano), decimal> PrecioBase =
        new Dictionary<(TipoServicio, Tamano), decimal>
        {
            [(TipoServicio.Bano, Tamano.Pequeno)] = 150m,
            [(TipoServicio.Bano, Tamano.Mediano)] = 200m,
            [(TipoServicio.Bano, Tamano.Grande)] = 280m,
            [(TipoServicio.BanoYCorte, Tamano.Pequeno)] = 250m,
            [(TipoServicio.BanoYCorte, Tamano.Mediano)] = 320m,
            [(TipoServicio.BanoYCorte, Tamano.Grande)] = 420m
        };

    public static decimal Calcular(TipoServicio servicio, Tamano tamano, int citasPrevias)
    {
        var precio = PrecioBase[(servicio, tamano)];
        return citasPrevias >= CitasPreviasParaSerFrecuente ? precio * (1 - DescuentoClienteFrecuente) : precio;
    }
}
