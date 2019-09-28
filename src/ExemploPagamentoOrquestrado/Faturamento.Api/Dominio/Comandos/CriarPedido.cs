using Faturamento.Api.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faturamento.Api.Dominio.Comandos
{
    public class CriarPedidoComando
    {
        public CriarPedidoComando(string processoId, int socioId, List<Item> itens)
        {
            ProcessoId = processoId;
            SocioId = socioId;
            Itens = itens;
        }

        public string ProcessoId { get; set; }
        public int SocioId { get; set; }
        public List<Item> Itens { get; set; }

        public sealed class Item
        {
            public Item(int dividaId, decimal valor)
            {
                DividaId = dividaId;
                Valor = valor;
            }

            public int DividaId { get; set; }
            public decimal Valor { get; set; }
        }
    }

    public sealed class CriarPedidoGerenciadorComando
    {
        private readonly EFContexto _contextoSql;

        public CriarPedidoGerenciadorComando(EFContexto contextoSql)
        {
            _contextoSql = contextoSql;
        }

        public async Task<Pedido> ExecutarAsync(CriarPedidoComando comando)
        {
            var pedido = Pedido.Criar(
                comando.ProcessoId, 
                comando.SocioId,
                comando.Itens.Select(i=> Pedido.Item.Criar(i.DividaId, i.Valor)).ToList());
            await _contextoSql.Pedidos.AddAsync(pedido);
            await _contextoSql.SaveChangesAsync();
            return pedido;
        }
    }
}
