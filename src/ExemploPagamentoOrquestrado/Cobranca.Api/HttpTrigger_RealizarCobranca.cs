using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cobranca.Api
{
    public class HttpTrigger_RealizarCobranca
    {
        [FunctionName(nameof(HttpTrigger_RealizarCobranca))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Cobrancas")] HttpRequest requisicao,
            ILogger logger)
        {
            logger.LogInformation("Trigger HttpTrigger_ConfirmarPagamento recebida");
            var json = await new StreamReader(requisicao.Body).ReadToEndAsync();
            var dto = JsonConvert.DeserializeObject<PagamentoDTO>(json);
            if (dto.Valor > 1000)
                return new OkObjectResult(new RetornoDTO(Guid.NewGuid().ToString(), 2));
            if (dto.Valor > 200)
                return new OkObjectResult(new RetornoDTO(Guid.NewGuid().ToString(), 1));
            return new OkObjectResult(new RetornoDTO(Guid.NewGuid().ToString(), 0));
        }
    }

    public sealed class RetornoDTO
    {
        public RetornoDTO(string transacaoId, int status)
        {
            TransacaoId = transacaoId;
            Status = status;
        }

        public string TransacaoId { get; set; }
        public int Status { get; set; }
    }

    public sealed class PagamentoDTO
    {
        public PagadorDTO Pagador { get; set; }
        public decimal Valor { get; set; }
        public int PedidoId { get; set; }

        public sealed class PagadorDTO
        {
            public string Nome { get; set; }
            public string Cpf { get; set; }
            public string Email { get; set; }
            public string TokenCartao { get; set; }
        }
    }
}
