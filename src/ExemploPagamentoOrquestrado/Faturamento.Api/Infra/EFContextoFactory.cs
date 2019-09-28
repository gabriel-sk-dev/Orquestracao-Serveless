using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Faturamento.Api.Infra
{
    public class EFContextoFactory : IDesignTimeDbContextFactory<EFContexto>
    {
        public EFContexto CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EFContexto>();
            optionsBuilder.UseSqlServer("Server = tcp:azuredayserveless.database.windows.net, 1433; Initial Catalog = azureday; Persist Security Info = False; User ID = gskohlrausch; Password = azureday@2019; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;");

            return new EFContexto(optionsBuilder.Options);
        }
    }
}
