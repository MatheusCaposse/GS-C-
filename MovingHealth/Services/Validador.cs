using System.Text.RegularExpressions;

namespace MovingHealth.Services;

public static class Validador
{
    // Retorna a mensagem de erro, ou null quando está tudo certo
    public static string? Validar(string nome, int idade)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return "Digite o nome do paciente.";

        if (nome.Trim().Length < 3)
            return "O nome precisa ter pelo menos 3 letras.";

        if (!Regex.IsMatch(nome.Trim(), @"^[\p{L} ']+$"))
            return "O nome deve ter apenas letras.";

        if (idade < 0 || idade > 120)
            return "Idade deve estar entre 0 e 120.";

        return null;
    }
}
