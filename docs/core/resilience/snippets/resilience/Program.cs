﻿using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Retry;

var services = new ServiceCollection();

const string key = "Retry-CircuitBreaker-Timeout";

services.AddResiliencePipeline(key, builder =>
{
    // See: https://www.pollydocs.org/strategies/retry.html
    builder.AddRetry(new RetryStrategyOptions());

    // See: https://www.pollydocs.org/strategies/circuit-breaker.html
    builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions());

    // See: https://www.pollydocs.org/strategies/timeout.html
    builder.AddTimeout(TimeSpan.FromSeconds(1.5));
});

services.AddResilienceEnrichment();
services.AddExceptionSummarizer(); // TODO: remove once bug is fixed.

using ServiceProvider provider = services.BuildServiceProvider();

ResiliencePipelineProvider<string> pipelineProvider =
    provider.GetRequiredService<ResiliencePipelineProvider<string>>();

ResiliencePipeline pipeline = pipelineProvider.GetPipeline(key);

await pipeline.ExecuteAsync(static async cancellationToken =>
{
    // Code that could potentially fail.
    Console.WriteLine("TODO: code that could fail goes here.");

    await ValueTask.CompletedTask;
});

Console.WriteLine("Done...");
