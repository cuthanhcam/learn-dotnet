using Learning.Architecture.Application.Offerings;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Courses;

namespace Learning.Architecture.Tests;

public sealed class OfferingQueryTests
{
    [Theory]
    [InlineData(-1, 20)]
    [InlineData(10001, 20)]
    [InlineData(0, 0)]
    [InlineData(0, 101)]
    public async Task InvalidPage_IsRejectedAtTheApplicationBoundary(int offset, int limit)
    {
        var handler = new ListOfferingsHandler(new InMemoryCourseOfferingStore());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await handler.HandleAsync(new ListOfferingsQuery(offset, limit)));
    }

    [Fact]
    public async Task Page_IsOrderedBoundedAndProjectsAvailableSeats()
    {
        var store = new InMemoryCourseOfferingStore();
        Guid first = Guid.Parse("00000000-0000-0000-0000-000000000001");
        Guid second = Guid.Parse("00000000-0000-0000-0000-000000000002");
        store.TryAdd(CourseOffering.Create(second, "Second", 2).Enroll(Guid.NewGuid()).Offering);
        store.TryAdd(CourseOffering.Create(first, "First", 3));
        OfferingPage page = await new ListOfferingsHandler(store).HandleAsync(new ListOfferingsQuery(1, 1));
        OfferingSummary item = Assert.Single(page.Items);
        Assert.Equal(second, item.Id);
        Assert.Equal(1, item.AvailableSeats);
        Assert.Equal(1, page.Offset);
    }

    [Fact]
    public async Task EmptyPage_IsNotAnError()
    {
        OfferingPage page = await new ListOfferingsHandler(new InMemoryCourseOfferingStore())
            .HandleAsync(new ListOfferingsQuery());
        Assert.Empty(page.Items);
    }
}
