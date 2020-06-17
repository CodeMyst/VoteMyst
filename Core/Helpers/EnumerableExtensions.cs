using System;
using System.Linq;
using System.Collections.Generic;

namespace VoteMyst
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> sequence)
        {
            Random rnd = new Random();
            return sequence.OrderBy(e => rnd.NextDouble());
        }
    }
}