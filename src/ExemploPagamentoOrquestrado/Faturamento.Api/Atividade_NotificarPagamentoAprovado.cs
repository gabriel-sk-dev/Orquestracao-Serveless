using Faturamento.Api.Dominio.Eventos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Faturamento.Api
{
    public sealed class Atividade_NotificarPagamentoAprovado
    {
        [FunctionName(nameof(Atividade_NotificarPagamentoAprovado))]
        public async Task Run(
            [ActivityTrigger]  PagamentoAprovadoEvento evento,
            [ServiceBus(nameof(PagamentoAprovadoEvento), Connection = "AzureServiceBus", EntityType = EntityType.Queue)] IAsyncCollector<PagamentoAprovadoEvento> outputQueue,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("Notificando que pagamento {0} foi aprovado para processo {1} e pedido {2}", evento.PagamentoId, evento.ProcessoId, evento.PedidoId);
            await outputQueue.AddAsync(evento);
        }
    }
}
