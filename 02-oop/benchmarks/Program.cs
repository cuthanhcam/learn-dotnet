using BenchmarkDotNet.Running;
using OopBasics.Benchmarks;

Console.WriteLine("Running OOP Benchmarks...");
BenchmarkSwitcher.FromAssembly(typeof(InheritanceBenchmarks).Assembly).Run(args);
