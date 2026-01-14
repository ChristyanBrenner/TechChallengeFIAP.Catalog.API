namespace Domain.Entities
{
    public class Jogo : EntityBase
    {
        public required string Nome { get; set; }
        public required string Genero { get; set; }
        public required decimal Preco { get; set; }
        public string NomeNormalizado { get; set; } = null!;
        public string GeneroNormalizado { get; set; } = null!;
    }
}
