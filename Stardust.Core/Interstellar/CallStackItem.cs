﻿//
// callstackitem.cs
// This file is part of Stardust
//
// Author: Jonas Syrstad (jsyrstad2+StardustCore@gmail.com), http://no.linkedin.com/in/jonassyrstad/) 
// Copyright (c) 2014 Jonas Syrstad. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stardust.Interstellar
{
    [DataContract()]
    public class CallStackItem
    {
        public override string ToString()
        {
            return string.Join(".", Name, MethodName);
        }

        private DateTime? TaskEndTime;

        [DataMember]
        public string AdditionalInformation { get; set; }

        [DataMember]
        public List<CallStackItem> CallStack { get; set; }

        [DataMember]
        public DateTime? EndTimeStamp
        {
            get
            {
                return TaskEndTime;
            }
            set
            {
                if (value.HasValue)
                {
                    ExecutionTime = (value.Value - TimeStamp).TotalMilliseconds;
                }
                else
                    ExecutionTime = null;
                TaskEndTime = value;
            }
        }

        [DataMember]
        public string ErrorPath { get; set; }

        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public double? ExecutionTime { get; set; }

        [DataMember]
        public string MethodName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }
    }
}