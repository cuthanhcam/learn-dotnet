namespace Learning.Auth.UnitTests;

public sealed class ArchitectureSmokeTests
{
    [Fact]
    public void DomainAssembly_HasNoProjectDependencies()
    {
        string[] references = typeof(Learning.Auth.Domain.AssemblyMarker).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(references, name => name.StartsWith("Learning.Auth.", StringComparison.Ordinal));
    }
}
