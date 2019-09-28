using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PagamentoDemo.WebApi.Util.Saga;
using PagamentoDemo.WebApi.UtilComResultado;

namespace PagamentoDemo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosComBusController : ControllerBase
    {
        private readonly IRequisicaoContexto _contexto;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IServicoPagamento _servicoPagamento;
        private readonly IServicoEmail _servicoEmail;
        private readonly IRegrasPosPagamento _regrasPosPagamento;
        private readonly ILogger _logger;
        private readonly IBus _bus;

        public PagamentosComBusController(
            IRequisicaoContexto contexto,
            IUnitOfWorkFactory unitOfWorkFactory,
            IServicoPagamento servicoPagamento,
            IServicoEmail servicoEmail,
            IRegrasPosPagamento regrasPosPagamento,
            ILogger logger,
            IBus bus)
        {
            _contexto = contexto;
            _unitOfWorkFactory = unitOfWorkFactory;
            _servicoPagamento = servicoPagamento;
            _servicoEmail = servicoEmail;
            _regrasPosPagamento = regrasPosPagamento;
            _logger = logger;
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> RealizarPagamento([FromBody]PedidoModel pedidoModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CriarParaTenant(_contexto.CodigoTenant))
            {
                var clientesRepositorio = new ClientesRepositorio(unitOfWork);
                var pedidosRepositorio = new PedidosRepositorio(unitOfWork);
                if (await clientesRepositorio.ObterAsync(pedidoModel.CodigoCliente) is var clienteResultado && clienteResultado.EhFalha)
                    return BadRequest("Cliente inválido");
                var pedido = Pedido.NovoPendente(clienteResultado.Sucesso, pedidoModel.Dividas, pedidoModel.DadosPagamento);
                await pedidosRepositorio.IncluirOuAtualizarAsync(pedido);
                if (await unitOfWork.CommitAsync() is var transacaoResultado && transacaoResultado.EhFalha)
                    return BadRequest("Falha ao realizar pagamento");
                await _bus.Enviar(new RealizarPagamentoParaPedidoPendenteComando(_contexto.CodigoTenant, pedido.Id));
                return Accepted(pedido);
            }
        }
    }
}
