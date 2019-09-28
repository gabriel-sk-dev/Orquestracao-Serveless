namespace Faturamento.Api.Dominio.Eventos
{
    public sealed class PagamentoFalhouEvento
    {
        public PagamentoFalhouEvento(string processoId, int pedidoId, int pagamentoId )
        {
            PedidoId = pedidoId;
            PagamentoId = pagamentoId;
            ProcessoId = processoId;
        }

        public string ProcessoId { get; set; }
        public int PedidoId { get; set; }
        public int PagamentoId { get; set; }
    }
}
