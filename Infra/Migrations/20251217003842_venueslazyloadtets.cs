using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace MyVocaList.Infra.Migrations
{
    /// <inheritdoc />
    public partial class venueslazyloadtets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            //Generate some ACCENTED and equivalent NO-ACCENTED entries for testing
            var accentNames = new[]
            {
                "Café Central",
                "São Paulo Bistro",
                "Japão Grill",
                "Cafe Bistro"
            };

            for (int i = 0; i < accentNames.Length; i++)
            {
                sqlBuilder.AppendLine($"INSERT INTO Estabelecimentos (Nome) VALUES ('{accentNames[i]}');");
            }

            // Generate 500 venues for testing pagination]
            for (int i = 1; i <= 500; i++)
            {
                // Escape single quotes if necessary, though simpler here
                sqlBuilder.AppendLine($"INSERT INTO Estabelecimentos (Nome) VALUES ('Venue {i}');");
            }

            migrationBuilder.Sql(sqlBuilder.ToString());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Estabelecimentos");
        }
    }
}
