namespace Huellitas.Citas.Dominio;

public enum Tamano
{
    Pequeno,
    Mediano,
    Grande
}

public sealed record Mascota(string Nombre, Tamano Tamano);

public sealed record Dueno(string Nombre, string Telefono);
