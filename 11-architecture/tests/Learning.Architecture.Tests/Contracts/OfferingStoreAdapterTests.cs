using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Courses;
using Learning.Architecture.Infrastructure.Persistence;
using Learning.Architecture.Tests.Persistence;

namespace Learning.Architecture.Tests.Contracts;

public sealed class InMemoryOfferingStoreContractTests : CourseOfferingStoreContractTests
{
    protected override Task<StoreLease> OpenAsync(CourseOffering initial)
    {
        var store = new InMemoryCourseOfferingStore();
        store.TryAdd(initial);
        return Task.FromResult(new StoreLease(store));
    }
}

public sealed class SqliteOfferingStoreContractTests : CourseOfferingStoreContractTests
{
    protected override async Task<StoreLease> OpenAsync(CourseOffering initial)
    {
        EnrollmentDatabase database = await EnrollmentDatabase.CreateAsync(initial);
        EnrollmentDbContext context = database.CreateContext();
        return new StoreLease(new EfCourseOfferingStore(context), context, database);
    }
}
