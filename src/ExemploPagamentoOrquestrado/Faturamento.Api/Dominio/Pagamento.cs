using System;

namespace Faturamento.Api.Dominio
{
    public sealed class Pagamento
    {
        private Pagamento() { }
        public Pagamento(int id, string transacaoId, DateTime criadoEm, int pedidoId, decimal valor, EStatus status)
        {
            Id = id;
            TransacaoId = transacaoId;
            CriadoEm = criadoEm;
            PedidoId = pedidoId;
            Valor = valor;
            Status = status;
        }

        public int Id { get; private set; }
        public string TransacaoId { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public int PedidoId { get; private set; }
        public decimal Valor { get; private set; }
        public EStatus Status { get; private set; }

        public static Pagamento Criar(int pedidoId, decimal valor)
            => new Pagamento(0, "", DateTime.UtcNow, pedidoId, valor, EStatus.Criado);

        public enum EStatus
        {
            Criado,
            EmAnalise,
            Aprovado,
            Negado,
            Erro
        }

        internal Pagamento Aprovado(string transacaoId)
        {
            TransacaoId = transacaoId;
            Status = EStatus.Aprovado;
            return this;
        }

        internal Pagamento Negado(string transacaoId)
        {
            TransacaoId = transacaoId;
            Status = EStatus.Negado;
            return this;
        }

        internal Pagamento EmAnalise(string transacaoId)
        {
            TransacaoId = transacaoId;
            Status = EStatus.EmAnalise;
            return this;
        }

        internal void Erro()
        {
            Status = EStatus.Erro;
        }
    }
}
