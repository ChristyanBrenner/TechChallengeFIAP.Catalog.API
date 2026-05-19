using Domain.DTOs;
using Domain.Entities;

namespace Services
{
    public interface IGameSearchService
    {
        Task IndexarJogoAsync(Jogo jogo);
        Task AtualizarJogoAsync(Jogo jogo);
        Task<List<JogoSearchDocument>> BuscarAsync(string query);
    }
}
