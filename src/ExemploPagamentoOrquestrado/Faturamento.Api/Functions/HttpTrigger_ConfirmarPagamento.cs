using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Faturamento.Api.Functions
{
    public class HttpTrigger_ConfirmarPagamento
    {
        [FunctionName(nameof(HttpTrigger_ConfirmarPagamento))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Faturamento/Pagamentos")] HttpRequest requisicao,
            [OrchestrationClient] DurableOrchestrationClientBase workflow,
            ILogger logger)
        {
            logger.LogInformation("Trigger HttpTrigger_ConfirmarPagamento recebida");
            var json = await new StreamReader(requisicao.Body).ReadToEndAsync();
            var dto = JsonConvert.DeserializeObject<NovoPagamentoDTO>(json);
            var processoId = await workflow.StartNewAsync(nameof(WorkFlow_ConfirmarPagamento), dto);
            return new OkObjectResult( workflow.CreateHttpManagementPayload(processoId));
        }
    }

    public sealed class NovoPagamentoDTO
    {
        public string ProcessoId { get; set; }
        public int SocioId { get; set; }
        public PagadorDTO Pagador { get; set; }
        public ItemDTO[] Itens { get; set; }

        public sealed class PagadorDTO
        {
            public string Nome { get; set; }
            public string Cpf { get; set; }
            public string Email { get; set; }
            public string TokenCartao { get; set; }
        }

        public sealed class ItemDTO
        {
            public int DividaId { get; set; }
            public decimal Valor { get; set; }
        }
    }
}
