namespace NContext.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Defines a generic data-transfer-object for containing arbitrary data.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    public class DataResponse<T> : ServiceResponse<T>
    {
        private readonly T _Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResponse{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public DataResponse(T data)
        {
            _Data = MaterializeDataIfNeeded(data);
        }

        /// <summary>
        /// Gets the is left.
        /// </summary>
        /// <value>The is left.</value>
        public override Boolean IsLeft { get { return false; } }

        /// <summary>
        /// Gets the left value. (Returns null)
        /// </summary>
        /// <returns>Error.</returns>
        public override Error GetLeft() { return null; }

        /// <summary>
        /// Gets the right value, <see cref="Data"/>.
        /// </summary>
        /// <returns>T.</returns>
        public override T GetRight() { return _Data; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public override T Data { get { return _Data; } }

        private static T MaterializeDataIfNeeded(T data)
        {
            if (typeof(T).GetTypeInfo().IsValueType || data == null)
            {
                return data;
            }

            var runtimeType = data.GetType();
            return IsPossiblyALazySequence(data, runtimeType)
                ? Materialize(runtimeType, data)
                : data;
        }

        private static Boolean IsPossiblyALazySequence(T data, Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return data is IEnumerable &&
                typeInfo.IsGenericType &&
                !Implements(type, typeof(IDictionary<,>)) &&
                (Implements(type, typeof(IQueryable<>)) || typeInfo.IsNestedPrivate);
        }

        private static Boolean Implements(Type type, Type interfaceType)
        {
            if (type == null) return false;
            
            return Enumerable.Repeat(type, 1)
                .Concat(type.GetTypeInfo().ImplementedInterfaces)
                .Any(t => t.GetTypeInfo().IsGenericType
                    && t.GetGenericTypeDefinition() == interfaceType);
        }

        private static U Materialize<U>(Type type , U data)
        {
            // Get the last generic argument.
            // .NET has several internal iterable types in LINQ that have multiple generic
            // arguments.  The last is reserved for the actual type used for projection.
            // ex. WhereSelectArrayIterator, WhereSelectEnumerableIterator, WhereSelectListIterator
            var elementType = type.GenericTypeArguments.Last();
            var collectionType = type.GetGenericTypeDefinition() == typeof(Collection<>)
                ? typeof(Collection<>)
                : typeof(List<>);

            var constructedType = collectionType.MakeGenericType(elementType);
            return (U)constructedType.CreateInstance(data);
        }
    }
}