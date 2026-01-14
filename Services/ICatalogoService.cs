using Domain.DTOs;
using Domain.Entities;

namespace Services
{
    public interface ICatalogoService
    {
        Task<List<Jogo>> ListarJogosAsync();
        Task<Jogo?> ObterPorIdAsync(int id);
        Task ComprarJogoAsync(PedidoDto dto);
        Task<int> CadastrarJogoAsync(JogoDto dto);
        Task AlterarPrecoJogoAsync(int id, decimal preco);
    }
}
