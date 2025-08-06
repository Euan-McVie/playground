using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Benchmarks.Linq;

var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80).AsBaseline())
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core90));

BenchmarkRunner.Run<LinqOrderByBenchmarks>(config);
