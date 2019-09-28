using Faturamento.Api.Dominio;
using Faturamento.Api.Dominio.Comandos;
using Faturamento.Api.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Faturamento.Api.Startup))]
namespace Faturamento.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Testes
            string SqlConnection = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddDbContext<EFContexto>(options =>
            {
                options.UseSqlServer(SqlConnection);
            });
            //builder.Services.AddDbContext<EFContexto>(options =>
            //{
            //    options.UseSqlServer(SqlConnection, o => o.EnableRetryOnFailure());
            //});
            builder.Services.AddSingleton<PagamentoService>(new PagamentoService(@"https://azureday-serveless-cobranca.azurewebsites.net"));
            builder.Services.AddScoped<CriarPedidoGerenciadorComando>();
            builder.Services.AddScoped<SolicitarPagamentoGerenciadorComando>();
            builder.Services.AddScoped<FecharPedidoGerenciadorComando>();
        }
    }
}
