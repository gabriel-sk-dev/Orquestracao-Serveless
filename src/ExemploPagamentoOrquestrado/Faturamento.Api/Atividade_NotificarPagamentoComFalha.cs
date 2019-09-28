using Faturamento.Api.Dominio.Eventos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Faturamento.Api
{
    public sealed class Atividade_NotificarPagamentoComFalha
    {
        [FunctionName(nameof(Atividade_NotificarPagamentoComFalha))]
        public async Task Run(
            [ActivityTrigger]  PagamentoFalhouEvento evento,
            [ServiceBus(nameof(PagamentoFalhouEvento), Connection = "AzureServiceBus", EntityType = EntityType.Queue)] IAsyncCollector<PagamentoFalhouEvento> outputQueue,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("Notificando que pagamento {0} estava em análise mas houve falha durante aprovação para processo {1} e pedido {2}", evento.PagamentoId, evento.ProcessoId, evento.PedidoId);
            await outputQueue.AddAsync(evento);
        }
    }
}
