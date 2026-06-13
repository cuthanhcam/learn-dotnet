using BenchmarkDotNet.Running;

BenchmarkRunner.Run<AllocationBenchmarks>();
BenchmarkRunner.Run<SpanAndPoolingBenchmarks>();
