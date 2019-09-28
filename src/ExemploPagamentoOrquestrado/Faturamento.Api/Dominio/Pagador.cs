namespace Faturamento.Api.Dominio
{
    public sealed class Pagador
    {
        public Pagador(string nome, string cpf, string email, string tokenCartao)
        {
            Nome = nome;
            Cpf = cpf;
            Email = email;
            TokenCartao = tokenCartao;
        }

        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get; private set; }
        public string TokenCartao { get; private set; }
    }
}
