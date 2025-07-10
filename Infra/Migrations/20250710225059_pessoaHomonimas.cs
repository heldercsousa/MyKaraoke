using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKaraoke.Infra.Migrations
{
    /// <inheritdoc />
    public partial class pessoaHomonimas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaMesAniversario",
                table: "Pessoas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Pessoas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            // Atualiza dados existentes com aniversários fictícios para teste
            migrationBuilder.Sql(@"
                UPDATE Pessoas SET DiaMesAniversario = '15/03' WHERE NomeCompleto LIKE '%João Silva%';
                UPDATE Pessoas SET DiaMesAniversario = '22/07' WHERE NomeCompleto LIKE '%Maria José%';
                UPDATE Pessoas SET DiaMesAniversario = '10/12' WHERE NomeCompleto LIKE '%Pedro Santos%';
                UPDATE Pessoas SET DiaMesAniversario = '05/05' WHERE NomeCompleto LIKE '%José André%';
                UPDATE Pessoas SET DiaMesAniversario = '18/09' WHERE NomeCompleto LIKE '%Mônica%';
                UPDATE Pessoas SET DiaMesAniversario = '30/11' WHERE NomeCompleto LIKE '%João Paulo%';
                UPDATE Pessoas SET DiaMesAniversario = '14/02' WHERE NomeCompleto LIKE '%Carlos%';
                UPDATE Pessoas SET DiaMesAniversario = '25/12' WHERE NomeCompleto LIKE '%Rafael%';
                UPDATE Pessoas SET DiaMesAniversario = '01/06' WHERE NomeCompleto LIKE '%Beatriz%';
            ");

            // Adiciona alguns e-mails de exemplo (opcional)
            migrationBuilder.Sql(@"
                UPDATE Pessoas SET Email = 'joao.silva@exemplo.com' WHERE NomeCompleto LIKE '%João Silva%';
                UPDATE Pessoas SET Email = 'maria.jose@exemplo.com' WHERE NomeCompleto LIKE '%Maria José%';
                -- Outros ficam sem e-mail para simular usuários ""analfabytes""
            ");

            // Cria índice para busca por e-mail
            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Email",
                table: "Pessoas",
                column: "Email");

            // Cria índice para aniversário (para futura funcionalidade de parabéns)
            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_DiaMesAniversario",
                table: "Pessoas",
                column: "DiaMesAniversario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove índices
            migrationBuilder.DropIndex(
                name: "IX_Pessoas_Email",
                table: "Pessoas");

            migrationBuilder.DropIndex(
                name: "IX_Pessoas_DiaMesAniversario",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "DiaMesAniversario",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Pessoas");
        }
    }
}
