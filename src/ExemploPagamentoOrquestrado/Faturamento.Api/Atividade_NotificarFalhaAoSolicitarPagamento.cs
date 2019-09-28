using Faturamento.Api.Dominio.Eventos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Faturamento.Api
{
    public sealed class Atividade_NotificarFalhaAoSolicitarPagamento
    {
        [FunctionName(nameof(Atividade_NotificarFalhaAoSolicitarPagamento))]
        public async Task Run(
            [ActivityTrigger]  FalhaAoRealizarPagamentoEvento evento,
            [ServiceBus(nameof(FalhaAoRealizarPagamentoEvento), Connection = "AzureServiceBus", EntityType = EntityType.Queue)] IAsyncCollector<FalhaAoRealizarPagamentoEvento> outputQueue,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("Notificando falha ao solicitar pagamento para processo {0} e pedido {1}", evento.ProcessoId, evento.PedidoId);
            await outputQueue.AddAsync(evento);
        }
    }
}
