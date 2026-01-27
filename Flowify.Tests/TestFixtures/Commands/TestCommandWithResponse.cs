using Flowify.Contracts;

namespace Flowify.Tests.TestFixtures.Commands;

public record TestCommandWithResponse(string Value) : IRequest<int>;
