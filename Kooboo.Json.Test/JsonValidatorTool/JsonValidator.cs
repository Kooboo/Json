
#region License, Terms and Author(s)
//
// JSON Checker
// Copyright (c) 2018 Wendell Sampaio. All rights reserved.
//
//  Author(s):
//
//      Wendell Sampaio, https://wendellantildes.github.io
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the New BSD License, a copy of which should have 
// been delivered along with this distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
#endregion

#region Original JsonChecker License, Terms and Author(s)
//
// JSON Checker
// Copyright (c) 2007 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the New BSD License, a copy of which should have 
// been delivered along with this distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
#endregion

#region Original JSON_checker.c License

/* 2007-08-24 */

/*
Copyright (c) 2005 JSON.org
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
The Software shall be used for Good, not Evil.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion


using System.Collections.Generic;
using System.IO;

namespace JsonValidatorTool
{
    /// <summary>
    /// The JsonValidator class contains all methods to validate json strings.
    /// </summary>
    public static class JsonValidator
    {
        /// <summary>
        /// Validates the specified json.
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="depth">It restricts the level of maximum nesting.</param>
        /// <exception cref="JsonNotValidException"></exception>
        public static void Validate(string json, int? depth = null)
        {
            using (var reader = new StringReader(json))
            {
                ValidateJson(ReadChars(reader), depth);
            }
        }

        /// <summary>
        /// Validates the specified json.
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="depth">It restricts the level of maximum nesting.</param>
        public static bool IsValid(string json, int? depth = null)
        {
            var isint = int.TryParse(json, out _);
            var isbool = bool.TryParse(json, out _);
            var isdecimal = decimal.TryParse(json,out _);
            var isfloat = float.TryParse(json, out _);
            var isdouble = double.TryParse(json,out _);
            if (json == "null" || isint || json.StartsWith("\"") && json.EndsWith("\"") || isbool|| isdecimal|| isfloat|| isdouble)
                return true;
            try
            {
                Validate(json, depth);
                return true;
            }
            catch (JsonNotValidException)
            {
                return false;
            }

        }

        static void ValidateJson(IEnumerable<char> chars, int? depth = null)
        {
            var checker = depth.HasValue ? new JsonChecker(depth.Value) : new JsonChecker();
            foreach (char ch in chars)
            {
                checker.Check(ch);
            }
            checker.FinalCheck();
        }

        static IEnumerable<char> ReadChars(TextReader reader)
        {
            int ch = reader.Read();
            while (ch != -1)
            {
                yield return (char)ch;
                ch = reader.Read();
            }
        }
    }
}
