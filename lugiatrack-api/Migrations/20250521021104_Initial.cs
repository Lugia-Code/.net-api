using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lugiatrack_api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_funcionarios",
                columns: table => new
                {
                    id_funcionario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(70)", maxLength: 70, nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    cpf = table.Column<string>(type: "NVARCHAR2(11)", maxLength: 11, nullable: false),
                    cargo = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_funcionarios", x => x.id_funcionario);
                });

            migrationBuilder.CreateTable(
                name: "tbl_moto",
                columns: table => new
                {
                    chassi = table.Column<string>(type: "NVARCHAR2(17)", maxLength: 17, nullable: false),
                    placa = table.Column<string>(type: "NVARCHAR2(7)", maxLength: 7, nullable: false),
                    id_vaga = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    modelo = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_moto", x => new { x.chassi, x.placa });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_funcionarios");

            migrationBuilder.DropTable(
                name: "tbl_moto");
        }
    }
}
