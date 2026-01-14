using System.Text.RegularExpressions;

namespace Utils
{
    public static class StringNormalizer
    {
        public static string Normalizar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            return Regex.Replace(
               valor.Trim().ToUpperInvariant(),
               @"[^A-Z0-9]",
               string.Empty
           );
        }
    }
}
