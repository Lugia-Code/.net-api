using lugiatrack_api.Data;
using lugiatrack_api.Dtos;
using lugiatrack_api.Models;
using lugiatrack_api.Validators;
using Microsoft.EntityFrameworkCore;

public static class MotosEndpoints
{
    public static void MapMotosEndpoints(this WebApplication app)
    {
        app.MapGet("/motos", GetMotos).WithTags("Motos");
        app.MapGet("/motos/buscar", GetMoto).WithTags("Motos");
        app.MapPost("/motos", CreateMoto).WithTags("Motos");
        app.MapPut("/motos/{chassi}/{placa}", UpdateMoto).WithTags("Motos");
        app.MapDelete("/motos", DeleteMoto).WithTags("Motos");
    }

    private static async Task<IResult> GetMotos(OracleDbContext db, int page = 1, int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
            return Results.BadRequest("Parâmetros de paginação inválidos.");

        if (pageSize > 100)
            pageSize = 100;

        var total = await db.Motos.CountAsync();
        var motos = await db.Motos
            .OrderBy(m => m.Chassi)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MotoReadDto(
                m.Chassi,
                m.Placa,
                m.Modelo,
                m.Status,
                m.Descricao
            ))
            .ToListAsync();

        return Results.Ok(new
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = motos
        });
    }

    private static async Task<IResult> GetMoto(string? chassi, string? placa, OracleDbContext db)
    {
        if (string.IsNullOrWhiteSpace(chassi) && string.IsNullOrWhiteSpace(placa))
            return Results.BadRequest("Informe ao menos o chassi ou a placa.");

        var moto = await db.Motos
            .FirstOrDefaultAsync(m =>
                (!string.IsNullOrWhiteSpace(chassi) && m.Chassi == chassi) ||
                (!string.IsNullOrWhiteSpace(placa) && m.Placa == placa));

        if (moto is null)
            return Results.NotFound("Moto não encontrada.");

        var dto = new MotoReadDto(
            moto.Chassi,
            moto.Placa,
            moto.Modelo,
            moto.Status,
            moto.Descricao
        );

        return Results.Ok(dto);
    }

    private static async Task<IResult> CreateMoto(MotoCreateDto dto, OracleDbContext db)
    {
        if (!MotoValidator.CamposObrigatoriosPreenchidos(dto.Chassi, dto.Placa, dto.Modelo))
            return Results.BadRequest("Chassi (17), placa (7) e modelo são obrigatórios e devem ser válidos.");

        var existe = await db.Motos.AnyAsync(m => m.Chassi == dto.Chassi || m.Placa == dto.Placa);
        if (existe)
            return Results.BadRequest("Já existe uma moto cadastrada com esse chassi ou placa.");

        var moto = new Moto
        {
            Chassi = dto.Chassi,
            Placa = dto.Placa,
            IdVaga = dto.IdVaga,
            Modelo = dto.Modelo,
            Status = dto.Status,
            Descricao = dto.Descricao
        };

        db.Motos.Add(moto);
        await db.SaveChangesAsync();

        return Results.Created($"/motos/{moto.Chassi}/{moto.Placa}", new MotoReadDto(
            moto.Chassi,
            moto.Placa,
            moto.Modelo,
            moto.Status,
            moto.Descricao
        ));
    }

    private static async Task<IResult> UpdateMoto(string chassi, string placa, MotoCreateDto dto, OracleDbContext db)
    {
        if (!MotoValidator.CamposObrigatoriosPreenchidos(dto.Chassi, dto.Placa, dto.Modelo))
            return Results.BadRequest("Chassi (17), placa (7) e modelo são obrigatórios e devem ser válidos.");

        if (dto.Chassi != chassi || dto.Placa != placa)
            return Results.BadRequest("O chassi e a placa do corpo devem ser iguais aos da URL.");

        var moto = await db.Motos.FindAsync(chassi, placa);
        if (moto is null)
            return Results.NotFound("Moto não encontrada.");

        moto.IdVaga = dto.IdVaga;
        moto.Modelo = dto.Modelo;
        moto.Status = dto.Status;
        moto.Descricao = dto.Descricao;

        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteMoto(string? chassi, string? placa, OracleDbContext db)
    {
        if (string.IsNullOrWhiteSpace(chassi) && string.IsNullOrWhiteSpace(placa))
            return Results.BadRequest("Informe ao menos o chassi ou a placa.");

        var moto = await db.Motos
            .FirstOrDefaultAsync(m =>
                (!string.IsNullOrWhiteSpace(chassi) && m.Chassi == chassi) ||
                (!string.IsNullOrWhiteSpace(placa) && m.Placa == placa));

        if (moto is null)
            return Results.NotFound("Moto não encontrada.");

        db.Motos.Remove(moto);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
