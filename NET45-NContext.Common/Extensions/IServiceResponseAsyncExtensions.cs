namespace NContext.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines async extension methods for <see cref="IServiceResponse{T}"/>.
    /// </summary>
    public static class IServiceResponseAsyncExtensions
    {
        /// <summary>
        /// If <seealso cref="IServiceResponse{T}.IsLeft" />, returns a new <see cref="IServiceResponse{T2}" /> instance with the current
        /// <seealso cref="IServiceResponse{T}.Error" />. Else, binds the <seealso cref="IServiceResponse{T}.Data" /> into the 
        /// specified <paramref name="bindFunc" />.
        /// </summary>
        /// <typeparam name="T">The type of the current <see cref="IServiceResponse{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the next <see cref="IServiceResponse{T2}" /> to return.</typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="bindFunc">The binding function.</param>
        /// <returns>Instance of <see cref="IServiceResponse{T2}" />.</returns>
        public static Task<IServiceResponse<T2>> BindAsync<T, T2>(
            this IServiceResponse<T> serviceResponse,
            Func<T, Task<IServiceResponse<T2>>> bindFunc)
        {
            return serviceResponse.IsRight
                ? bindFunc(serviceResponse.Data)
                : serviceResponse.Error.AsErrorResponse<T2>().ToTask();
        }

        public static async Task<IServiceResponse<IEnumerable<T2>>> BindManyAsync<T, T2>(
            this IServiceResponse<IEnumerable<T>> serviceResponse,
            Func<T, Task<IServiceResponse<T2>>> bindFunc)
        {
            if (serviceResponse.IsLeft)
            {
                return serviceResponse.Error.AsErrorResponse<IEnumerable<T2>>();
            }

            var result = new List<T2>();
            foreach (var element in serviceResponse.Data)
            {
                var elementResponse = await bindFunc(element);
                if (elementResponse.IsLeft)
                {
                    return elementResponse.Error.AsErrorResponse<IEnumerable<T2>>();
                }

                result.Add(elementResponse.Data);
            }

            return result.AsServiceResponse();
        }

        /// <summary>
        /// Invokes the specified function if <see cref="IServiceResponse{T}.IsLeft"/>. Returns the current <paramref name="serviceResponse"/> 
        /// unless the <paramref name="catchFunc"/> returns a faulted task. <see cref="Task.IsFaulted"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="catchFunc">Async function to invoke.</param>
        /// <returns>The current <paramref name="serviceResponse"/> unless the <paramref name="catchFunc"/> returns a faulted task.</returns>
        [Obsolete("JF - This method name is misleading, since a catch block will stop error propagation in imperative languages, but this does not stop the error.")]
        public static Task<IServiceResponse<T>> CatchAsync<T>(
            this IServiceResponse<T> serviceResponse,
            Func<Error, Task> catchFunc)
        {
            return serviceResponse.IsRight
                ? serviceResponse.ToTask()
                : catchFunc(serviceResponse.Error).ErrorIfFaulted(serviceResponse);
        }

        /// <summary>
        /// Invokes the specified <paramref name="continueWithFunction"/> function if <see cref="IServiceResponse{T}.IsLeft"/>. 
        /// Allows you to re-direct control flow with a new <typeparamref name="T" /> value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="continueWithFunction">The continue with function.</param>
        /// <returns>If <paramref name="serviceResponse"/>.IsLeft, then the instance of <see cref="IServiceResponse{T}" /> 
        /// returned by <paramref name="continueWithFunction" />, else returns current <paramref name="serviceResponse"/>.</returns>
        [Obsolete("JF - This method more resembles a catch block, but since Catch is already used, the name should get changed to something new if Catch does too.")]
        public static Task<IServiceResponse<T>> CatchAndContinueAsync<T>(
            this IServiceResponse<T> serviceResponse, 
            Func<Error, Task<IServiceResponse<T>>> continueWithFunction)
        {
            return serviceResponse.IsRight
                ? serviceResponse.ToTask()
                : continueWithFunction(serviceResponse.Error);
        }

        /// <summary>
        /// Invokes the specified <paramref name="continueWithFunction"/> function if <see cref="IServiceResponse{T}.IsLeft"/>. 
        /// Allows you to re-direct control flow with a new <typeparamref name="T" /> value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="continueWithFunction">The continue with function.</param>
        /// <returns>If <paramref name="serviceResponse"/>.IsLeft, then the instance of <see cref="IServiceResponse{T}" /> 
        /// returned by <paramref name="continueWithFunction" />, else returns current <paramref name="serviceResponse"/>.</returns>
        [Obsolete("JF - This method isn't really async at all.  It might as well be a method that just returns ISR, and thus be expressed as isr.CatchAndContinue(continuteWithFunction).ToTask().")]
        public static Task<IServiceResponse<T>> CatchAndContinueAsync<T>(
            this IServiceResponse<T> serviceResponse, 
            Func<Error, T> continueWithFunction)
        {
            return (serviceResponse.IsRight
                    ? serviceResponse
                    : continueWithFunction(serviceResponse.Error).AsServiceResponse())
                .ToTask();
        }

        /// <summary>
        /// Invokes the specified action if <see cref="IServiceResponse{T}.IsRight" />.
        /// Returns the current <paramref name="serviceResponse"/> instance unless <paramref name="letFunc"/> 
        /// returns a faulted task. <see cref="Task.IsFaulted"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="letFunc">The let function.</param>
        /// <returns>Returns the current <paramref name="serviceResponse"/> instance unless 
        /// <paramref name="letFunc"/> returns a faulted task. <see cref="Task.IsFaulted"/></returns>
        public static Task<IServiceResponse<T>> LetAsync<T>(
            this IServiceResponse<T> serviceResponse,
            Func<T, Task> letFunc)
        {
            return serviceResponse.IsRight
                ? letFunc(serviceResponse.Data).ErrorIfFaulted(serviceResponse)
                : serviceResponse.ToTask();
        }

        /// <summary>
        /// Invokes the specified action if <paramref name="serviceResponse"/>.<see cref="IServiceResponse{T}.IsRight" />.
        /// Returns the current <paramref name="serviceResponse"/> instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="letAction">The let function.</param>
        /// <returns>Returns the current <paramref name="serviceResponse"/> instance unless 
        /// <paramref name="letAction"/> returns a faulted task. <see cref="Task.IsFaulted"/></returns>
        [Obsolete("JF - This can be expressed more clearly as isr.Let(action).ToTask(). Labelling the method as Async is misleading.")]
        public static Task<IServiceResponse<T>> LetAsync<T>(
            this IServiceResponse<T> serviceResponse,
            Action<T> letAction)
        {
            if (serviceResponse.IsRight)
            {
                // TODO: (DG) This can yield unexpected results of the action is an asyncVoid and throws an exception!
                letAction(serviceResponse.Data);
            }
            return serviceResponse.ToTask();
        }        
    }
}