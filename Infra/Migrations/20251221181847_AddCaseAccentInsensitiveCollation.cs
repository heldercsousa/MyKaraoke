using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVocaList.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseAccentInsensitiveCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NomeCompletoNormalizado",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 250,
                nullable: true,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 250,
                oldNullable: true,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 250,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Pessoas",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "DiaMesAniversario",
                table: "Pessoas",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "NomeEvento",
                table: "Eventos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Estabelecimentos",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 30,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "ConfiguracoesSistema",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Chave",
                table: "ConfiguracoesSistema",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "NOCASE_NOACCENT",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NomeCompletoNormalizado",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 250,
                nullable: true,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 250,
                oldNullable: true,
                oldCollation: "NOCASE_NOACCENT");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 250,
                oldCollation: "NOCASE_NOACCENT");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Pessoas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE_NOACCENT");

            migrationBuilder.AlterColumn<string>(
                name: "DiaMesAniversario",
                table: "Pessoas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE_NOACCENT");

            migrationBuilder.AlterColumn<string>(
                name: "NomeEvento",
                table: "Eventos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldCollation: "NOCASE_NOACCENT");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Estabelecimentos",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 30,
                oldCollation: "NOCASE_NOACCENT");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "ConfiguracoesSistema",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldCollation: "NOCASE_NOACCENT");

            migrationBuilder.AlterColumn<string>(
                name: "Chave",
                table: "ConfiguracoesSistema",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldCollation: "NOCASE_NOACCENT");
        }
    }
}
