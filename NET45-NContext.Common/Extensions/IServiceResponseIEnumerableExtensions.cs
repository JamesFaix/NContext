namespace NContext.Common
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// Defines extension methods for <see cref="IEnumerable{T}"/> yielding a new <see cref="IServiceResponse{T}"/>.
    /// </summary>
    public static class IServiceResponseIEnumerableExtensions
    {
        /// <summary>
        /// Returns an <see cref="IServiceResponse{T}"/> with the first element of a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to return the first element of.</param>
        /// <param name="predicate">An optional function to test each element for a condition.</param>
        /// <returns>IServiceResponse{T} with the first element in the sequence that passes the test in the (optional) predicate function.</returns>
        public static IServiceResponse<T> FirstResponse<T>(this IEnumerable<T> enumerable, Func<T, Boolean> predicate = null)
        {
            using (var e = enumerable.GetEnumerator(predicate))
            {
                return e.MoveNext()
                    ? e.Current.AsServiceResponse()
                    : new Error(
                            (Int32)HttpStatusCode.InternalServerError,
                            "IServiceResponseIEnumerableExtensions_FirstResponse_NoMatch", 
                            new[] { "Enumerable is empty." })
                        .AsErrorResponse<T>();
            }
        }

        /// <summary>
        /// Returns an <see cref="IServiceResponse{T}"/> with the single, specific element of a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to return the single element of.</param>
        /// <param name="predicate">An optional function to test each element for a condition.</param>
        /// <returns>IServiceResponse{T} with the single element in the sequence that passes the test in the (optional) predicate function.</returns>
        public static IServiceResponse<T> SingleResponse<T>(this IEnumerable<T> enumerable, Func<T, Boolean> predicate = null)
        {
            using (var e = enumerable.GetEnumerator(predicate))
            {
                if (!e.MoveNext())
                {
                    return new Error(
                            (Int32)HttpStatusCode.InternalServerError,
                            "IServiceResponseIEnumerableExtensions_SingleResponse_NoMatch",
                            new[] { "Enumerable is empty." })
                        .AsErrorResponse<T>();
                }

                T current = e.Current;
                if (!e.MoveNext())
                {
                    return current.AsServiceResponse();
                }
            }

            return new Error(
                    (Int32)HttpStatusCode.InternalServerError,
                    "IServiceResponseIEnumerableExtensions_SingleResponse_MoreThanOneMatch",
                    new[] {"Enumerable has more than one matched entry."})
                .AsErrorResponse<T>();
        }
    }
}