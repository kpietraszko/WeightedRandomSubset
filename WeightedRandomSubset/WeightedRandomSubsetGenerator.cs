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
        var weightsSum = elementsByWeight.Gen().Sum(e => e.Value.Count * e.Key);

        for (int i = 0; i < numberOfOffersToPick; i++)
        {           
            var randomPoint = Random01() * weightsSum;

            double currentRangeStart = 0;

            foreach (var kvp in elementsByWeight) // iterates 5 times (number of possible weights)
            {
                var weight = kvp.Key;
                var elementsWithThisWeight = kvp.Value;
                if (elementsWithThisWeight == null || elementsWithThisWeight.Count == 0) // probably redundant
                {
                    continue;
                }

                var rangeEnd = currentRangeStart + elementsWithThisWeight.Count * weight;

                if (randomPoint < rangeEnd)
                {
                    var randomPointOnThisWeightLine = randomPoint - currentRangeStart;
                    var index = (int)(randomPointOnThisWeightLine / weight);
                    index = Math.Clamp(index, 0, elementsWithThisWeight.Count - 1); // helps with rounding edge-cases

                    pickedElements.Add(elementsWithThisWeight[index]);
                    elementsWithThisWeight.RemoveAt(index);
                    weightsSum -= weight;
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
                if (list.Count > 0) throw new Exception("List not empty!");
                dict[element.Weight] = list;
            }

            list.Add(element);
        }

        return dict;
    }


}
