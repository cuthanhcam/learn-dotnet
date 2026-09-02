using Learning.Auth.Domain.Documents;

namespace Learning.Auth.Application.Abstractions;

public interface ILearningDocumentRepository
{
    Task AddAsync(LearningDocument document, CancellationToken cancellationToken);
    Task<LearningDocument?> FindAsync(Guid id, CancellationToken cancellationToken);
}
