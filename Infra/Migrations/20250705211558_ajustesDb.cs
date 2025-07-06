using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyKaraoke.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ajustesDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Estabelecimentos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Estabelecimentos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Pessoas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "NomeEvento",
                table: "Eventos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Estabelecimentos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "ConfiguracoesSistema",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Chave",
                table: "ConfiguracoesSistema",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "NomeEvento",
                table: "Eventos",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Estabelecimentos",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "ConfiguracoesSistema",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Chave",
                table: "ConfiguracoesSistema",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                table: "Estabelecimentos",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Salão Principal" },
                    { 2, "Área Externa" }
                });
        }
    }
}
