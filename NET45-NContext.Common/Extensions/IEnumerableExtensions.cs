using System;
using System.Collections.Generic;
using System.Linq;

namespace NContext.Common
{
    internal static class IEnumerableExtensions
    {
        public static IEnumerator<T> GetEnumerator<T>(this IEnumerable<T> enumerable, Func<T, Boolean> predicate = null)
        {
            return (predicate == null
                    ? enumerable
                    : enumerable.Where(predicate))
                .GetEnumerator();
        }
    }
}
