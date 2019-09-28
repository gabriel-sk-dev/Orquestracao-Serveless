using STI.Compartilhado.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoDemo.WebApi.Util
{
    public sealed class PedidosRepositorio
    {
        private readonly IUnitOfWork _unitOfWork;

        public PedidosRepositorio(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task IncluirAsync(Pedido pedido)
        {
            return Task.Run(() => { });
        }

        public Task<Resultado<Pedido>> RecuperarAsync(int id)
        {
            return Task.Run<Resultado<Pedido>>(() => { return new Pedido(); });
        }
    }

    public sealed class ClientesRepositorio
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientesRepositorio(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Cliente> ObterAsync(string codigoCliente)
        {
            return null;
        }
    }

    public sealed class Cliente
    {

    }

    public sealed class Pedido
    {
        public static Pedido Criar(Cliente cliente, object[] dividas, object dadosPagamento) => new Pedido();
        public void RealizarPagamento(Pagamento pagamento)
        {

        }
    }

    public sealed class PedidoModel
    {
        public string CodigoCliente { get; set; }
        public object[] Dividas { get; set; }
        public object DadosPagamento { get; set; }
    }

    public interface IServicoEmail
    {
        Task EnviarObrigadoPeloPagamentoAsync(Pedido pedido);
    }

    public interface IRegrasPosPagamento
    {
        Task ExecutarRegrasPosPagamentoAsync(Pedido pedido);
    }

    public interface IServicoPagamento
    {
        Task<Pagamento> PagarAsync(Pedido pedido);
        Task CancelarAsync(Pedido pedido);
        Task<Pagamento> AutorizarPagamentoAsync(Pedido pedido);
        Task<Pagamento> CapturarPagamentoAsync(Pedido pedido);
    }

    public sealed class Pagamento
    {
        public bool Aprovado { get; set; }
        public object NegadoPor { get; set; }
        public static Pagamento NovoPagamento() => new Pagamento();
    }

    public interface IRequisicaoContexto
    {
        string CodigoTenant { get; }
    }

    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CriarParaTenant(string codigoTenant);
    }

    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync();
        Task RollBackAsync();
    }

    public interface ILogger
    {
        Task Error(string mensagem);
        Task Info(string mensagem);
    }
}

namespace PagamentoDemo.WebApi.UtilComResultado
{
    public sealed class PedidosRepositorio
    {
        private readonly IUnitOfWork _unitOfWork;

        public PedidosRepositorio(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<Pedido>> RecuperarAsync(int pedidoId)
        {
            return await Task.Run(() => { return new Pedido(); });
        }

        public Task IncluirOuAtualizarAsync(Pedido pedido)
        {
            return Task.Run(() => { });
        }
    }

    public sealed class ClientesRepositorio
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientesRepositorio(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Resultado<Cliente>> ObterAsync(string codigoCliente)
        {
            return null;
        }
    }

    public sealed class Cliente
    {

    }

    public sealed class Pedido
    {
        public int Id { get; set; }
        public static Pedido NovoPendente(Cliente cliente, object[] dividas, object dadosPagamento) => new Pedido();
        public void RealizarPagamento(Pagamento pagamento)
        {

        }
    }

    public sealed class PedidoModel
    {
        public string CodigoCliente { get; set; }
        public object[] Dividas { get; set; }
        public object DadosPagamento { get; set; }
    }

    public interface IServicoEmail
    {
        Task<Resultado> EnviarObrigadoPeloPagamentoAsync(Pedido pedido);
    }

    public interface IRegrasPosPagamento
    {
        Task<Resultado> ExecutarRegrasPosPagamentoAsync(Pedido pedido);
    }

    public interface IServicoPagamento
    {
        Task<Resultado<Pagamento>> PagarAsync(Pedido pedido);
        Task<Resultado> CancelarAsync(Pedido pedido);
        Task<Resultado<Pagamento>> AutorizarPagamentoAsync(Pedido pedido);
        Task<Resultado<Pagamento>> CapturarPagamentoAsync(Pedido pedido);
    }

    public interface IBus
    {
        Task Enviar(object comando);
    }

    public sealed class Pagamento
    {
        public bool Aprovado { get; set; }
        public object NegadoPor { get; set; }
        public static Pagamento NovoPagamento() => new Pagamento();
        public static Pagamento NovoPagamentoPendente() => new Pagamento();
    }

    public interface IRequisicaoContexto
    {
        string CodigoTenant { get; }
    }

    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CriarParaTenant(string codigoTenant);
    }

    public interface IUnitOfWork : IDisposable
    {
        Task<Resultado> CommitAsync();
        Task<Resultado> RollBackAsync();
    }

    public interface ILogger
    {
        Task Error(string mensagem);
        Task Info(string mensagem);
    }
}
