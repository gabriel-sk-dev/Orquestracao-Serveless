using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PagamentoDemo.WebApi.UtilComResultado;

namespace PagamentoDemo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosComResultadoController : ControllerBase
    {
        private readonly IRequisicaoContexto _contexto;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IServicoPagamento _servicoPagamento;
        private readonly IServicoEmail _servicoEmail;
        private readonly IRegrasPosPagamento _regrasPosPagamento;
        private readonly ILogger _logger;

        public PagamentosComResultadoController(
            IRequisicaoContexto contexto,
            IUnitOfWorkFactory unitOfWorkFactory,
            IServicoPagamento servicoPagamento,
            IServicoEmail servicoEmail,
            IRegrasPosPagamento regrasPosPagamento,
            ILogger logger)
        {
            _contexto = contexto;
            _unitOfWorkFactory = unitOfWorkFactory;
            _servicoPagamento = servicoPagamento;
            _servicoEmail = servicoEmail;
            _regrasPosPagamento = regrasPosPagamento;
            _logger = logger;
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
                var pagamento = Pagamento.NovoPagamento();

                if (await _servicoPagamento.AutorizarPagamentoAsync(pedido) is var autorizacaoResultado && autorizacaoResultado.EhFalha)
                    return BadRequest("Aconteceu um problema ao realizar o pagamento");
                if (!pagamento.Aprovado)
                    return BadRequest(pagamento.NegadoPor);
                pedido.RealizarPagamento(pagamento);

                if (await _servicoEmail.EnviarObrigadoPeloPagamentoAsync(pedido) is var emailEnviado && emailEnviado.EhFalha)
                    await _logger.Error("Admin favor enviar email de pagamento realizado");
                if(await _regrasPosPagamento.ExecutarRegrasPosPagamentoAsync(pedido) is var regrasExecutadas && regrasExecutadas.EhFalha)
                    await _logger.Error("Admin favor executar regras manualmente");

                await pedidosRepositorio.IncluirOuAtualizarAsync(pedido);
                if(await unitOfWork.CommitAsync() is var transacaoResultado && transacaoResultado.EhFalha)
                    return BadRequest("Falha ao realizar pagamento");

                if(await _servicoPagamento.CapturarPagamentoAsync(pedido) is var capturaResultado && capturaResultado.EhFalha)
                    await _logger.Error("Capturar manualmente o pagamento");
                return Ok(pagamento);
            }

            
        }
    }
}
