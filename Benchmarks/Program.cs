using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Benchmarks.Tasks;

var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80).AsBaseline())
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core90))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0));

BenchmarkRunner.Run<TaskWhenAll>(config);
