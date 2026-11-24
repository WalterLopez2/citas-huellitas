using System.Text.Json;
using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Infraestructura.Persistencia;

public sealed class RepositorioCitasArchivo : IRepositorioCitas
{
    private static readonly JsonSerializerOptions Formato = new() { WriteIndented = true };

    private readonly string _ruta;

    public RepositorioCitasArchivo(string ruta) => _ruta = ruta;

    public IReadOnlyList<Cita> DelDia(DateOnly dia) =>
        Leer().Where(c => DateOnly.FromDateTime(c.Inicio) == dia).ToList();

    public int ContarDelTelefono(string telefono) =>
        Leer().Count(c => c.Dueno.Telefono == telefono);

    public void Agregar(Cita cita) =>
        File.WriteAllText(_ruta, JsonSerializer.Serialize(Leer().Append(cita), Formato));

    private List<Cita> Leer() =>
        File.Exists(_ruta)
            ? JsonSerializer.Deserialize<List<Cita>>(File.ReadAllText(_ruta)) ?? []
            : [];
}
