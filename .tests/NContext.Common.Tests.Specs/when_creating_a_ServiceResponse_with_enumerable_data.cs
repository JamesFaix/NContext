﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="when_creating_a_ServiceResponse_with_enumerable_data.cs" company="Waking Venture, Inc.">
//   Copyright (c) 2013 Waking Venture, Inc.
// 
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//   and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions 
//   of the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//   DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NContext.Common.Tests.Specs
{
    using System;
    using System.Collections.Generic;

    using Machine.Specifications;

    public abstract class when_creating_a_ServiceResponse_with_enumerable_data
    {
        static Lazy<IResponseTransferObject<IEnumerable<Object>>> _ServiceResponse;

        Establish context =
            () =>
            _ServiceResponse =
                new Lazy<IResponseTransferObject<IEnumerable<Object>>>(() => new ServiceResponse<IEnumerable<Object>>(Data));

        protected static void CreateServiceResponse()
        {
            var x = _ServiceResponse.Value;
        }

        public static IResponseTransferObject<IEnumerable<Object>> ServiceResponse
        {
            get { return _ServiceResponse.Value; }
        } 

        public static IEnumerable<Object> Data { get; set; } 
    }
}