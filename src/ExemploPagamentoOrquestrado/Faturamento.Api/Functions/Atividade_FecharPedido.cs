using Comum.CrossCutting;
using Faturamento.Api.Dominio;
using Faturamento.Api.Dominio.Comandos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Faturamento.Api.Functions
{
    public sealed class Atividade_FecharPedido
    {
        private readonly FecharPedidoGerenciadorComando _gerenciadorComando;

        public Atividade_FecharPedido(FecharPedidoGerenciadorComando gerenciadorComando)
        {
            _gerenciadorComando = gerenciadorComando;
        }

        [FunctionName(nameof(Atividade_FecharPedido))]
        public async Task<Resultado<Pedido>> Run(
            [ActivityTrigger]  FecharPedidoComando comando,
            ExecutionContext context,
            ILogger log)
        {
            try
            {
                log.LogInformation("Processo {0} solicitando fechamento do pedido {1}", comando.ProcessoId, comando.PedidoId);
                return await _gerenciadorComando.ExecutarAsync(comando);
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, "Falha catrastrófica no processo {0} ao fechar pedido {1}", comando.ProcessoId, comando.PedidoId);
                return Falha.NovaFalha(Motivo.Novo("FalhaAoFecharPedido", "Falha ao fechar pedido"));
            }
        }
    }
}
