using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Pedido : EntityBase
    {
        public int UsuarioId { get; set; }
        public int JogoId { get; set; }
        public string NomeJogo { get; set; }
        public decimal PrecoPago { get; set; }
        public DateTime DataCompra { get; set; }
    }
}
