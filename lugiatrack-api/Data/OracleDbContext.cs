using lugiatrack_api.Models;
using Microsoft.EntityFrameworkCore;

namespace lugiatrack_api.Data;

public class OracleDbContext : DbContext
{
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<Moto> Motos { get; set; }

    public string? ConnectionString { get; set; }

    public OracleDbContext()
    {
        ConnectionString = System.Configuration.ConfigurationManager
                               .AppSettings["FiapOracleDb"] ?? 
                           "Data Source=//oracle.fiap.com.br:1521/orcl;User Id=rm554854; Password=090304;";
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseOracle(ConnectionString);
}