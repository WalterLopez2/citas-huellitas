using System.Text.Json;
using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Infraestructura;

public sealed class RepositorioCitasArchivo
{
    private static readonly JsonSerializerOptions Formato = new() { WriteIndented = true };

    private readonly string _ruta;

    public RepositorioCitasArchivo(string ruta) => _ruta = ruta;

    public IReadOnlyList<Cita> Todas() =>
        File.Exists(_ruta)
            ? JsonSerializer.Deserialize<List<Cita>>(File.ReadAllText(_ruta)) ?? []
            : [];

    public void Guardar(Cita cita)
    {
        var citas = Todas().Append(cita).ToList();
        File.WriteAllText(_ruta, JsonSerializer.Serialize(citas, Formato));
    }
}
