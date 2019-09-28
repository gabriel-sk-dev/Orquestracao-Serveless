using Faturamento.Api.Dominio.Eventos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Faturamento.Api
{
    public sealed class Atividade_NotificarPagamentoEmAnalise
    {
        [FunctionName(nameof(Atividade_NotificarPagamentoEmAnalise))]
        public async Task Run(
            [ActivityTrigger]  PagamentoEstaEmAnaliseEvento evento,
            [ServiceBus(nameof(PagamentoEstaEmAnaliseEvento), Connection = "AzureServiceBus", EntityType = EntityType.Queue)] IAsyncCollector<PagamentoEstaEmAnaliseEvento> outputQueue,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("Notificando que pagamento {0} se encontra em análise para processo {1} e pedido {2}", evento.PagamentoId, evento.ProcessoId, evento.PedidoId);
            await outputQueue.AddAsync(evento);
        }
    }
}
