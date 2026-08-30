using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Learning.Persistence;

public sealed class DesignTimeLearningDbContextFactory : IDesignTimeDbContextFactory<LearningDbContext>
{
    public LearningDbContext CreateDbContext(string[] args)
    {
        // Design-time tooling needs a deterministic provider and connection without booting a web
        // host. This local file is for migration generation only, never a production secret source.
        var options = new DbContextOptionsBuilder<LearningDbContext>()
            .UseSqlite("Data Source=learning-design.db")
            .Options;
        return new LearningDbContext(options);
    }
}
