using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Persistence;
using Learning.Architecture.Tests.Contracts;

namespace Learning.Testing.PostgresTests;

[Trait("Category", "Postgres")]
public sealed class PostgresOfferingStoreTests : CourseOfferingStoreContractTests
{
    protected override async Task<StoreLease> OpenAsync(CourseOffering initial)
    {
        PostgresDatabase database = await PostgresDatabase.CreateAsync(initial);
        EnrollmentDbContext context = database.CreateContext();
        return new StoreLease(new EfCourseOfferingStore(context), context, database);
    }
}
