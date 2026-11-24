using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Aplicacion.Puertos;

public interface IRepositorioCitas
{
    IReadOnlyList<Cita> DelDia(DateOnly dia);

    int ContarDelTelefono(string telefono);

    void Agregar(Cita cita);
}
