using System.Text.Json.Serialization;

namespace lugiatrack_api.Dtos;

public record FuncionarioReadDto(
    [property: JsonPropertyName("id_funcionario")] int IdFuncionario,
    string Nome,
    string Email,
    string Cpf,
    string? Cargo
);
