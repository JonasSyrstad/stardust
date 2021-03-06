//
// IBinaryConverter.cs
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

namespace Stardust.Particles.Converters
{
    public interface IBinaryConverter
    {
        /// <summary>
        /// Function converts byte array to it's correct string representation
        /// </summary>
        /// <param name="arrayToConvert">Array to be converted</param>
        /// <param name="delimiter">Delimiter to be inserted between bytes</param>
        /// <returns>String to represent given array</returns>
        string ByteArrayToString(byte[] arrayToConvert, string delimiter);

        /// <summary>
        /// Function converts given string to it's binary representation
        /// </summary>
        /// <param name="stringToConvert">String to convert to byte array</param>
        /// <returns>Byte array representing given string</returns>
        byte[] StringToByteArray(string stringToConvert);
    }
}