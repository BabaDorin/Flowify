using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Flowify.Commands;
using Benchmarks.Flowify.Notifications;
using Benchmarks.Flowify.Queries;
using Benchmarks.MediatR.Commands;
using Benchmarks.MediatR.Notifications;
using Benchmarks.MediatR.Queries;
using Flowify.Extensions;
using Microsoft.Extensions.DependencyInjection;
using FlowifyMediator = Flowify.Contracts.IMediator;
using MediatRMediator = MediatR.IMediator;

namespace Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class MediatorBenchmarks
{
    private FlowifyMediator _flowifyMediator = null!;
    private MediatRMediator _mediatRMediator = null!;

    private FlowifyPingCommand _flowifyCommand = null!;
    private FlowifyPingQuery _flowifyQuery = null!;
    private FlowifyPingNotification _flowifyNotification = null!;

    private MediatRPingCommand _mediatRCommand = null!;
    private MediatRPingQuery _mediatRQuery = null!;
    private MediatRPingNotification _mediatRNotification = null!;

    private FlowifyComplexQuery _flowifyComplexQuery = null!;
    private MediatRComplexQuery _mediatRComplexQuery = null!;

    [GlobalSetup]
    public void Setup()
    {
        var flowifyServices = new ServiceCollection();
        flowifyServices.AddFlowify(typeof(FlowifyPingCommandHandler).Assembly);
        var flowifyProvider = flowifyServices.BuildServiceProvider();
        _flowifyMediator = flowifyProvider.GetRequiredService<FlowifyMediator>();

        var mediatRServices = new ServiceCollection();
        mediatRServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatRPingCommandHandler).Assembly));
        var mediatRProvider = mediatRServices.BuildServiceProvider();
        _mediatRMediator = mediatRProvider.GetRequiredService<MediatRMediator>();

        _flowifyCommand = new FlowifyPingCommand("Hello");
        _flowifyQuery = new FlowifyPingQuery("Hello");
        _flowifyNotification = new FlowifyPingNotification("Hello");
        _flowifyComplexQuery = new FlowifyComplexQuery(123, "Active", DateTime.Now, DateTime.Now.AddDays(30));

        _mediatRCommand = new MediatRPingCommand("Hello");
        _mediatRQuery = new MediatRPingQuery("Hello");
        _mediatRNotification = new MediatRPingNotification("Hello");
        _mediatRComplexQuery = new MediatRComplexQuery(123, "Active", DateTime.Now, DateTime.Now.AddDays(30));
    }

    [Benchmark(Description = "Flowify: Command (no response)")]
    public async Task Flowify_SendCommand()
    {
        await _flowifyMediator.Send(_flowifyCommand);
    }

    [Benchmark(Description = "MediatR: Command (no response)")]
    public async Task MediatR_SendCommand()
    {
        await _mediatRMediator.Send(_mediatRCommand);
    }

    [Benchmark(Description = "Flowify: Query (with response)")]
    public async Task<string> Flowify_SendQuery()
    {
        return await _flowifyMediator.Send(_flowifyQuery);
    }

    [Benchmark(Description = "MediatR: Query (with response)")]
    public async Task<string> MediatR_SendQuery()
    {
        return await _mediatRMediator.Send(_mediatRQuery);
    }

    [Benchmark(Description = "Flowify: Notification (2 handlers)")]
    public async Task Flowify_PublishNotification()
    {
        await _flowifyMediator.Publish(_flowifyNotification);
    }

    [Benchmark(Description = "MediatR: Notification (2 handlers)")]
    public async Task MediatR_PublishNotification()
    {
        await _mediatRMediator.Publish(_mediatRNotification);
    }

    [Benchmark(Description = "Flowify: Complex query (multiple params)")]
    public async Task<ComplexResult> Flowify_SendComplexQuery()
    {
        return await _flowifyMediator.Send(_flowifyComplexQuery);
    }

    [Benchmark(Description = "MediatR: Complex query (multiple params)")]
    public async Task<MediatRComplexResult> MediatR_SendComplexQuery()
    {
        return await _mediatRMediator.Send(_mediatRComplexQuery);
    }
}

