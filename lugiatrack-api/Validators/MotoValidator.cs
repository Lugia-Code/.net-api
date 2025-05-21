using lugiatrack_api.Dtos;

namespace lugiatrack_api.Validators;

public static class MotoValidator
{
    public static bool IsValidChassi(string? chassi)
    {
        return !string.IsNullOrWhiteSpace(chassi) && chassi.Length == 17;
    }

    public static bool IsValidPlaca(string? placa)
    {
        return !string.IsNullOrWhiteSpace(placa) && placa.Length == 7;
    }

    public static bool CamposObrigatoriosPreenchidos(string? chassi, string? placa, string? modelo)
    {
        return IsValidChassi(chassi) &&
               IsValidPlaca(placa) &&
               !string.IsNullOrWhiteSpace(modelo);
    }
}
