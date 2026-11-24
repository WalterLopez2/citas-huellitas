using Huellitas.Citas.Aplicacion.Puertos;
using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Infraestructura.Persistencia;

public sealed class RepositorioCitasEnMemoria : IRepositorioCitas
{
    private readonly List<Cita> _citas = [];

    public IReadOnlyList<Cita> DelDia(DateOnly dia) =>
        _citas.Where(c => DateOnly.FromDateTime(c.Inicio) == dia).ToList();

    public int ContarDelTelefono(string telefono) =>
        _citas.Count(c => c.Dueno.Telefono == telefono);

    public void Agregar(Cita cita) => _citas.Add(cita);
}
