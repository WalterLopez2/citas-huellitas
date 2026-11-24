using Huellitas.Citas.Dominio;

namespace Huellitas.Citas.Aplicacion.Eventos;

public sealed record CitaAgendada(Cita Cita);

public interface IObservadorDeCitas
{
    void AlAgendar(CitaAgendada evento);
}

public sealed class PublicadorDeEventos
{
    private readonly List<IObservadorDeCitas> _observadores = [];

    public PublicadorDeEventos Suscribir(IObservadorDeCitas observador)
    {
        _observadores.Add(observador);
        return this;
    }

    public void Publicar(CitaAgendada evento)
    {
        foreach (var observador in _observadores)
            observador.AlAgendar(evento);
    }
}
