namespace lugiatrack_api.Dtos;

public record FuncionarioCreateDto(
    string Nome,
    string Senha,
    string Email,
    string Cpf,
    string? Cargo
);
