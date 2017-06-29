using System.Threading.Tasks;
using System.Threading;

namespace NContext.Common
{
    public static class TaskExtensions
    {
        public static Task<IServiceResponse<T>> ErrorIfFaulted<T>(this Task task, IServiceResponse<T> defaultValue)
        {
            return task.ContinueWith(t =>
                t.IsFaulted
                    ? task.Exception.ToError().AsErrorResponse<T>()
                    : defaultValue,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}
