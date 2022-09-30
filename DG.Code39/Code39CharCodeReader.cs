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

using System.Text;

namespace DG.Code39
{
    internal class Code39CharCodeReader
    {
        private const int _charLength = 15;
        private readonly string _text;

        public int Position { get; set; }

        public Code39CharCodeReader(string text)
        {
            _text = text;
        }

        public string ReadNextCharCode()
        {
            var builder = new StringBuilder();
            if (Position + _charLength > _text.Length)
                throw new Code39Exception($"Barcode character is shorter (or longer) than expected.");

            for (int i = Position; i < Position + _charLength; i++)
            {
                var ch = _text[i];
                if (ch == '0' || ch == '1')
                    builder.Append(ch);
                else
                    throw new Code39Exception($"Unexpected character in barcode: \'{ch}\'.");
            }

            Position = Math.Min(_text.Length, Position + _charLength + 1); // +1 - means readed space 

            return builder.ToString();
        }


        public string ReadNextRevertCharCode()
        {
            var builder = new StringBuilder();
            if (Position - _charLength < -1)
                throw new Code39Exception($"Barcode character is shorter (or longer) than expected.");

            for (int i = Position; i > Position - _charLength; i--)
            {
                var ch = _text[i];
                if (ch == '0' || ch == '1')
                    builder.Append(ch);
                else
                    throw new Code39Exception($"Unexpected character in barcode: \'{ch}\'.");
            }

            Position = Math.Max(0, Position - _charLength - 1); // -1 - means readed space 

            return builder.ToString();
        }
    }
}
