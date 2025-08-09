using lugiatrack_api.Data;
using lugiatrack_api.Dtos;
using lugiatrack_api.Models;
using lugiatrack_api.Validators;
using Microsoft.EntityFrameworkCore;

namespace lugiatrack_api.Endpoints;

public static class FuncionarioEndpoints
{
    public static void MapFuncionarioEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/funcionarios").WithTags("Funcionarios");

        group.MapGet("", GetAll);
        group.MapGet("/{id}", GetById);
        group.MapPost("/login", Login);
        group.MapPost("", Create);
        group.MapPut("/{id}", Update);
        group.MapDelete("/{id}", Delete);
    }

    private static async Task<IResult> GetAll(OracleDbContext db, int page = 1, int pageSize = 10)
    {
        if (!PaginationValid(page, pageSize))
            return Results.BadRequest("Parâmetros de paginação inválidos.");

        var total = await db.Funcionarios.CountAsync();
        var funcionarios = await db.Funcionarios
            .OrderBy(f => f.IdFuncionario)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FuncionarioReadDto(f.IdFuncionario, f.Nome, f.Email, f.Cpf, f.Cargo))
            .ToListAsync();

        return Results.Ok(new { Page = page, PageSize = pageSize, Total = total, Items = funcionarios });
    }

    private static async Task<IResult> GetById(int id, OracleDbContext db)
    {
        if (id <= 0) return Results.BadRequest("ID inválido.");

        var f = await db.Funcionarios.FindAsync(id);
        return f is null
            ? Results.NotFound($"Funcionário com ID {id} não encontrado.")
            : Results.Ok(new FuncionarioReadDto(f.IdFuncionario, f.Nome, f.Email, f.Cpf, f.Cargo));
    }

    private static async Task<IResult> Login(LoginRequestDto login, OracleDbContext db)
    {
        if (!LoginValidator.CamposPreenchidos(login.Email, login.Senha))
            return Results.BadRequest("Email e senha são obrigatórios.");

        if (!LoginValidator.EmailValido(login.Email))
            return Results.BadRequest("Formato de email inválido.");

        var funcionario = await db.Funcionarios.FirstOrDefaultAsync(f => f.Email == login.Email && f.Senha == login.Senha);
        return funcionario is null
            ? Results.Json(new { error = "Email ou senha incorretos" }, statusCode: StatusCodes.Status401Unauthorized)
            : Results.Ok(new FuncionarioReadDto(funcionario.IdFuncionario, funcionario.Nome, funcionario.Email, funcionario.Cpf, funcionario.Cargo));
    }

    private static async Task<IResult> Create(FuncionarioCreateDto dto, OracleDbContext db)
    {
        if (!FuncionarioValidator.CamposObrigatoriosPreenchidos(dto.Nome, dto.Email, dto.Cpf) || string.IsNullOrWhiteSpace(dto.Senha))
            return Results.BadRequest("Todos os campos obrigatórios devem ser preenchidos.");

        if (!FuncionarioValidator.EmailValido(dto.Email))
            return Results.BadRequest("Email inválido.");

        if (!FuncionarioValidator.CpfValido(dto.Cpf))
            return Results.BadRequest("CPF deve conter exatamente 11 dígitos numéricos (sem pontuação).");

        if (await db.Funcionarios.AnyAsync(f => f.Cpf == dto.Cpf))
            return Results.BadRequest("Já existe um funcionário cadastrado com esse CPF.");

        var funcionario = new Funcionario
        {
            Nome = dto.Nome,
            Senha = dto.Senha,
            Email = dto.Email,
            Cpf = dto.Cpf,
            Cargo = dto.Cargo
        };

        db.Funcionarios.Add(funcionario);
        await db.SaveChangesAsync();

        return Results.Created($"/funcionarios/{funcionario.IdFuncionario}", new FuncionarioReadDto(funcionario.IdFuncionario, funcionario.Nome, funcionario.Email, funcionario.Cpf, funcionario.Cargo));
    }

    private static async Task<IResult> Update(int id, FuncionarioUpdateDto dto, OracleDbContext db)
    {
        if (id <= 0) return Results.BadRequest("ID inválido.");

        if (!FuncionarioValidator.CamposObrigatoriosPreenchidos(dto.Nome, dto.Email, dto.Cpf))
            return Results.BadRequest("Nome, email e CPF são obrigatórios.");

        if (!FuncionarioValidator.EmailValido(dto.Email))
            return Results.BadRequest("Email inválido.");

        if (!FuncionarioValidator.CpfValido(dto.Cpf))
            return Results.BadRequest("CPF deve conter exatamente 11 dígitos numéricos (sem pontuação).");

        var funcionario = await db.Funcionarios.FindAsync(id);
        if (funcionario is null)
            return Results.NotFound($"Funcionário com ID {id} não encontrado.");

        funcionario.Nome = dto.Nome;
        funcionario.Email = dto.Email;
        funcionario.Cpf = dto.Cpf;
        funcionario.Cargo = dto.Cargo;

        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(int id, OracleDbContext db)
    {
        if (id <= 0) return Results.BadRequest("ID inválido.");

        var funcionario = await db.Funcionarios.FindAsync(id);
        if (funcionario is null)
            return Results.NotFound($"Funcionário com ID {id} não encontrado.");

        db.Funcionarios.Remove(funcionario);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static bool PaginationValid(int page, int pageSize) => page > 0 && pageSize > 0;
}