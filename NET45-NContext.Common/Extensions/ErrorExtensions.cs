namespace NContext.Common
{
    public static class ErrorExtensions
    {
        public static IServiceResponse<T> AsErrorResponse<T>(this Error error)
        {
            return new ErrorResponse<T>(error);
        }
    }
}
