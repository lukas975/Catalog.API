using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddCatalogContext(this IServiceCollection services, string connectionString)
        {
            return services.AddEntityFrameworkSqlServer().AddDbContext<CatalogContext>(contextOptions => { 
                contextOptions.UseSqlServer(connectionString, serverOptions => {
                    serverOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                });
            });
        }
    }
}
