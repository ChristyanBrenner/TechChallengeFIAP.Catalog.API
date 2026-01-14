using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class JogoDto
    {
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Genero { get; set; }
        [Required]
        public decimal Preco { get; set; }
    }
}
