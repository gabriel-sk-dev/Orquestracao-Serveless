using Faturamento.Api.Dominio.Eventos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Faturamento.Api.Functions
{
    public sealed class Atividade_NotificarPagamentoNegado
    {
        [FunctionName(nameof(Atividade_NotificarPagamentoNegado))]
        public async Task Run(
            [ActivityTrigger]  PagamentoNegadoEvento evento,
            [ServiceBus(nameof(PagamentoNegadoEvento), Connection = "AzureServiceBus", EntityType = EntityType.Queue)] IAsyncCollector<PagamentoNegadoEvento> outputQueue,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("Notificando que pagamento {0} foi negado para processo {1} e pedido {2}", evento.PagamentoId, evento.ProcessoId, evento.PedidoId);
            await outputQueue.AddAsync(evento);
        }
    }
}
