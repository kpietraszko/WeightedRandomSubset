using ListPool;
using static WeightedRandomSubset.RandomHelpers;

namespace WeightedRandomSubset;

public static class WeightedRandomSubsetGenerator
{
    /// <returns>List of result Ids</returns>
    public static IReadOnlyList<int> PickN(WeightedElement[] allElements, int numberOfOffersToPick)
    {
        var pickedElements = new List<int>(numberOfOffersToPick); // allocates ~210 B

        var elementsByWeight = GroupByWeight(allElements, out var occuringWeights, out var sumOfWeights);

        for (int i = 0; i < numberOfOffersToPick; i++)
        {           
            var randomPoint = Random01() * sumOfWeights;

            double currentRangeStart = 0;

            for (int weightIndex = 0; weightIndex < occuringWeights.Count; weightIndex++) // iterates 5 times (number of possible weights)
            {
                float weight = occuringWeights[weightIndex];
                //var weight = kvp.Key;

                var elementsWithThisWeight = elementsByWeight[weightIndex];
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
                    sumOfWeights -= weight;
                    break;
                }

                currentRangeStart = rangeEnd;
            }
        }

        for (int i = 0; i < occuringWeights.Count; i++)
        {
            elementsByWeight[i].Dispose(); // returns the list to the pool
        }

        return pickedElements;
    }

    private static List<ListPool<int>> GroupByWeight(
        WeightedElement[] elements, 
        out List<float> occuringWeights,
        out double sumOfWeights)
    {
        const int expectedNumberOfWeights = 5;
        var elementsPerWeight = new List<ListPool<int>>(expectedNumberOfWeights);
        occuringWeights = new List<float>(expectedNumberOfWeights);

        foreach (var element in elements) // find all weights occuring in the set
        {
            var existingWeightIndex = occuringWeights.IndexOf(element.Weight);

            if (existingWeightIndex == -1)
            {
                occuringWeights.Add(element.Weight);
                elementsPerWeight.Add(new ListPool<int>());
            }
        }

        occuringWeights.Sort();

        sumOfWeights = 0;

        foreach (var element in elements) // assign elements to appropriate list per weight
        {
            var weightIndex = occuringWeights.IndexOf(element.Weight);
            elementsPerWeight[weightIndex].Add(element.Id);
            sumOfWeights += element.Weight;
        }

        return elementsPerWeight;
    }


}
