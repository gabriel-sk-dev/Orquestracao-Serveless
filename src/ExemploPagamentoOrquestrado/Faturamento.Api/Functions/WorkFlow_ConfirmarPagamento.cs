using Comum.CrossCutting;
using Faturamento.Api.Dominio;
using Faturamento.Api.Dominio.Comandos;
using Faturamento.Api.Dominio.Eventos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faturamento.Api.Functions
{
    public static class WorkFlow_ConfirmarPagamento
    {
        [FunctionName(nameof(WorkFlow_ConfirmarPagamento))]
        public static async Task<Resultado<ResultadoDoPedido>> Run(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log)
        {
            var dto = context.GetInput<NovoPagamentoDTO>();
            dto.ProcessoId = context.InstanceId;

            if (!context.IsReplaying)
                log.LogInformation("Iniciando processo {0} para pagamento", dto.ProcessoId);

            //Criar 'pedido' de pagamento
            var resultadoPedido = await context.CallActivityAsync<Resultado<Pedido>>(
                    nameof(Atividade_CriarPedido),
                    new CriarPedidoComando(dto.ProcessoId, dto.SocioId, dto.Itens.Select(i => new CriarPedidoComando.Item(i.DividaId, i.Valor)).ToList()));
            if (!resultadoPedido.EhSucesso)
                return resultadoPedido.Falha;

            // Solicitar pagamento
            var resultadoPagamento = await context.CallActivityAsync<Resultado<Pagamento>>(
                    nameof(Atividade_SolicitarPagamento),
                    new SolicitarPagamentoComando(dto.ProcessoId, resultadoPedido.Sucesso.Id, resultadoPedido.Sucesso.ValorTotal,
                        new Pagador(dto.Pagador.Nome, dto.Pagador.Cpf, dto.Pagador.Email, dto.Pagador.TokenCartao)));
            if (!resultadoPagamento.EhSucesso)
            {
                await context.CallActivityAsync(
                        nameof(Atividade_NotificarFalhaAoSolicitarPagamento),
                        new FalhaAoRealizarPagamentoEvento(dto.ProcessoId, resultadoPedido.Sucesso.Id, resultadoPagamento.Falha));
                return resultadoPagamento.Falha;
            }

            if (resultadoPagamento.Sucesso.Status == Pagamento.EStatus.EmAnalise)
            {
                await context.CallActivityAsync(
                        nameof(Atividade_NotificarPagamentoEmAnalise),
                        new PagamentoEstaEmAnaliseEvento(dto.ProcessoId, resultadoPedido.Sucesso.Id, resultadoPagamento.Sucesso.Id));
                var evento = await context.WaitForExternalEvent<string>("PagamentoRealizado", TimeSpan.FromDays(1), "Falha");
                if (evento == "Falha")
                {
                    await context.CallActivityAsync(
                        nameof(Atividade_NotificarPagamentoComFalha),
                        new PagamentoFalhouEvento(dto.ProcessoId, resultadoPedido.Sucesso.Id, resultadoPagamento.Sucesso.Id));
                }
            }

            var resultado = await context.CallActivityAsync<Resultado<Pedido>>(
                    nameof(Atividade_FecharPedido),
                    new FecharPedidoComando(dto.ProcessoId, resultadoPedido.Sucesso.Id, resultadoPagamento.Sucesso.Id));
            if(!resultado.EhSucesso)
                return resultado.Falha;

            if (resultadoPagamento.Sucesso.Status == Pagamento.EStatus.Aprovado)
                await context.CallActivityAsync(
                       nameof(Atividade_NotificarPagamentoAprovado),
                       new PagamentoAprovadoEvento(dto.ProcessoId, resultadoPedido.Sucesso.Id, resultadoPagamento.Sucesso.Id));

            if (resultadoPagamento.Sucesso.Status == Pagamento.EStatus.Negado)
                await context.CallActivityAsync(
                       nameof(Atividade_NotificarPagamentoNegado),
                       new PagamentoNegadoEvento(dto.ProcessoId, resultadoPedido.Sucesso.Id, resultadoPagamento.Sucesso.Id));

            return new ResultadoDoPedido(
                resultadoPedido.Sucesso.Id,
                resultadoPagamento.Sucesso.Id,
                resultadoPagamento.Sucesso.Status.ToString());
        }
    }

    public sealed class ResultadoDoPedido
    {
        public ResultadoDoPedido(int pedidoId, int pagamentoId, string status)
        {
            PedidoId = pedidoId;
            PagamentoId = pagamentoId;
            Status = status;
        }

        public int PedidoId { get; set; }
        public int PagamentoId { get; set; }
        public string Status { get; set; }
    }
}
