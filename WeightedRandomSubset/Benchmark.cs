using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System.Security.Cryptography;

namespace WeightedRandomSubset;

[SimpleJob]
[MemoryDiagnoser]
public class Benchmark
{
    private IReadOnlyList<WeightedElement> _allOffers;
    private const int NumberOfOffersToPick = 50;

    [GlobalSetup]
    public void Setup()
    {
        // generate random offers
        var totalOffersCount = 10_000;
        _allOffers = Enumerable.Range(0, totalOffersCount).Select(i =>
        {
            var id = RandomNumberGenerator.GetInt32(99999);
            var priority = ((Priority)RandomNumberGenerator.GetInt32(5)).ToWeight();
            return new WeightedElement(id, priority);
        }).ToArray();

    }

    [Benchmark]
    public void Pick50()
    {
        var result = WeightedRandomSubsetGenerator.PickN(_allOffers, 50);
    }
}
