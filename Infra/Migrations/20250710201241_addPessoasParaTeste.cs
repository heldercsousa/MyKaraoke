using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKaraoke.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addPessoasParaTeste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Pessoas (NomeCompleto, Participacoes, Ausencias)
                VALUES
                ('João Xpto', 0, 0),
                ('Maria Testeira', 0, 0),
                ('Pedro Fakerson', 0, 0),
                ('Ana Mocknha', 0, 0),
                ('Carlos Dummyson', 0, 0),
                ('Lucia Debugger', 0, 0),
                ('Rafael Placeholder', 0, 0),
                ('Beatriz Sampleton', 0, 0),
                ('Fernando Deletesson', 0, 0),
                ('Camila Tempdata', 0, 0),
                ('João Mockington', 0, 0),
                ('Maria Demostra', 0, 0),
                ('Pedro Testcase', 0, 0),
                ('Ana Devtools', 0, 0),
                ('João Unitest', 0, 0)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove todos os dados de teste usando pattern de sobrenomes esquisitos
            migrationBuilder.Sql(@"
                DELETE FROM Pessoas 
                WHERE NomeCompleto LIKE '%Xpto%'
                   OR NomeCompleto LIKE '%Testeira%'
                   OR NomeCompleto LIKE '%Fakerson%'
                   OR NomeCompleto LIKE '%Mocknha%'
                   OR NomeCompleto LIKE '%Dummyson%'
                   OR NomeCompleto LIKE '%Debugger%'
                   OR NomeCompleto LIKE '%Placeholder%'
                   OR NomeCompleto LIKE '%Sampleton%'
                   OR NomeCompleto LIKE '%Deletesson%'
                   OR NomeCompleto LIKE '%Tempdata%'
                   OR NomeCompleto LIKE '%Mockington%'
                   OR NomeCompleto LIKE '%Demostra%'
                   OR NomeCompleto LIKE '%Testcase%'
                   OR NomeCompleto LIKE '%Devtools%'
                   OR NomeCompleto LIKE '%Unitest%'
            ");
        }
    }
}