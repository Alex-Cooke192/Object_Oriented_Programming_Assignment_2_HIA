using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JIDS.Data
{
    public class JetDbContextFactory : IDesignTimeDbContextFactory<JetDbContext>
    {
        public JetDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<JetDbContext>();

            // Use SQLite (adjust the path as needed)
            optionsBuilder.UseSqlite("Data Source=jetConfigs.db");

            return new JetDbContext(optionsBuilder.Options);
        }
    }
}