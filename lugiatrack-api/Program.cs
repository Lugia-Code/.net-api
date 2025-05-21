using lugiatrack_api.Data;
using lugiatrack_api.Dtos;
using lugiatrack_api.Models;
using lugiatrack_api.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OracleDbContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Endpoints Funcionarios

// GET todos os funcionários (com paginação)
app.MapGet("/funcionarios", async (OracleDbContext db, int page = 1, int pageSize = 10) =>
{
    if (page <= 0 || pageSize <= 0)
        return Results.BadRequest("Parâmetros de paginação inválidos.");

    var total = await db.Funcionarios.CountAsync();
    var funcionarios = await db.Funcionarios
        .OrderBy(f => f.IdFuncionario)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(f => new FuncionarioReadDto(
            f.IdFuncionario,
            f.Nome,
            f.Email,
            f.Cpf,
            f.Cargo
        ))
        .ToListAsync();

    return Results.Ok(new
    {
        Page = page,
        PageSize = pageSize,
        Total = total,
        Items = funcionarios
    });
}).WithTags("Funcionarios");

// GET funcionário por ID
app.MapGet("/funcionarios/{id}", async (int id, OracleDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("ID inválido."); 

    var f = await db.Funcionarios.FindAsync(id);
    if (f is null)
        return Results.NotFound($"Funcionário com ID {id} não encontrado."); 

    var dto = new FuncionarioReadDto(
        f.IdFuncionario,
        f.Nome,
        f.Email,
        f.Cpf,
        f.Cargo
    );

    return Results.Ok(dto); 
}).WithTags("Funcionarios");

// POST (adicionar funcionário)
app.MapPost("/funcionarios", async (FuncionarioCreateDto dto, OracleDbContext db) =>
{
    if (!FuncionarioValidator.CamposObrigatoriosPreenchidos(dto.Nome, dto.Email, dto.Cpf) ||
        string.IsNullOrWhiteSpace(dto.Senha))
    {
        return Results.BadRequest("Todos os campos obrigatórios devem ser preenchidos.");
    }

    if (!FuncionarioValidator.EmailValido(dto.Email))
        return Results.BadRequest("Email inválido.");

    if (!FuncionarioValidator.CpfValido(dto.Cpf))
        return Results.BadRequest("CPF deve conter exatamente 11 dígitos numéricos (sem pontuação).");
    
    var cpfExiste = await db.Funcionarios.AnyAsync(f => f.Cpf == dto.Cpf);
    if (cpfExiste)
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

    var readDto = new FuncionarioReadDto(
        funcionario.IdFuncionario,
        funcionario.Nome,
        funcionario.Email,
        funcionario.Cpf,
        funcionario.Cargo
    );

    return Results.Created($"/funcionarios/{funcionario.IdFuncionario}", readDto);
}).WithTags("Funcionarios");

// PUT (atualizar funcionário)
app.MapPut("/funcionarios/{id}", async (int id, FuncionarioUpdateDto dto, OracleDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("ID inválido.");

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
}).WithTags("Funcionarios");

// DELETE
app.MapDelete("/funcionarios/{id}", async (int id, OracleDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("ID inválido."); 

    var funcionario = await db.Funcionarios.FindAsync(id);
    if (funcionario is null)
        return Results.NotFound($"Funcionário com ID {id} não encontrado."); 

    db.Funcionarios.Remove(funcionario);
    await db.SaveChangesAsync();
    return Results.NoContent(); 
}).WithTags("Funcionarios");

// Endpoints Motos

// GET todas as motos (com paginação)
app.MapGet("/motos", async (OracleDbContext db, int page = 1, int pageSize = 10) =>
{
    if (page <= 0 || pageSize <= 0)
        return Results.BadRequest("Parâmetros de paginação inválidos.");

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
}).WithTags("Motos");

// GET moto por chassi ou placa
app.MapGet("/motos/buscar", async (string? chassi, string? placa, OracleDbContext db) =>
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
}).WithTags("Motos");

// POST (adicionar moto)
app.MapPost("/motos", async (MotoCreateDto dto, OracleDbContext db) =>
{
    if (!MotoValidator.CamposObrigatoriosPreenchidos(dto.Chassi, dto.Placa, dto.Modelo))
        return Results.BadRequest("Chassi (17), placa (7) e modelo são obrigatórios e devem ser válidos.");

    var existeChassi = await db.Motos.CountAsync(m => m.Chassi == dto.Chassi) > 0;
    if (existeChassi)
        return Results.BadRequest("Já existe uma moto cadastrada com esse chassi.");

    var existePlaca = await db.Motos.CountAsync(m => m.Placa == dto.Placa) > 0;
    if (existePlaca)
        return Results.BadRequest("Já existe uma moto cadastrada com essa placa.");

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
}).WithTags("Motos");

// PUT (atualizar moto por chassi e placa)
app.MapPut("/motos/{chassi}/{placa}", async (string chassi, string placa, MotoCreateDto dto, OracleDbContext db) =>
{
    if (!MotoValidator.CamposObrigatoriosPreenchidos(dto.Chassi, dto.Placa, dto.Modelo))
        return Results.BadRequest("Chassi (17), placa (7) e modelo são obrigatórios e devem ser válidos.");

    var moto = await db.Motos.FindAsync(chassi, placa);
    if (moto is null)
        return Results.NotFound("Moto não encontrada.");

    moto.IdVaga = dto.IdVaga;
    moto.Modelo = dto.Modelo;
    moto.Status = dto.Status;
    moto.Descricao = dto.Descricao;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Motos");

// DELETE por chassi ou placa
app.MapDelete("/motos", async (string? chassi, string? placa, OracleDbContext db) =>
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
}).WithTags("Motos");


app.Run();