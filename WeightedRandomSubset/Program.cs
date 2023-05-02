using BenchmarkDotNet.Running;
using Spectre.Console;
using System.Security.Cryptography;
using WeightedRandomSubset;

//var summary = BenchmarkRunner.Run<Benchmark>(); // return;
//Console.WriteLine($"kB allocated total: {summary.Reports[0].GcStats.GetTotalAllocatedBytes(true)}");
//return;


// TODO: POSSIBLY THERE'S A RARE INDEXOUTOFRANGE EXCEPTION, TRACK IT DOWN. Can't reproduce, for some reason

var totalOffersCount = 10_000;
var allElements = Enumerable.Range(0, totalOffersCount).Select(i =>
{
    var id = i;
    var priority = ((Priority)RandomNumberGenerator.GetInt32(5)).ToWeight();
    return new WeightedElement(id, priority);
}).ToArray();


//while (true)
//{
//    WeightedRandomSubsetGenerator.PickN(allElements, 50); // alloc test
//}

var samples = 1_000_000;
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
        var task = ctx.AddTask($"Picking random subsets {samples.ToString("N0")} times");
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

Console.ReadLine();

static void ExecuteManySamples(WeightedElement[] allElements, int samples, Dictionary<float, long> timesPickedPerWeightValue, ProgressTask task)
{
    var weightOfElementLookup = allElements.ToDictionary(e => e.Id, e => e.Weight);
    for (int i = 0; i < samples; i++)
    {
        var pickedElements = WeightedRandomSubsetGenerator.PickN(allElements, 50);

        foreach (var element in pickedElements)
        {
            var elementWeight = weightOfElementLookup[element];
            timesPickedPerWeightValue.TryGetValue(elementWeight, out var count);
            timesPickedPerWeightValue[elementWeight] = count + 1;
        }

        var pickedIds = pickedElements.Select(e => e).ToArray();
        if (pickedIds.ToHashSet().Count != pickedIds.Length)
        {
            throw new Exception("Duplicate found!");
        }

        task.Increment(1d / samples * 100d);
    }
}