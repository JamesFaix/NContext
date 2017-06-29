using System.Threading.Tasks;

namespace NContext.Common
{
    public static class ObjectExtensions
    {
        public static IServiceResponse<T> AsServiceResponse<T>(this T data)
        {
            return new DataResponse<T>(data);
        }

        public static Task<T> ToTask<T>(this T result)
        {
            return Task.FromResult(result);
        }
    }
}
