using Microsoft.EntityFrameworkCore;

namespace LibertyRustAcquiring.Data.Extensions
{
    public static class DbExtensions
    {
        public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.MigrateAsync();
            SeedData.Init(dbContext);

            return app;
        }
    }
}
