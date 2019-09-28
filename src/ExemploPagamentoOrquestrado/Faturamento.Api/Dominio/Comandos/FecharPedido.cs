using Comum.CrossCutting;
using Faturamento.Api.Infra;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Faturamento.Api.Dominio.Comandos
{
    public sealed class FecharPedidoComando
    {
        public FecharPedidoComando(string processoId, int pedidoId, int pagamentoId)
        {
            ProcessoId = processoId;
            PedidoId = pedidoId;
            PagamentoId = pagamentoId;
        }

        public string ProcessoId { get; set; }
        public int PedidoId { get; set; }
        public int PagamentoId { get; set; }
    }

    public sealed class FecharPedidoGerenciadorComando
    {
        private readonly EFContexto _contextoSql;

        public FecharPedidoGerenciadorComando(EFContexto contextoSql)
        {
            _contextoSql = contextoSql;
        }

        public async Task<Resultado<Pedido>> ExecutarAsync(FecharPedidoComando comando)
        {
            var pedido = await _contextoSql.Pedidos.Include(c => c.Itens).FirstOrDefaultAsync(c=> c.Id== comando.PedidoId);
            var pagamento = await _contextoSql.Pagamentos.FindAsync(comando.PagamentoId);
            if (pagamento.Status == Pagamento.EStatus.EmAnalise)
                return Falha.NovaFalha(Motivo.Novo("ErroAoFecharPedido", "Pagamento está em análise"));
            if (pagamento.Status == Pagamento.EStatus.Aprovado)
                pedido.Pago();
            if (pagamento.Status == Pagamento.EStatus.Negado)
                pedido.Cancelado();
            await _contextoSql.SaveChangesAsync();
            return pedido;
        }
    }
}
