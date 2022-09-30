// Copyright (c) 2022 Dmitry Goryachev
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

namespace DG.Code39
{
    public class CharMap<TChar, TCode>
        where TChar : notnull
        where TCode : notnull
    {
        private readonly Dictionary<TChar, TCode> _charToCode = new();
        private readonly Dictionary<TCode, TChar> _codeToChar = new();

        public void Add(TChar @char, TCode code)
        {
            _charToCode.Add(@char, code);
            _codeToChar.Add(code, @char);
        }

        public TChar GetChar(TCode code)
        {
            return _codeToChar[code];
        }

        public TCode GetCode(TChar @char)
        {
            return _charToCode[@char];
        }

        public bool TryGetChar(TCode code, out TChar @char)
        {
            return _codeToChar.TryGetValue(code, out @char);
        }

        public bool TryGetCode(TChar @char, out TCode code)
        {
            return _charToCode.TryGetValue(@char, out code);
        }
    }
}
