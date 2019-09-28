using System;
using System.Collections.Generic;
using System.Linq;

namespace Comum.CrossCutting
{
    public class Falha
    {
        public Falha(string codigo, string mensagem, DateTime dataHoraUtc, IEnumerable<Motivo> motivos)
        {
            Codigo = codigo;
            Mensagem = mensagem;
            DataHoraUtc = dataHoraUtc;
            _motivos = motivos?.ToList() ?? new List<Motivo>();
        }

        public string Codigo { get; }
        public string Mensagem { get; }
        public DateTime DataHoraUtc { get; }

        private List<Motivo> _motivos;
        public IEnumerable<Motivo> Motivos => _motivos;

        public static Falha NovaFalha(Motivo motivo)
            => new Falha(motivo.Codigo, motivo.Mensagem, DateTime.UtcNow, Enumerable.Empty<Motivo>());


        public Falha EmRazaoDe(Motivo motivo)
        {
            _motivos.Add(motivo);
            return this;
        }

        public Falha EmRazaoDe(IEnumerable<Motivo> motivos)
        {
            _motivos.AddRange(motivos);
            return this;
        }

        public Falha EmRazaoDe(Falha falha)
        {
            if (falha.Motivos.Any())
                EmRazaoDe(falha.Motivos);
            else
                EmRazaoDe(Motivo.Novo(falha.Codigo, falha.Mensagem));
            return this;
        }

        public override string ToString()
            => Mensagem;
    }

    public struct Motivo : IComparable
    {
        public Motivo(string codigo, string mensagem)
        {
            Codigo = codigo;
            Mensagem = mensagem;
        }

        public string Codigo { get; }
        public string Mensagem { get; }

        public static Motivo Novo(string codigo, string mensagem)
            => new Motivo(codigo, mensagem);

        public int CompareTo(object obj)
        {
            var valueObject = (Motivo)obj;
            if (ReferenceEquals(valueObject, null))
                return 0;
            if (Codigo == valueObject.Codigo && Mensagem == valueObject.Mensagem)
                return 0;
            return Codigo.GetHashCode() > valueObject.Codigo.GetHashCode()
                ? 1
                : -1;
        }
    }
}
