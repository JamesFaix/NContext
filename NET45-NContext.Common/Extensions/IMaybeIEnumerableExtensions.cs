﻿namespace NContext.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IMaybeIEnumerableExtensions
    {
        /// <summary>
        /// Returns the first element in an <see cref="IEnumerable{T}" /> as a
        /// <see cref="Just{T}" />, or, if the sequence contains no elements, returns
        /// a <see cref="Nothing{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the object in the <see cref="IEnumerable{T}" /></typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to return the first element of.</param>
        /// <param name="predicate">An optional function to test each element for a condition.</param>
        /// <returns><see cref="IMaybe{T}" /></returns>
        public static IMaybe<T> MaybeFirst<T>(this IEnumerable<T> enumerable, Func<T, Boolean> predicate = null)
        {
            using (var enumerator = GetEnumerator(enumerable, predicate))
            {
                return enumerator.MoveNext()
                ? enumerator.Current.ToMaybe()
                : new Nothing<T>();
            }
        }

        /// <summary>
        /// Returns the single element in an <see cref="IEnumerable{T}" /> as a <see cref="Just{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the object in the <see cref="IEnumerable{T}" /></typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to return the single element of.</param>
        /// <param name="predicate">An optional function to test each element for a condition.</param>
        /// <returns><see cref="IMaybe{T}" /> with the single element in the sequence that passes the test in the (optional) predicate function.</returns>
        public static IMaybe<T> MaybeSingle<T>(this IEnumerable<T> enumerable, Func<T, Boolean> predicate = null)
        {
            using (var enumerator = GetEnumerator(enumerable, predicate))
            {
                if (!enumerator.MoveNext())
                {
                    return new Nothing<T>();
                }

                T current = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    return current.ToMaybe();
                }
            }

            return new Nothing<T>();
        }

        private static IEnumerator<T> GetEnumerator<T>(IEnumerable<T> enumerable, Func<T, Boolean> predicate = null)
        {
            return (predicate == null) ? enumerable.GetEnumerator() : enumerable.Where(predicate).GetEnumerator();
        }
    }
}