using Domain.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using OpenSearch.Client;
using OpenSearch.Net;
using Services;

public class OpenSearchGameSearchService : IGameSearchService
{
    private readonly IOpenSearchClient _client;
    private readonly string _index;

    public OpenSearchGameSearchService(IOpenSearchClient client, IConfiguration config)
    {
        _client = client;
        _index = config["OpenSearch:Index"] ?? "games";
    }

    public async Task IndexarJogoAsync(Jogo jogo)
    {
        var doc = new JogoSearchDocument
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Genero = jogo.Genero,
            Preco = jogo.Preco
        };

        var response = await _client.IndexAsync(doc, i => i
            .Index(_index)
            .Id(jogo.Id)
            .Refresh(Refresh.True));

        if (!response.IsValid)
        {
            throw new Exception("Erro ao indexar jogo no OpenSearch: " + response.DebugInformation);
        }
    }

    public async Task AtualizarJogoAsync(Jogo jogo)
    {
        await IndexarJogoAsync(jogo);
    }

    public async Task<List<JogoSearchDocument>> BuscarAsync(string query)
    {
        var response = await _client.SearchAsync<JogoSearchDocument>(s => s
            .Index(_index)
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(p => p.Nome)
                        .Field(p => p.Genero))
                    .Query(query)
                    .Fuzziness(Fuzziness.Auto)
                )
            )
        );

        return response.Documents.ToList();
    }
}