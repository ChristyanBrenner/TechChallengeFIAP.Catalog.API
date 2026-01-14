using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _catalogo;
        public CatalogoController(ICatalogoService jogo)
        {
            _catalogo = jogo;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListarJogos()
        {
            try
            {
                var listaJogos = await _catalogo.ListarJogosAsync();

                return Ok(listaJogos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var jogo = await _catalogo.ObterPorIdAsync(id);

            if (jogo == null)
                return NotFound("Jogo não encontrado!");

            return Ok(jogo);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] JogoDto dto)
        {
            var id = await _catalogo.CadastrarJogoAsync(dto);

            return CreatedAtAction(nameof(ObterPorId), new { id }, new
            {
                jogoId = id,
                message = "Jogo cadastrado com sucesso!"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/preco")]
        public async Task<IActionResult> AlterarPreco(int id, [FromBody] AlterarPrecoDto dto)
        {
            await _catalogo.AlterarPrecoJogoAsync(id, dto.Preco);

            return Ok(new
            {
                message = "Preço alterado com sucesso"
            });
        }

        [Authorize]
        [HttpPost("/comprar")]
        public async Task<IActionResult> Comprar([FromBody] PedidoDto dto)
        {
            await _catalogo.ComprarJogoAsync(dto);

            return Accepted(new
            {
                message = "Pedido de compra realizado!"
            });
        }
    }
}
