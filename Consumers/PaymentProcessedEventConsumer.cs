using CloudGames.Contracts.Events;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly AppDbContext _ctx;

        public PaymentProcessedEventConsumer(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var evento = context.Message;

            if (evento.Status != PaymentStatus.Approved)
                return;

            var existe = await _ctx.Pedido.AnyAsync(p =>
            p.UsuarioId == evento.UserId &&
            p.JogoId == evento.GameId);

            if (existe)
                return;

            var pedido = new Pedido
            {
                UsuarioId = evento.UserId,
                JogoId = evento.GameId,
                NomeJogo = evento.GameName,
                PrecoPago = evento.GamePrice,
                DataCriacao = DateTime.UtcNow,
                DataCompra = DateTime.UtcNow
            };

            _ctx.Pedido.Add(pedido);
            await _ctx.SaveChangesAsync();
        }
    }
}
