using System.Text.Json.Serialization;

namespace lugiatrack_api.Dtos;

public record MotoCreateDto(
    string Chassi,
    string Placa,
    [property: JsonPropertyName("id_vaga")] int IdVaga,
    string Modelo,
    int Status,
    string? Descricao
);

