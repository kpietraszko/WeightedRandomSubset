using System.Runtime.CompilerServices;

namespace WeightedRandomSubset;

public enum Priority
{
    P1,
    P2, 
    P3, 
    P4, 
    P5
}

public static class PriorityExtensions 
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToWeight(this Priority priority)
    {
        return priority switch
        {
            Priority.P1 => 1.2f,
            Priority.P2 => 1.1f,
            Priority.P3 => 1.0f,
            Priority.P4 => 0.9f,
            Priority.P5 => 0.8f,
            _ => throw new NotImplementedException(),
        };
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static float ToWeight(this int priority)
    //{
    //    return ((Priority)priority).ToWeight();
    //}
}
