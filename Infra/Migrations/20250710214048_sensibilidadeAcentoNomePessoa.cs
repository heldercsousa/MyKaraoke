using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKaraoke.Infra.Migrations
{
    /// <inheritdoc />
    public partial class sensibilidadeAcentoNomePessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldCollation: "NOCASE");

            migrationBuilder.AddColumn<string>(
                name: "NomeCompletoNormalizado",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 250,
                nullable: true,
                collation: "NOCASE");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_NomeCompletoNormalizado",
                table: "Pessoas",
                column: "NomeCompletoNormalizado");

            // 3. Remove dados de teste antigos
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
                   OR NomeCompleto LIKE '%Testador%'
                   OR NomeCompleto LIKE '%Mockão%'
                   OR NomeCompleto LIKE '%Tesão%'
                   OR NomeCompleto LIKE '%Débugger%'
                   OR NomeCompleto LIKE '%Español%'
                   OR NomeCompleto LIKE '%Testführer%'
                   OR NomeCompleto LIKE '%Testovich%'
                   OR NomeCompleto LIKE '%Test%'
                   OR NomeCompleto LIKE '%Debug%'
                   OR NomeCompleto LIKE '%Mock%'
            ");

            // 4. Adiciona dados de teste com nomes MUITO LONGOS para testar limites
            migrationBuilder.Sql(@"
                INSERT INTO Pessoas (NomeCompleto, NomeCompletoNormalizado, Participacoes, Ausencias)
                VALUES
                -- Nomes normais para teste básico
                ('João Silva Testador', 'joao silva testador', 0, 0),
                ('Maria José Testeira', 'maria jose testeira', 0, 0),
                ('Pedro Santos Fakerson', 'pedro santos fakerson', 0, 0),
                
                -- Português com acentos
                ('José André Coração Debugger', 'jose andre coracao debugger', 0, 0),
                ('Mônica Lição da Cruz Placeholder', 'monica licao da cruz placeholder', 0, 0),
                ('João Paulo Ação Santos Sampleton', 'joao paulo acao santos sampleton', 0, 0),
                
                -- Francês
                ('François Marie-Claude Débugger', 'francois marie-claude debugger', 0, 0),
                ('Geneviève Amélie Cœur Testcase', 'genevieve amelie coeur testcase', 0, 0),
                
                -- Alemão
                ('Müller Hans Günther Testführer', 'muller hans gunther testfuhrer', 0, 0),
                ('Björn Søren Åse Devtools', 'bjorn soren ase devtools', 0, 0),
                
                -- Espanhol
                ('José María González Niño Unitest', 'jose maria gonzalez nino unitest', 0, 0),
                ('María Piñata Corazón Mockington', 'maria pinata corazon mockington', 0, 0),
                
                -- Russo (transliteração)
                ('Владимир Иванович Testovich', 'vladimir ivanovich testovich', 0, 0),
                ('Екатерина Александровна Demostra', 'ekaterina aleksandrovna demostra', 0, 0),
                
                -- Asiáticos (mantém caracteres originais)
                ('田中 太郎 Test-San', '田中 太郎 test-san', 0, 0),
                ('李小明 Wang Debug-Ming', '李小明 wang debug-ming', 0, 0),
                ('김철수 Park Mock-Soo', '김철수 park mock-soo', 0, 0),
                
                -- Árabe (mantém script)
                ('محمد أحمد Test-عربي', 'محمد أحمد test-عربي', 0, 0),
                ('فاطمة علي Debug-فاطمة', 'فاطمة علي debug-فاطمة', 0, 0),
                
                -- Hindi (mantém script)
                ('राज कुमार Test-हिंदी', 'राज कुमार test-हिंदी', 0, 0),
                ('सुनीता शर्मा Mock-शर्मा', 'सुनीता शर्मा mock-शर्मा', 0, 0),
                
                -- NOMES EXTREMAMENTE LONGOS para testar limite (próximo de 200 chars)
                ('Maria Antonieta Josefina dos Santos Silva Oliveira Pereira da Costa Lima Ferreira Alves Rodrigues Testeira Muito Longa Para Testar Limite Maximum', 'maria antonieta josefina dos santos silva oliveira pereira da costa lima ferreira alves rodrigues testeira muito longa para testar limite maximum', 0, 0),
                
                ('Jean-François Marie-Claude Pierre-Antoine de la Montagne Débugger Extraordinaire du Château de Versailles Test Case Ultra Long', 'jean-francois marie-claude pierre-antoine de la montagne debugger extraordinaire du chateau de versailles test case ultra long', 0, 0),
                
                ('Владимир Александрович Николаевич Дмитриевич Testovich-Extraordinaire-Длинный-Maximum-Length-Name-Test-Case', 'vladimir aleksandrovich nikolaevich dmitrievich testovich-extraordinaire-dlinnyy-maximum-length-name-test-case', 0, 0),
                
                -- Nome que atinge quase 200 caracteres (para testar limite do input)
                ('José María González Fernández de la Torre y Pérez de los Santos Rodríguez-Martínez Test-Case-Maximum-Input-Length-Almost-200-Characters-Exactly', 'jose maria gonzalez fernandez de la torre y perez de los santos rodriguez-martinez test-case-maximum-input-length-almost-200-characters-exactly', 0, 0)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pessoas_NomeCompletoNormalizado",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "NomeCompletoNormalizado",
                table: "Pessoas");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Pessoas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 250,
                oldCollation: "NOCASE");

            // Remove todos os dados de teste
            migrationBuilder.Sql(@"
                DELETE FROM Pessoas 
                WHERE NomeCompleto LIKE '%Test%'
                   OR NomeCompleto LIKE '%Debug%'
                   OR NomeCompleto LIKE '%Mock%'
                   OR NomeCompleto LIKE '%Testeira%'
                   OR NomeCompleto LIKE '%Fakerson%'
                   OR NomeCompleto LIKE '%Placeholder%'
                   OR NomeCompleto LIKE '%Sampleton%'
                   OR NomeCompleto LIKE '%Devtools%'
                   OR NomeCompleto LIKE '%Unitest%'
                   OR NomeCompleto LIKE '%Mockington%'
                   OR NomeCompleto LIKE '%Demostra%'
                   OR NomeCompleto LIKE '%Testcase%'
                   OR NomeCompleto LIKE '%Maximum%'
                   OR NomeCompleto LIKE '%Long%'
                   OR NomeCompleto LIKE '%Length%'
            ");
        }
    }
}
