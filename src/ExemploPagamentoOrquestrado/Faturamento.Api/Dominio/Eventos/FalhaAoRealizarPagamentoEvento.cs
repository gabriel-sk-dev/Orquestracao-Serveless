using Comum.CrossCutting;

namespace Faturamento.Api.Dominio.Eventos
{
    public sealed class FalhaAoRealizarPagamentoEvento
    {
        public FalhaAoRealizarPagamentoEvento(string processoId, int pedidoId, Falha falha )
        {
            PedidoId = pedidoId;
            Falha = falha;
            ProcessoId = processoId;
        }

        public string ProcessoId { get; set; }
        public int PedidoId { get; set; }
        public Falha Falha { get; set; }
    }
}
