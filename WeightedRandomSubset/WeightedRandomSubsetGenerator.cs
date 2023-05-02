using ListPool;
using static WeightedRandomSubset.RandomHelpers;

namespace WeightedRandomSubset;

public static class WeightedRandomSubsetGenerator
{
    /// <returns>List of result Ids</returns>
    public static IReadOnlyList<int> PickN(IReadOnlyList<WeightedElement> allElements, int numberOfOffersToPick)
    {
        var pickedElements = new List<int>(numberOfOffersToPick); // allocates ~210 B

        var elementsByWeight = GroupByWeight(allElements, out var occuringWeights);
        var weightsSum = elementsByWeight.Sum(e => e.Value.Count * e.Key); // allocates ~155 B

        for (int i = 0; i < numberOfOffersToPick; i++)
        {           
            var randomPoint = Random01() * weightsSum;

            double currentRangeStart = 0;

            foreach (var weight in occuringWeights)//foreach (var kvp in elementsByWeight) // iterates 5 times (number of possible weights)
            {
                //var weight = kvp.Key;

                var elementsWithThisWeight = elementsByWeight[weight];
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

        foreach (var weight in occuringWeights)
        {
            elementsByWeight[weight].Dispose();
        }

        return pickedElements;
    }

    private static SortedDictionary<float, ListPool<int>> GroupByWeight(IReadOnlyList<WeightedElement> elements, out float[] occuringWeights)
    {
        var dict = new SortedDictionary<float, ListPool<int>>(); // WARN: when iterated allocates over 5 times more than with regular dictionary
        var occuringWeightsList = new List<float>(5);

        foreach (var element in elements)
        {
            if (!dict.TryGetValue(element.Weight, out var list))
            {
                list = new ListPool<int>(); // providing capacity doesn't change run time or total allocs
                dict[element.Weight] = list;
                occuringWeightsList.Add(element.Weight);
            }

            list.Add(element.Id);
        }

        occuringWeights = occuringWeightsList.ToArray();
        Array.Sort(occuringWeights);

        return dict;
    }


}
