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

        /// <summary>
        /// Returns the instance as a <see cref="IMaybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the object to wrap</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns><see cref="IMaybe{T}"/></returns>
        public static IMaybe<T> ToMaybe<T>(this T instance)
        {
            return instance == null
                ? new Nothing<T>() as IMaybe<T>
                : new Just<T>(instance);
        }
    }
}
