using Huellitas.Citas.Aplicacion.Puertos;

namespace Huellitas.Citas.Infraestructura;

public sealed class RelojDelSistema : IReloj
{
    public DateTime Ahora => DateTime.Now;
}
