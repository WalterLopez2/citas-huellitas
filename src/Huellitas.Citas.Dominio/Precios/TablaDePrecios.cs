namespace Huellitas.Citas.Dominio.Precios;

public static class TablaDePrecios
{
    private static readonly IReadOnlyDictionary<(TipoServicio, Tamano), decimal> Precios =
        new Dictionary<(TipoServicio, Tamano), decimal>
        {
            [(TipoServicio.Bano, Tamano.Pequeno)] = 150m,
            [(TipoServicio.Bano, Tamano.Mediano)] = 200m,
            [(TipoServicio.Bano, Tamano.Grande)] = 280m,
            [(TipoServicio.BanoYCorte, Tamano.Pequeno)] = 250m,
            [(TipoServicio.BanoYCorte, Tamano.Mediano)] = 320m,
            [(TipoServicio.BanoYCorte, Tamano.Grande)] = 420m
        };

    public static decimal PrecioBase(TipoServicio servicio, Tamano tamano) => Precios[(servicio, tamano)];
}
