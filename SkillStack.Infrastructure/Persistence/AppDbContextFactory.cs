using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SkillStack.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Ajustar o caminho para apontar para o diretório correto
            var configuration = new ConfigurationBuilder()
                // Define o caminho base como o diretório do projeto SkillStack.API
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)  // Vai um nível acima para o diretório SkillStack.BackEnd
                .AddJsonFile("SkillStack.API/appsettings.Development.json", optional: false, reloadOnChange: true) // Caminho do arquivo JSON
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString); // Configura o PostgreSQL

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
