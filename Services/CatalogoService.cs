using Domain.DTOs;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Repositories;
using StackExchange.Redis;
using Utils;

namespace Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly AppDbContext _ctx;
        private readonly SqsService _sqsService;
        private readonly IDatabase _cache;
        private readonly IEventLogService _eventLogService;
        private readonly IGameSearchService _gameSearchService;

        public CatalogoService(AppDbContext ctx, SqsService sqsService, IConnectionMultiplexer redis, IEventLogService eventLogService, IGameSearchService gameSearchService)
        {
            _ctx = ctx;
            _sqsService = sqsService;
            _cache = redis.GetDatabase();
            _eventLogService = eventLogService;
            _gameSearchService = gameSearchService;
        }
        public async Task<List<Jogo>> ListarJogosAsync()
        {
            const string cacheKey = "catalogo:jogos";

            var cache = await _cache.StringGetAsync(cacheKey);

            if (cache.HasValue)
                return System.Text.Json.JsonSerializer.Deserialize<List<Jogo>>(cache!)!;

            var jogos = await _ctx.Jogo.AsNoTracking().ToListAsync();

            await _cache.StringSetAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(jogos), TimeSpan.FromMinutes(10));

            return jogos;
        }
        public async Task<Jogo?> ObterPorIdAsync(int id)
        {
            var cacheKey = $"catalogo:jogo{id}";

            var cache = await _cache.StringGetAsync(cacheKey);

            if (cache.HasValue)
                return System.Text.Json.JsonSerializer.Deserialize<Jogo>(cache!)!;

            var jogo = await _ctx.Jogo.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);

            if (jogo != null) 
            {
                await _cache.StringSetAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(jogo), TimeSpan.FromMinutes(10));
            }

            return jogo;
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

            await _eventLogService.RegistrarAsync(new EventLog
            {
                EventType = "JodoCriado",
                Payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    jogo.Id,
                    jogo.Nome,
                    jogo.Genero,
                    jogo.Preco
                })
            });
            await _gameSearchService.IndexarJogoAsync(jogo);
            await _cache.KeyDeleteAsync("catalogo:jogos");

            return jogo.Id;
        }

        public async Task AlterarPrecoJogoAsync(int id, decimal preco)
        {
            var jogo = await _ctx.Jogo.FindAsync(id);

            if (jogo == null)
                throw new KeyNotFoundException("Jogo não encontrado");

            jogo.Preco = preco;

            await _ctx.SaveChangesAsync();

            await _eventLogService.RegistrarAsync(new EventLog
            {
                EventType = "JogoAtualizado",
                Payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    jogo.Id,
                    jogo.Nome,
                    jogo.Preco
                })
            });
            await _gameSearchService.AtualizarJogoAsync(jogo);
            await _cache.KeyDeleteAsync("catalogo:jogos");
            await _cache.KeyDeleteAsync($"catalogo:jogo:{id}");
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

            await _eventLogService.RegistrarAsync(new EventLog
            {
                EventType = "PedidoCriado",
                Payload = System.Text.Json.JsonSerializer.Serialize(evento)
            });
        }
    }
}
