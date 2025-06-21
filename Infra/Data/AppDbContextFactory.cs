using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design; // Necessário para IDesignTimeDbContextFactory

namespace MyKaraoke.Infra.Data
{
    // Esta fábrica é usada pelas ferramentas 'dotnet ef' para criar uma instância do DbContext
    // em tempo de design (quando você executa 'dotnet ef migrations add' ou 'dotnet ef database update').
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // ATENÇÃO: O caminho aqui DEVE ser um caminho absoluto para o arquivo DB
            // Para o design-time, o FileSystem.AppDataDirectory não está disponível.
            // Usaremos um caminho em um diretório temporário ou de usuário.
            // Para migrações, o arquivo DB não precisa existir, apenas o esquema é gerado.
            // No entanto, o EF Core precisa de uma string de conexão válida para o provedor.

            // Uma abordagem comum é usar Environment.GetFolderPath para um diretório de usuário
            // ou um diretório temporário no contexto do Windows/Linux/macOS da máquina de build.
            // Como esta é uma fábrica para design-time, o caminho pode ser simplificado.
            // Vamos usar um caminho que funcione em um ambiente de desenvolvimento.
            // Geralmente, o banco de dados é criado na pasta de saída do build do projeto Infra
            // ou em uma pasta de usuário para o design time.
            string basePath = AppContext.BaseDirectory; // Caminho base da execução da ferramenta
            string dbPath = Path.Combine(basePath, "FilaDeC_Design.db"); // Nome temporário ou de design

            optionsBuilder.UseSqlite($"Filename={dbPath}");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}