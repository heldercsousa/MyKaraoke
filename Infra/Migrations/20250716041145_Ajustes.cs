using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKaraoke.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Ajustes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Estabelecimentos",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldCollation: "NOCASE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Estabelecimentos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 30,
                oldCollation: "NOCASE");
        }
    }
}
