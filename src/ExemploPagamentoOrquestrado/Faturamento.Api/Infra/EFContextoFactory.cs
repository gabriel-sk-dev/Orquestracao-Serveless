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
            optionsBuilder.UseSqlServer("Server = [ENDERECO], 1433; Initial Catalog = [BASE]; Persist Security Info = False; User ID = [USUARIO]; Password = [PASSWORD]; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;");

            return new EFContexto(optionsBuilder.Options);
        }
    }
}
