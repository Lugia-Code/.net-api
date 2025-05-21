namespace lugiatrack_api.Dtos;

public record MotoReadDto(
    string Chassi,
    string Placa,
    string Modelo,
    int Status,
    string? Descricao
);
