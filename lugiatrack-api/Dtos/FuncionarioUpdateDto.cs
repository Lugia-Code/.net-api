namespace lugiatrack_api.Dtos;

public record FuncionarioUpdateDto(
    string Nome,
    string Email,
    string Cpf,
    string? Cargo
);
