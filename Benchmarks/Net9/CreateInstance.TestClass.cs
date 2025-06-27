using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.Net9;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Child Class")]
public class TestClass
{
    public TestClass()
    {
    }

    public TestClass(int a)
    {
    }

    public TestClass(int a, int b)
    {
    }

    [ActivatorUtilitiesConstructor]
    public TestClass(int a, int b, int c)
    {
    }
}
