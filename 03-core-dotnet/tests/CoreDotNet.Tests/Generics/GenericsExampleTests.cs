using CoreDotNet.Examples.Generics;

namespace CoreDotNet.Tests.Generics;

[Collection("Console")]
public class GenericsExampleTests
{
    [Fact]
    public void Box_Holds_Any_Generic_Value()
    {
        var box = new Box<string> { Value = "Hello" };

        Assert.Equal("Hello", box.Value);
    }

    [Fact]
    public void GenericStack_Pushes_Peeks_And_Pops()
    {
        var stack = new GenericStack<int>();

        stack.Push(10);
        stack.Push(20);

        Assert.Equal(20, stack.Peek());
        Assert.Equal(20, stack.Pop());
        Assert.Equal(10, stack.Pop());
    }

    [Fact]
    public void Repository_Can_Add_And_Retrieve_Entities()
    {
        var repository = new Repository<User>();
        var user = new User { Id = 1, Name = "Alice" };

        repository.Add(user);

        Assert.Same(user, repository.GetById(1));
        Assert.True(repository.TryGetById(1, out var found));
        Assert.Same(user, found);
    }

    [Fact]
    public void Run_Prints_Generic_Examples()
    {
        string output = ConsoleCapture.Run(GenericsExample.Run);

        Assert.Contains("Generics Examples", output);
        Assert.Contains("Stack peek:", output);
        Assert.Contains("Repository exposes read-only list count:", output);
    }
}
