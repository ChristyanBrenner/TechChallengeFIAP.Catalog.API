using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

public class SqsService
{
    private readonly AmazonSQSClient _sqsClient;
    private readonly string _filaPedidoUrl;
    private readonly string _filaPagamentoUrl;

    public SqsService(IConfiguration config)
    {
        _sqsClient = new AmazonSQSClient();
        _filaPedidoUrl = config["AWS:SQS:PedidoCriado"];
        _filaPagamentoUrl = config["AWS:SQS:Pagamento"];
    }
    public async Task EnviarPedidoCriadoAsync(object mensagem)
    {
        await _sqsClient.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = _filaPedidoUrl,
            MessageBody = JsonSerializer.Serialize(mensagem)
        });
    }

    public async Task EnviarPagamentoAsync(object mensagem)
    {
        await _sqsClient.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = _filaPagamentoUrl,
            MessageBody = JsonSerializer.Serialize(mensagem)
        });
    }
}