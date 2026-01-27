using Flowify.Contracts;

namespace Flowify.Tests.TestFixtures.Commands;

public record TestCommandWithoutResponse(string Message) : IRequest;
