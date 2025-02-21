using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Benchmarks.Net9;

var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core90));

BenchmarkRunner.Run<StackAlloc>(config);
