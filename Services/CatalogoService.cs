using Domain.DTOs;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Utils;

namespace Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly AppDbContext _ctx;
        private readonly SqsService _sqsService;

        public CatalogoService(AppDbContext ctx, SqsService sqsService)
        {
            _ctx = ctx;
            _sqsService = sqsService;
        }
        public async Task<List<Jogo>> ListarJogosAsync()
        {
            return await _ctx.Jogo.AsNoTracking().ToListAsync();
        }
        public async Task<Jogo?> ObterPorIdAsync(int id)
        {
            return await _ctx.Jogo
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);
        }        
        public async Task<int> CadastrarJogoAsync(JogoDto dto)
        {
            var nomeNorm = StringNormalizer.Normalizar(dto.Nome);
            var generoNorm = StringNormalizer.Normalizar(dto.Genero);

            var existe = await _ctx.Jogo
                .AnyAsync(j =>
                    j.NomeNormalizado == nomeNorm &&
                    j.GeneroNormalizado == generoNorm);

            if (existe)
                throw new ApplicationException("Já existe um jogo cadastrado com este nome e gênero.");

            var jogo = new Jogo
            {
                Nome = dto.Nome.Trim(),
                Genero = dto.Genero.Trim(),
                NomeNormalizado = nomeNorm,
                GeneroNormalizado = generoNorm,
                Preco = dto.Preco,
                DataCriacao = DateTime.UtcNow
            };

            _ctx.Jogo.Add(jogo);
            await _ctx.SaveChangesAsync();

            return jogo.Id;
        }

        public async Task AlterarPrecoJogoAsync(int id, decimal preco)
        {
            var jogo = await _ctx.Jogo.FindAsync(id);

            if (jogo == null)
                throw new KeyNotFoundException("Jogo não encontrado");

            jogo.Preco = preco;

            await _ctx.SaveChangesAsync();
        }
        public async Task ComprarJogoAsync(PedidoDto dto)
        {
            var jogo = await _ctx.Jogo.FindAsync(dto.JogoId);

            if (jogo == null)
                throw new ApplicationException("Jogo não encontrado");

            var evento = new
            {
                UsuarioId = dto.UsuarioId,
                JogoId = jogo.Id,
                NomeJogo = jogo.Nome,
                Valor = jogo.Preco
            };

            await _sqsService.EnviarPedidoCriadoAsync(evento);
            await _sqsService.EnviarPagamentoAsync(evento);
        }
    }
}
