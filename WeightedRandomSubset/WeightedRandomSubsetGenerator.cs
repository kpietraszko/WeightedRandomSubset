using Cathei.LinqGen;
using ListPool;
using static WeightedRandomSubset.RandomHelpers;

namespace WeightedRandomSubset;

public static class WeightedRandomSubsetGenerator
{
    public static IReadOnlyList<WeightedElement> PickN(IReadOnlyList<WeightedElement> allElements, int numberOfOffersToPick)
    {
        var pickedElements = new List<WeightedElement>(numberOfOffersToPick);

        var elementsByWeight = GroupByPriority(allElements);

        for (int i = 0; i < numberOfOffersToPick; i++)
        {
            var prioritiesSum = elementsByWeight.Gen().Sum(e => e.Value.Count * e.Key);

            var randomPoint = Random01() * prioritiesSum;

            double currentRangeStart = 0;

            foreach (var kvp in elementsByWeight) // iterates 5 times (number of possible priorities)
            {
                var priority = kvp.Key;
                var elementsWithThisWeight = kvp.Value;
                if (elementsWithThisWeight == null || elementsWithThisWeight.Count == 0) // probably redundant
                {
                    continue;
                }

                var rangeEnd = currentRangeStart + elementsWithThisWeight.Count * priority;

                if (randomPoint < rangeEnd)
                {
                    var randomPointOnThisWeightLine = randomPoint - currentRangeStart;
                    var index = (int)(randomPointOnThisWeightLine / priority);
                    pickedElements.Add(elementsWithThisWeight[index]);
                    elementsWithThisWeight.RemoveAt(index);
                    break;
                }

                currentRangeStart = rangeEnd;
            }
        }

        foreach (var kvp in elementsByWeight)
        {
            kvp.Value.Dispose();
        }

        return pickedElements;
    }

    private static SortedDictionary<float, ListPool<WeightedElement>> GroupByPriority(IReadOnlyList<WeightedElement> elements)
    {
        var dict = new SortedDictionary<float, ListPool<WeightedElement>>();

        foreach (var element in elements)
        {
            if (!dict.TryGetValue(element.Weight, out var list))
            {
                list = new ListPool<WeightedElement>(); // providing capacity doesn't change run time or total allocs
                dict[element.Weight] = list;
            }

            list.Add(element);
        }

        return dict;
    }


}
