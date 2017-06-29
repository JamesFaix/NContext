namespace NContext.Common
{
    using System;
    using System.Threading.Tasks;

    public static class IEitherExtensions
    {
        public static T3 Fold<T, T2, T3>(this IEither<T, T2> either, Func<T, T3> leftFunc, Func<T2, T3> rightFunc)
        {
            return either.IsLeft 
                ? leftFunc(either.GetLeft())
                : rightFunc(either.GetRight());
        }

        public static Task<T3> FoldAsync<T, T2, T3>(this IEither<T, T2> either, Func<T, Task<T3>> leftFunc, Func<T2, Task<T3>> rightFunc)
        {
            return either.IsLeft
                ? leftFunc(either.GetLeft())
                : rightFunc(either.GetRight());
        }

        public static IEither<T, T2> JoinLeft<T, T2>(this IEither<IEither<T, T2>, T2> either)
        {
            return either.IsLeft
                ? either.GetLeft()
                : new Right<T, T2>(either.GetRight());
        }

        public static IEither<T, T2> JoinRight<T, T2>(this IEither<T, IEither<T, T2>> either)
        {
            return either.IsLeft
                ? new Left<T, T2>(either.GetLeft())
                : either.GetRight();
        }
    }
}