using System;
using System.Linq;

namespace NContext.Common
{
    public static class ExceptionExtensions
    {
        public static Error ToError(this Exception exception)
        {
            return new Error(
                500, 
                exception.GetType().Name, 
                new[] { exception.Message });
        }

        public static Error ToError(this AggregateException aggregateException)
        {
            return new AggregateError(
                500,
                aggregateException.GetType().Name,
                aggregateException.InnerExceptions.Select(e => e.ToError()));
        }
    }
}
