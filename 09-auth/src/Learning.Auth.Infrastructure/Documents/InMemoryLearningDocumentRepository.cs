using System.Collections.Concurrent;
using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Documents;

namespace Learning.Auth.Infrastructure.Documents;

public sealed class InMemoryLearningDocumentRepository : ILearningDocumentRepository
{
    private readonly ConcurrentDictionary<Guid, LearningDocument> _documents = new();

    public Task AddAsync(LearningDocument document, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_documents.TryAdd(document.Id, document))
            throw new InvalidOperationException("A document identifier collision was detected.");
        return Task.CompletedTask;
    }

    public Task<LearningDocument?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _documents.TryGetValue(id, out LearningDocument? document);
        return Task.FromResult(document);
    }
}
