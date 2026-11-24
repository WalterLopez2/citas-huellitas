namespace Huellitas.Citas.Dominio;

public sealed record Cita(Guid Id, Dueno Dueno, Mascota Mascota, TipoServicio Servicio, DateTime Inicio, decimal Precio)
{
    public DateTime Fin => Inicio + Servicio.Duracion();

    public bool SeTraslapaCon(DateTime inicio, TimeSpan duracion) =>
        inicio < Fin && Inicio < inicio + duracion;
}
