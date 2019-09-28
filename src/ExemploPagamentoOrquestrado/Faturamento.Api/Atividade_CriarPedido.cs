using Comum.CrossCutting;
using Faturamento.Api.Dominio;
using Faturamento.Api.Dominio.Comandos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Faturamento.Api
{
    public sealed class Atividade_CriarPedido
    {
        private readonly CriarPedidoGerenciadorComando _gerenciadorComando;

        public Atividade_CriarPedido(CriarPedidoGerenciadorComando gerenciadorComando)
        {
            _gerenciadorComando = gerenciadorComando;
        }

        [FunctionName(nameof(Atividade_CriarPedido))]
        public async Task<Resultado<Pedido>> Run(
            [ActivityTrigger]  CriarPedidoComando comando,
            ExecutionContext context,
            ILogger log)
        {
            try
            {
                log.LogInformation("Criando pedido para socio {0}", comando.SocioId);
                return await _gerenciadorComando.ExecutarAsync(comando);
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, "Falha catrastrófica ao criar pedido para socio {0}", comando.SocioId);
                return Falha.NovaFalha(Motivo.Novo("FalhaAoCriarPedido", "Falha ao criar pedido"));
            }
        }
    }
}
