//
// isimplemapcontext.cs
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

using Stardust.Wormhole.MapBuilder;

namespace Stardust.Wormhole
{
    public interface ISimpleMapContext
    {
        /// <summary>
        /// Converts the source to the target type using the default map
        /// </summary>
        /// <typeparam name="TOut">The output type</typeparam>
        /// <returns></returns>
        TOut To<TOut>();

        /// <summary>
        /// Updates an existing object with data from the in data
        /// </summary>
        /// <typeparam name="TOut">The output type</typeparam>
        /// <returns></returns>
        TOut To<TOut>(TOut outItem);

        /// <summary>
        /// Converts the source to the target type using the provided map
        /// </summary>
        /// <typeparam name="TOut">The output type</typeparam>
        /// <param name="map">the map rule to use when converting</param>
        /// <returns></returns>
        TOut To<TOut>(IMapContainer map);

        /// <summary>
        /// Converts the source to the target type using the provided map
        /// </summary>
        /// <typeparam name="TOut">The output type</typeparam>
        /// <param name="map">the map rule to use when converting</param>
        /// <returns></returns>
        TOut To<TOut>(TOut outItem,IMapContainer map);
    }
}