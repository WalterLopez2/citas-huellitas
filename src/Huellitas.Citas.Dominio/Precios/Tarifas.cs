namespace Huellitas.Citas.Dominio.Precios;

public interface ITarifa
{
    decimal Aplicar(decimal precioBase);
}

public sealed class TarifaEstandar : ITarifa
{
    public decimal Aplicar(decimal precioBase) => precioBase;
}

public sealed class TarifaClienteFrecuente : ITarifa
{
    private const decimal Descuento = 0.10m;

    public decimal Aplicar(decimal precioBase) => precioBase * (1 - Descuento);
}

public static class SelectorDeTarifa
{
    private const int CitasPreviasParaSerFrecuente = 5;

    private static readonly ITarifa Estandar = new TarifaEstandar();
    private static readonly ITarifa ClienteFrecuente = new TarifaClienteFrecuente();

    public static ITarifa Para(int citasPrevias) =>
        citasPrevias >= CitasPreviasParaSerFrecuente ? ClienteFrecuente : Estandar;
}
