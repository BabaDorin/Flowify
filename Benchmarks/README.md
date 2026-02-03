# Flowify vs MediatR Benchmarks

This project compares the performance of Flowify against MediatR, the industry-standard mediator library for .NET.

## Running Benchmarks

To run the benchmarks in Release mode (required for accurate results):

```bash
cd Benchmarks
dotnet run -c Release
```

## What's Being Tested

The benchmarks compare three scenarios:

1. **Command (no response)**: Dispatching a request that returns `Task` (void)
2. **Query (with response)**: Dispatching a request that returns `Task<T>`
3. **Notification**: Publishing a notification to multiple handlers

## Understanding Results

- **Mean**: Average execution time
- **Allocated**: Memory allocated per operation
- **Rank**: Performance ranking (1 = fastest)

## Architecture

Both libraries now use a similar wrapper pattern for optimal performance:

- **First request**: Reflection is used to create a cached wrapper instance
- **Subsequent requests**: Direct virtual method dispatch (no reflection)

This approach eliminates the per-request reflection overhead that would otherwise make these libraries unusable at scale.
