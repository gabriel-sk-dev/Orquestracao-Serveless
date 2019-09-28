using System;
using System.Collections.Generic;
using System.Linq;

namespace Faturamento.Api.Dominio
{
    public sealed class Pedido
    {
        private Pedido() { }
        public Pedido(int id, string processoId, DateTime criadoEm, int socioId, EStatus status, IList<Item> itens)
        {
            Id = id;
            ProcessoId = processoId;
            CriadoEm = criadoEm;
            SocioId = socioId;
            Itens = itens;
            Status = status;
        }

        public int Id { get; private set; }
        public string ProcessoId { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public int SocioId { get; private set; }
        public decimal ValorTotal => Itens == null ? 0 : Itens.Sum(i => i.Valor);
        public EStatus Status { get; private set; }
        public IList<Item> Itens { get; private set; }

        public static Pedido Criar(string processoId, int socioId, IList<Item> itens)
            => new Pedido(0, processoId, DateTime.UtcNow, socioId, EStatus.AguardandoPagamento, itens);
        
        public enum EStatus
        {
            AguardandoPagamento,
            Pago,
            Cancelado
        }

        public sealed class Item
        {
            private Item() { }
            public Item(int id, int dividaId, decimal valor)
            {
                Id = id;
                DividaId = dividaId;
                Valor = valor;
            }

            public int Id { get; private set; }
            public int DividaId { get; private set; }
            public decimal Valor { get; private set; }

            public static Item Criar(int dividaId, decimal valor)
                => new Item(0, dividaId, valor);
        }

        internal void Cancelado()
        {
            Status = EStatus.Cancelado;
        }

        internal void Pago()
        {
            Status = EStatus.Pago;
        }
    }
}
