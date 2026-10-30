namespace Learning.Patterns.Structural.Decorator.Contracts;

public interface IQuoteRetryDelay
{
    ValueTask WaitAsync(CancellationToken cancellationToken);
}
