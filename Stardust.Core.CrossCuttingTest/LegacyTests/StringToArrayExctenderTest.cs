﻿//
// StringToArrayExctenderTest.cs
// This file is part of Stardust.Core.CrossCuttingTest
//
// Author: Jonas Syrstad (jsyrstad2+StardustCore@gmail.com), http://no.linkedin.com/in/jonassyrstad/) 
// Copyright (c) 2013 Jonas Syrstad. All rights reserved.
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stardust.Particles;

namespace Stardust.Core.CrossCuttingTest.LegacyTests
{
    
    
    /// <summary>
    ///This is a test class for StringToArrayExctenderTest and is intended
    ///to contain all StringToArrayExctenderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringToArrayExctenderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for GetStringAsByteArray
        ///</summary>
        [TestMethod()]
        [TestCategory("String extensions")]
        public void GetStringAsByteArrayTest()
        {
            string self = "test";
            var expected = 4;
            var array = self.GetByteArray(EncodingType.Utf8);
            Assert.AreEqual(expected, array.Length);
        }

        /// <summary>
        ///A test for GetStringAsUnicodeArray
        ///</summary>
        [TestMethod()]
        [TestCategory("String extensions")]
        public void GetStringAsUnicodeArrayTest()
        {
            string self = "test";
            var expected = 8;
            var array = self.GetByteArray(EncodingType.Unicode);
            Assert.AreEqual(expected, array.Length);
        }

        /// <summary>
        ///A test for GetStringAsUtf7Array
        ///</summary>
        [TestMethod()]
        [TestCategory("String extensions")]
        public void GetStringAsUtf7ArrayTest()
        {
            string self = "test";
            var expected = 4;
            var array = self.GetByteArray( EncodingType.Utf7);
            Assert.AreEqual(expected, array.Length);
        }

        /// <summary>
        ///A test for GetStringAsAsciiArray
        ///</summary>
        [TestMethod()]
        [TestCategory("String extensions")]
        public void GetStringAsAsciiArrayTest()
        {
            string self = "test";
            var expected = 4;
            var array = self.GetByteArray(EncodingType.Ascii);
            Assert.AreEqual(expected, array.Length);
        }
    }
}
