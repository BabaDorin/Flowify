using Flowify.Contracts;

namespace Flowify.Tests.TestFixtures.Queries;

public record TestQuery(int Id) : IRequest<string>;
