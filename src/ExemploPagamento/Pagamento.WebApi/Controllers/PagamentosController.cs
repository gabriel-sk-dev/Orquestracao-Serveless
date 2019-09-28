using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PagamentoDemo.WebApi.Util;

namespace PagamentoDemo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosController : ControllerBase
    {
        private readonly IRequisicaoContexto _contexto;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IServicoPagamento _servicoPagamento;
        private readonly IServicoEmail _servicoEmail;
        private readonly IRegrasPosPagamento _regrasPosPagamento;

        public PagamentosController(IRequisicaoContexto contexto, IUnitOfWorkFactory unitOfWorkFactory, IServicoPagamento servicoPagamento, IServicoEmail servicoEmail, IRegrasPosPagamento regrasPosPagamento)
        {
            _contexto = contexto;
            _unitOfWorkFactory = unitOfWorkFactory;
            _servicoPagamento = servicoPagamento;
            _servicoEmail = servicoEmail;
            _regrasPosPagamento = regrasPosPagamento;
        }

        [HttpPost]
        public async Task<IActionResult> RealizarPagamento([FromBody]PedidoModel pedidoModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CriarParaTenant(_contexto.CodigoTenant))
            {
                var clientesRepositorio = new ClientesRepositorio(unitOfWork);
                var pedidosRepositorio = new PedidosRepositorio(unitOfWork);

                var cliente = await clientesRepositorio.ObterAsync(pedidoModel.CodigoCliente);
                var pedido = Pedido.Criar(cliente, pedidoModel.Dividas, pedidoModel.DadosPagamento);
                var pagamento = await _servicoPagamento.PagarAsync(pedido);
                if (!pagamento.Aprovado)
                    return BadRequest(pagamento.NegadoPor);
                pedido.RealizarPagamento(pagamento);

                await _servicoEmail.EnviarObrigadoPeloPagamentoAsync(pedido);
                await _regrasPosPagamento.ExecutarRegrasPosPagamentoAsync(pedido);
                await pedidosRepositorio.IncluirAsync(pedido);
                await unitOfWork.CommitAsync();
                return Ok(pagamento);
            }
        }
    }

    
}
