using System.Text.RegularExpressions;

namespace Api.AppDoar.Utils
{
    public static class Auxiliaries
    {
        public static string FormatarDocumento(string tipoDocumento, string documento)
        {
            if (string.IsNullOrWhiteSpace(tipoDocumento) || string.IsNullOrWhiteSpace(documento))
                return documento;

            documento = Regex.Replace(documento, @"[^\d]", ""); // remove não numéricos

            return tipoDocumento.ToLower() switch
            {
                "cpf" => Regex.Replace(documento, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4"),
                "rg" or "rne" or "crnm" => Regex.Replace(documento, @"^(\d{2})(\d{3})(\d{3})(\d{1})$", "$1.$2.$3-$4"),
                "cnpj" => Regex.Replace(documento, @"(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})", "$1.$2.$3/$4-$5"),
                "caepf" => Regex.Replace(documento, @"^(\d{3})(\d{3})(\d{3})(\d{3})(\d{2})$", "$1.$2.$3/$4-$5"),
                _ => documento
            };
        }

        public static string FormatarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return telefone;

            telefone = Regex.Replace(telefone, @"[^\d]", "");

            return telefone.Length switch
            {
                10 => Regex.Replace(telefone, @"(\d{2})(\d{4})(\d{4})", "($1) $2-$3"),
                11 => Regex.Replace(telefone, @"(\d{2})(\d{5})(\d{4})", "($1) $2-$3"),
                _ => telefone
            };
        }

        public static string FormatarCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return cep;

            cep = Regex.Replace(cep, @"[^\d]", "");

            return Regex.Replace(cep, @"(\d{5})(\d{3})", "$1-$2");
        }
    }

}
