using BenchmarkDotNet.Running;
using Spectre.Console;
using System.Security.Cryptography;
using WeightedRandomSubset;

//BenchmarkRunner.Run<Benchmark>();

var totalOffersCount = 10_000;
var allElements = Enumerable.Range(0, totalOffersCount).Select(i =>
{
    var id = i;
    var priority = ((Priority)RandomNumberGenerator.GetInt32(5)).ToWeight();
    return new WeightedElement(id, priority);
}).ToArray();


var samples = 1_000_000; // TODO: test this on 10 million samples
var elementsCountPerWeightValue = allElements.GroupBy(e => e.Weight)
    .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());

var timesPickedPerWeightValue = new Dictionary<float, long>();

#region fancy-rendering
AnsiConsole.Progress()
    .Columns(new ProgressColumn[]
    {
        new TaskDescriptionColumn(),
        new ProgressBarColumn(),
        new PercentageColumn(),
        new RemainingTimeColumn()
    } )
#endregion
    .Start(ctx =>
    {
        var task = ctx.AddTask("Picking random subsets many times");
        ExecuteManySamples(allElements, samples, timesPickedPerWeightValue, task);
    });


var timesPickedPerWeightAveragePerElement = new SortedDictionary<float, double>();

foreach (var kvp in timesPickedPerWeightValue)
{
    var weight = kvp.Key;
    var elementsWithThisWeight = elementsCountPerWeightValue[weight];
    timesPickedPerWeightAveragePerElement.Add(weight, (double)kvp.Value / elementsWithThisWeight);
}

#region fancy-rendering
AnsiConsole.Write(new BarChart()
    .Width(100)
    .Label("[bold underline]Times offer with given priority was picked, normalized over count[/]")
    .CenterLabel()
    .AddItems(timesPickedPerWeightAveragePerElement
                .Select(kvp => new BarChartItem(kvp.Key.ToString("N1"), Math.Round(kvp.Value,2)))));
#endregion

static void ExecuteManySamples(WeightedElement[] allElements, int samples, Dictionary<float, long> timesPickedPerWeightValue, ProgressTask task)
{
    for (int i = 0; i < samples; i++)
    {
        var pickedElements = WeightedRandomSubsetGenerator.PickN(allElements, 50);

        foreach (var element in pickedElements)
        {
            timesPickedPerWeightValue.TryGetValue(element.Weight, out var count);
            timesPickedPerWeightValue[element.Weight] = count + 1;
        }

        var pickedIds = pickedElements.Select(e => e.Id).ToArray();
        if (pickedIds.ToHashSet().Count != pickedIds.Count())
        {
            throw new Exception("Duplicate found!");
        }

        task.Increment(1d / samples * 100d);
    }
}