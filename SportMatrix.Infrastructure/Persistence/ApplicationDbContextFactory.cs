using SportMatrix.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SportMatrix.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        DatabaseConfiguration.ConfigureDatabase(optionsBuilder, "Data Source=FitnessAnalytics.db");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
