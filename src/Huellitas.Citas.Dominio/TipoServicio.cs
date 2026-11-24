namespace Huellitas.Citas.Dominio;

public enum TipoServicio
{
    Bano,
    BanoYCorte
}

public static class TipoServicioExtensiones
{
    public static TimeSpan Duracion(this TipoServicio servicio) => servicio switch
    {
        TipoServicio.Bano => TimeSpan.FromMinutes(60),
        TipoServicio.BanoYCorte => TimeSpan.FromMinutes(90),
        _ => throw new ArgumentOutOfRangeException(nameof(servicio))
    };

    public static string Nombre(this TipoServicio servicio) => servicio switch
    {
        TipoServicio.Bano => "Baño",
        TipoServicio.BanoYCorte => "Baño y corte",
        _ => throw new ArgumentOutOfRangeException(nameof(servicio))
    };
}
