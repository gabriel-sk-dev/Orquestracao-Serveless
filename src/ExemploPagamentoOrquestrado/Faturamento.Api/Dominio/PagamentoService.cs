using Comum.CrossCutting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Faturamento.Api.Dominio
{
    public sealed class PagamentoService
    {
        private static HttpClient _cliente;

        public PagamentoService(string uri)
        {
            if (_cliente == null)
            {
                _cliente = new HttpClient();
                _cliente.BaseAddress = new Uri(uri);
            }
        }
        public async Task<Resultado<Pagamento>> RealizarPagamentoAsync(Pagamento pagamento, Pagador pagador)
        {
            try
            {
                _cliente.DefaultRequestHeaders.Accept.Clear();
                var dto = new
                {
                    Pagador = pagador,
                    Valor = pagamento.Valor,
                    PedidoId = pagamento.PedidoId
                };
                var conteudo = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8);
                conteudo.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var resposta = await _cliente.PostAsync("/api/Cobrancas", conteudo);
                if (!resposta.IsSuccessStatusCode)
                    return Falha.NovaFalha(Motivo.Novo("", ""));
                var json = await resposta.Content.ReadAsStringAsync();
                var retorno = JsonConvert.DeserializeObject<RetornoDTO>(json);
                if (retorno.Status == 0)
                    return pagamento.Aprovado(retorno.TransacaoId);
                if(retorno.Status == 1)
                    return pagamento.Negado(retorno.TransacaoId);
                return pagamento.EmAnalise(retorno.TransacaoId);
            }
            catch (Exception ex)
            {
                return Falha.NovaFalha(Motivo.Novo("FalhaAoRealizarCobranca", "Não foi possível realizar a transação"));
            }
        }

        public sealed class RetornoDTO
        {
            public string TransacaoId { get; set; }
            public int Status { get; set; }
        }
    }
}
