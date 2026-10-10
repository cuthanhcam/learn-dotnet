using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Courses;

// This is the composition root: only the host selects a concrete adapter. The handler depends on
// the application port, and the domain can be exercised without constructing a host or database.
var store = new InMemoryCourseOfferingStore();
CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Architecture with explicit boundaries", 1);
store.TryAdd(offering);
var handler = new EnrollLearnerHandler(store);
Guid learner = Guid.NewGuid();

Console.WriteLine(await handler.HandleAsync(new EnrollLearnerCommand(offering.Id, learner)));
Console.WriteLine(await handler.HandleAsync(new EnrollLearnerCommand(offering.Id, learner)));
Console.WriteLine(await handler.HandleAsync(new EnrollLearnerCommand(offering.Id, Guid.NewGuid())));
// Expected statuses: Enrolled, AlreadyEnrolled, Full. A duplicate never consumes another seat.
