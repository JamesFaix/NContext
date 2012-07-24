﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceOptions.cs">
//   Copyright (c) 2012 Waking Venture, Inc.
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
//
// <summary>
//   Defines persistence-related options for transactional operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NContext.Data.Persistence
{
    using System;

    /// <summary>
    /// Defines persistence-related options for transactional operations.
    /// </summary>
    public class PersistenceOptions
    {
        private TimeSpan? _TransactionTimeOut;

        private Int32? _MaxDegreeOfParallelism;

        public Int32 MaxDegreeOfParallelism
        {
            get
            {
                return _MaxDegreeOfParallelism ?? 1;
            }
            set
            {
                _MaxDegreeOfParallelism = value;
            }
        }

        public TimeSpan TransactionTimeOut
        {
            get
            {
                return _TransactionTimeOut ?? System.Transactions.TransactionManager.DefaultTimeout;
            }
            set
            {
                _TransactionTimeOut = value;
            }
        }

        public static PersistenceOptions Default
        {
            get
            {
                return new PersistenceOptions();
            }
        }
    }
}