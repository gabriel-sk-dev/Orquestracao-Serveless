using NServiceBus;
using PagamentoDemo.WebApi.UtilComResultado;
using STI.Compartilhado.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoDemo.WebApi
{
    public class GerenciarPagamentoDePedido :
        Saga<PedidoDadosSaga>,
        IHandleMessages<RealizarPagamentoParaPedidoPendenteComando>,
        IHandleMessages<ConfirmarPagamentoDoPedidoComando>,
        IHandleMessages<FalhaAoRealizarPagamentoComando>
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IServicoPagamento _servicoPagamento;

        public GerenciarPagamentoDePedido(
            ILogger logger,
            IUnitOfWorkFactory unitOfWorkFactory,
            IServicoPagamento servicoPagamento)
        {
            _logger = logger;
            _unitOfWorkFactory = unitOfWorkFactory;
            _servicoPagamento = servicoPagamento;
        }

        public async Task Handle(RealizarPagamentoParaPedidoPendenteComando message, IMessageHandlerContext context)
        {
            using (var unitOfWork = _unitOfWorkFactory.CriarParaTenant(message.CodigoTenant))
            {
                var pedidosRepositorio = new PedidosRepositorio(unitOfWork);
                var pedidoResultado = await pedidosRepositorio.RecuperarAsync(message.PedidoId);
                if (await _servicoPagamento.CapturarPagamentoAsync(pedidoResultado.Sucesso) is var capturaResultado && capturaResultado.EhFalha)
                    await context.Send(new FalhaAoRealizarPagamentoComando(message.CodigoTenant, message.PedidoId, capturaResultado.Falha));
                await context.Send(new ConfirmarPagamentoDoPedidoComando(message.CodigoTenant, message.PedidoId, capturaResultado.Sucesso));
            }
        }

        public async Task Handle(ConfirmarPagamentoDoPedidoComando message, IMessageHandlerContext context)
        {
            using (var unitOfWork = _unitOfWorkFactory.CriarParaTenant(message.CodigoTenant))
            {
                var pedidosRepositorio = new PedidosRepositorio(unitOfWork);
                var pedidoResultado = await pedidosRepositorio.RecuperarAsync(message.PedidoId);
                pedidoResultado.Sucesso.RealizarPagamento(message.Pagamento);
                await pedidosRepositorio.IncluirOuAtualizarAsync(pedidoResultado.Sucesso);
                await unitOfWork.CommitAsync();
                if (message.Pagamento.Aprovado)
                    await context.Publish(new PagamentoAprovadoEvento(message.CodigoTenant, message.PedidoId));
                else
                    await context.Publish(new PagamentoNegadoEvento(message.CodigoTenant, message.PedidoId));
                MarkAsComplete();
            }
        }

        public async Task Handle(FalhaAoRealizarPagamentoComando message, IMessageHandlerContext context)
        {
            //Logar
            //Fazer retentativa
            //Aguardar um tempo
            //Avisar cliente
            await Task.Run(() => { });
            MarkAsComplete();
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PedidoDadosSaga> mapper)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ObrigadoPeloPagamentoEnvioDeEmail : IHandleMessages<PagamentoAprovadoEvento>
    {
        public async Task Handle(PagamentoAprovadoEvento message, IMessageHandlerContext context)
        {
            //Posso logar
            // Recuperar template de email
            // Enviar email
            await Task.Run(() => { });
        }
    }

    public sealed class PagamentoNegadoEvento
    {
        public PagamentoNegadoEvento(string codigoTenant, int pedidoId)
        {
            CodigoTenant = codigoTenant;
            PedidoId = pedidoId;
        }

        public string CodigoTenant { get; set; }
        public int PedidoId { get; set; }
    }

    public sealed class PagamentoAprovadoEvento
    {
        public PagamentoAprovadoEvento(string codigoTenant, int pedidoId)
        {
            CodigoTenant = codigoTenant;
            PedidoId = pedidoId;
        }

        public string CodigoTenant { get; set; }
        public int PedidoId { get; set; }
    }

    public sealed class FalhaAoRealizarPagamentoComando
    {
        public FalhaAoRealizarPagamentoComando(string codigoTenant, int pedidoId, Falha falha)
        {
            CodigoTenant = codigoTenant;
            PedidoId = pedidoId;
            Falha = falha;
        }

        public string CodigoTenant { get; set; }
        public int PedidoId { get; set; }
        public Falha Falha { get; set; }
    }

    public sealed class RealizarPagamentoParaPedidoPendenteComando
    {
        public RealizarPagamentoParaPedidoPendenteComando(string codigoTenant, int pedidoId)
        {
            CodigoTenant = codigoTenant;
            PedidoId = pedidoId;
        }

        public string CodigoTenant { get; set; }
        public int PedidoId { get; set; }
    }

    public sealed class FalhaAoCriarPedidoComando
    {
        public FalhaAoCriarPedidoComando(string codigoTenant, string pedidoId)
        {
            CodigoTenant = codigoTenant;
            PedidoId = pedidoId;
        }

        public string CodigoTenant { get; set; }
        public string PedidoId { get; set; }
    }

    public sealed class FalhaAoCriarPedidoClienteNaoLocalizadoComando
    {
        public FalhaAoCriarPedidoClienteNaoLocalizadoComando(string codigoTenant, string codigoCliente, string pedidoId)
        {
            CodigoTenant = codigoTenant;
            CodigoCliente = codigoCliente;
            PedidoId = pedidoId;
        }

        public string CodigoTenant { get; set; }
        public string CodigoCliente { get; set; }
        public string PedidoId { get; set; }
    }

    public sealed class CriarPedidoComando
    {
        public CriarPedidoComando(string codigoTenant, string pedidoId, string codigoCliente, object[] dividas, object dadosPagamento)
        {
            CodigoTenant = codigoTenant;
            PedidoId = pedidoId;
            CodigoCliente = codigoCliente;
            Dividas = dividas;
            DadosPagamento = dadosPagamento;
        }

        public string CodigoTenant { get; }
        public string PedidoId { get; set; }
        public string CodigoCliente { get; set; }
        public object[] Dividas { get; set; }
        public object DadosPagamento { get; set; }
    }

    public sealed class PedidoDadosSaga : ContainSagaData
    {
        public string CodigoTenant { get; set; }
        public string PedidoId { get; set; }
    }

    public sealed class ProcessarNovoPedidoComando
    {
        public ProcessarNovoPedidoComando(string codigoTenant, int pedidoId)
        {
            CodigoTenant = codigoTenant;
            PedidoId = pedidoId;
        }

        public string CodigoTenant { get; set; }
        public int PedidoId { get; set; }
    }

    public sealed class ConfirmarPagamentoDoPedidoComando
    {
        public string CodigoTenant;
        public int PedidoId;
        public Pagamento Pagamento;

        public ConfirmarPagamentoDoPedidoComando(string codigoTenant, int pedidoId, Pagamento sucesso)
        {

        }
    }
}
