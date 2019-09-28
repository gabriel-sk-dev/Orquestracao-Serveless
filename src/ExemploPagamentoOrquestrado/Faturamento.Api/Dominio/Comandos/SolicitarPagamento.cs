using Faturamento.Api.Infra;
using System.Threading.Tasks;

namespace Faturamento.Api.Dominio.Comandos
{
    public class SolicitarPagamentoComando
    {
        public SolicitarPagamentoComando(string processoId, int pedidoId, decimal valor, Pagador pagador)
        {
            ProcessoId = processoId;
            PedidoId = pedidoId;
            Valor = valor;
            Pagador = pagador;
        }

        public string ProcessoId { get; set; }
        public int PedidoId { get; set; }
        public decimal Valor { get; set; }
        public Pagador Pagador { get; set; }
    }

    public sealed class SolicitarPagamentoGerenciadorComando
    {
        private readonly EFContexto _contextoSql;
        private readonly PagamentoService _pagamentoService;

        public SolicitarPagamentoGerenciadorComando(
            EFContexto contextoSql,
            PagamentoService pagamentoService)
        {
            _contextoSql = contextoSql;
            _pagamentoService = pagamentoService;
        }

        public async Task<Pagamento> ExecutarAsync(SolicitarPagamentoComando comando)
        {
            var pagamento = Pagamento.Criar(comando.PedidoId, comando.Valor);
            var resultadoPagamento = await _pagamentoService.RealizarPagamentoAsync(pagamento, comando.Pagador);
            if (!resultadoPagamento.EhSucesso)
                pagamento.Erro();
            await _contextoSql.Pagamentos.AddAsync(pagamento);
            await _contextoSql.SaveChangesAsync();
            return pagamento;
        }
    }
}
