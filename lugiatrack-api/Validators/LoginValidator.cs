namespace lugiatrack_api.Validators;

public static class LoginValidator
{
    public static bool CamposPreenchidos(string email, string senha)
    {
        return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(senha);
    }

    public static bool EmailValido(string email)
    {
        return email.Contains('@') && email.Contains('.');
    }
}