namespace Huellitas.Citas.Dominio;

public static class HorarioAtencion
{
    private static readonly TimeSpan Apertura = TimeSpan.FromHours(8);
    private static readonly TimeSpan Cierre = TimeSpan.FromHours(17);

    public static string? MotivoDeRechazo(DateTime inicio, TimeSpan duracion)
    {
        if (inicio.DayOfWeek == DayOfWeek.Sunday)
            return "No atendemos los domingos.";

        if (inicio.TimeOfDay < Apertura || inicio + duracion > inicio.Date + Cierre)
            return "Fuera del horario de atención (8:00 a 17:00).";

        return null;
    }
}
