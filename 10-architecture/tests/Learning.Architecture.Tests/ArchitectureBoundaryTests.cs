using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Contracts;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Notifications;
using System.Xml.Linq;

namespace Learning.Architecture.Tests;

public sealed class ArchitectureBoundaryTests
{
    [Fact]
    public void Domain_HasNoApplicationInfrastructureOrTransportDependency()
    {
        string[] references = ReferencesOf(typeof(CourseOffering));
        Assert.DoesNotContain(references, name => name.StartsWith("Learning.Architecture.", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
    }

    [Fact]
    public void Application_OnlyReferencesDomainWithinTheSolution() => Assert.Equal(
        new[] { "Learning.Architecture.Domain" }, ProjectReferencesOf(typeof(EnrollLearnerHandler)));

    [Fact]
    public void Notifications_OnlyKnowsTheIntegrationContract() => Assert.Equal(
        new[] { "Learning.Architecture.Contracts" }, ProjectReferencesOf(typeof(WelcomeEnrollmentConsumer)));

    [Fact]
    public void IntegrationContracts_DoNotReferenceAnImplementation() =>
        Assert.Empty(ProjectReferencesOf(typeof(LearnerEnrolledV1)));

    [Theory]
    [InlineData("Domain", "")]
    [InlineData("Contracts", "")]
    [InlineData("Application", "Domain")]
    [InlineData("Infrastructure", "Application,Contracts")]
    [InlineData("Notifications", "Contracts")]
    [InlineData("Api", "Infrastructure,Notifications")]
    [InlineData("ConsoleApp", "Infrastructure,Notifications")]
    public void DeclaredProjectGraph_MatchesTheIntendedDirectDependencies(string project, string allowed)
    {
        // Assembly inspection cannot see an UNUSED ProjectReference that the compiler omits. Inspect
        // source project declarations too. This fitness test intentionally requires a repository checkout.
        DirectoryInfo? root = new(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "Directory.Packages.props")))
            root = root.Parent;
        Assert.NotNull(root);
        string name = $"Learning.Architecture.{project}";
        XDocument document = XDocument.Load(Path.Combine(root.FullName, "10-architecture", "src", name, $"{name}.csproj"));
        string[] dependencies = document.Descendants("ProjectReference")
            .Select(element => Path.GetFileNameWithoutExtension(element.Attribute("Include")!.Value)
                .Replace("Learning.Architecture.", "", StringComparison.Ordinal)).Order().ToArray();
        Assert.Equal(allowed.Split(',', StringSplitOptions.RemoveEmptyEntries).Order().ToArray(), dependencies);
        if (project is "Domain" or "Application" or "Contracts")
        {
            Assert.Empty(document.Descendants("PackageReference"));
            Assert.Empty(document.Descendants("FrameworkReference"));
        }
    }

    private static string[] ProjectReferencesOf(Type type) => ReferencesOf(type)
        .Where(name => name.StartsWith("Learning.Architecture.", StringComparison.Ordinal)).Order().ToArray();

    private static string[] ReferencesOf(Type type) => type.Assembly.GetReferencedAssemblies()
        .Select(reference => reference.Name ?? "").ToArray();
}
