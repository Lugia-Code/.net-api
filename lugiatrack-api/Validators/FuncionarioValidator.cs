using System.Text.RegularExpressions;

namespace lugiatrack_api.Validators;

public static class FuncionarioValidator
{
    public static bool EmailValido(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public static bool CpfValido(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        return Regex.IsMatch(cpf, @"^\d{11}$");
    }

    public static bool CamposObrigatoriosPreenchidos(string nome, string email, string cpf)
    {
        return !(string.IsNullOrWhiteSpace(nome) ||
                 string.IsNullOrWhiteSpace(email) ||
                 string.IsNullOrWhiteSpace(cpf));
    }
}