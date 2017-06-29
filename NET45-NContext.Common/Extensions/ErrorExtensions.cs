namespace NContext.Common
{
    using System.Collections.Generic;

    public static class ErrorExtensions
    {
        public static IServiceResponse<T> AsErrorResponse<T>(this Error error)
        {
            return new ErrorResponse<T>(error);
        }

        public static AggregateError Aggregate(this IList<Error> errors)
        {
            return new AggregateError(
                errors[0].HttpStatusCode, 
                errors[0].Code, 
                errors);
        }
    }
}
