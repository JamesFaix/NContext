﻿using System;
using System.Linq;
using System.Net;

namespace NContext.Common
{
    public static class ExceptionExtensions
    {
        public static Error ToError(this Exception exception)
        {
            return new Error(
                (Int32)HttpStatusCode.InternalServerError, 
                exception.GetType().Name, 
                new[] { exception.Message });
        }

        public static Error ToError(this AggregateException aggregateException)
        {
            return new AggregateError(
                (Int32)HttpStatusCode.InternalServerError,
                aggregateException.GetType().Name,
                aggregateException.InnerExceptions.Select(e => e.ToError()));
        }
    }
}
