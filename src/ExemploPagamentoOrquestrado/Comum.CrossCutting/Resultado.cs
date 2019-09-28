namespace Comum.CrossCutting
{
    public sealed class Resultado<T>
    {
        public Resultado(bool ehSucesso, T sucesso, Falha falha)
        {
            EhSucesso = ehSucesso;
            Sucesso = sucesso;
            Falha = falha;
        }

        public bool EhSucesso { get; }
        public T Sucesso { get; }
        public Falha Falha { get; }

        public static implicit operator Resultado<T>(Falha falha)
            => new Resultado<T>(false, default(T), falha);

        public static implicit operator Resultado<T>(T sucesso)
            => new Resultado<T>(true, sucesso, Falha.NovaFalha(Motivo.Novo("", "")));
    }

    public sealed class Resultado
    {
        public Resultado(bool sucesso, Falha falha)
        {
            Sucesso = sucesso;
            Falha = falha;
        }

        public bool Sucesso { get; }
        public Falha Falha { get; }

        public static Resultado Ok()
            => new Resultado(true, Falha.NovaFalha(Motivo.Novo("", "")));

        public static Resultado<T> Ok<T>(T resultado)
            => new Resultado<T>(true, resultado, Falha.NovaFalha(Motivo.Novo("", "")));

        public static Resultado Error(Falha falha)
            => falha;

        public static implicit operator Resultado(Falha falha)
            => new Resultado(false, falha);
    }
}
