using Comum.CrossCutting;
using Faturamento.Api.Dominio;
using Faturamento.Api.Dominio.Comandos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Faturamento.Api
{
    public sealed class Atividade_SolicitarPagamento
    {
        private readonly SolicitarPagamentoGerenciadorComando _gerenciadorComando;

        public Atividade_SolicitarPagamento(SolicitarPagamentoGerenciadorComando gerenciadorComando)
        {
            _gerenciadorComando = gerenciadorComando;
        }

        [FunctionName(nameof(Atividade_SolicitarPagamento))]
        public async Task<Resultado<Pagamento>> Run(
            [ActivityTrigger]  SolicitarPagamentoComando comando,
            ExecutionContext context,
            ILogger log)
        {
            try
            {
                log.LogInformation("Processo {0} solicitando pagamento para pedido {1}", comando.ProcessoId, comando.PedidoId);
                return await _gerenciadorComando.ExecutarAsync(comando);
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, "Falha catrastrófica no processo {0} ao solicitar pagamento para pedido {1}", comando.ProcessoId, comando.PedidoId);
                return Falha.NovaFalha(Motivo.Novo("FalhaAoSolicitarPedido", "Falha ao solicitar pedido"));
            }
        }
    }
}
