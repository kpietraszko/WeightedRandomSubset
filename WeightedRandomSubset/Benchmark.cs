using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ListPool;
using System.Security.Cryptography;

namespace WeightedRandomSubset;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[MemoryDiagnoser]
public class Benchmark
{
    private WeightedElement[] _allOffers;

    [GlobalSetup]
    public void Setup()
    {
        // generate random offers
        var totalOffersCount = 10_000;
        _allOffers = Enumerable.Range(0, totalOffersCount).Select(i =>
        {
            var id = i;
            var priority = ((Priority)RandomNumberGenerator.GetInt32(5)).ToWeight();
            return new WeightedElement(id, priority);
        }).ToArray();

        var prewarmedPooledLists = Enumerable.Range(0, 5).Select(x => new ListPool<int>(10000));

        foreach (var list in prewarmedPooledLists)
        {
            list.Dispose();
        }
    }

    [Benchmark]
    public void Pick50()
    {
        WeightedRandomSubsetGenerator.PickN(_allOffers, 50);
    }
}
