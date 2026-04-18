using CSharpBasics.Examples.Collections;
using CSharpBasics.Examples.ControlFlow;
using CSharpBasics.Examples.Memory;
using CSharpBasics.Examples.Methods;
using CSharpBasics.Examples.Nullability;
using CSharpBasics.Examples.Strings;
using CSharpBasics.Examples.Variables;

namespace CSharpBasics.Tests;

public class ExamplesRunSmokeTests
{
    [Fact]
    public void AllExampleRunMethods_ExecuteWithoutThrowing()
    {
        VariablesExamples.Run();
        DynamicVsTypedExample.Run();

        IfElseExample.Run();
        SwitchExample.Run();
        LoopsExample.Run();

        MethodBasicsExample.Run();
        ParamModifiersExample.Run();
        OverloadingExample.Run();
        OptionalParametersExample.Run();

        ArraysExample.Run();
        ListExample.Run();
        DictionaryExample.Run();
        HashSetExample.Run();
        EnumerableExample.Run();

        StringBasicsExample.Run();
        StringBuilderExample.Run();
        StringMethodsExample.Run();
        StringPerformanceExample.Run();

        NullabilityExample.Run();
        MemoryConceptsExample.Run();
    }
}
