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
    internal class Code39
    {
        private const char _startStopTag = '*';
        private const char _space = '0';

        private readonly CharMap<char,string> _charMap = new();
        public Code39()
        {
            _charMap.Add('0', "101000111011101");
            _charMap.Add('1', "111010001010111");
            _charMap.Add('2', "101110001010111");
            _charMap.Add('3', "111011100010101");
            _charMap.Add('4', "101000111010111");
            _charMap.Add('5', "111010001110101");
            _charMap.Add('6', "101110001110101");
            _charMap.Add('7', "101000101110111");
            _charMap.Add('8', "111010001011101");
            _charMap.Add('9', "101110001011101");
            _charMap.Add('A', "111010100010111");
            _charMap.Add('B', "101110100010111");
            _charMap.Add('C', "111011101000101");
            _charMap.Add('D', "101011100010111");
            _charMap.Add('E', "111010111000101");
            _charMap.Add('F', "101110111000101");
            _charMap.Add('G', "101010001110111");
            _charMap.Add('H', "111010100011101");
            _charMap.Add('I', "101110100011101");
            _charMap.Add('J', "101011100011101");
            _charMap.Add('K', "111010101000111");
            _charMap.Add('L', "101110101000111");
            _charMap.Add('M', "111011101010001");
            _charMap.Add('N', "101011101000111");
            _charMap.Add('O', "111010111010001");
            _charMap.Add('P', "101110111010001");
            _charMap.Add('Q', "101010111000111");
            _charMap.Add('R', "111010101110001");
            _charMap.Add('S', "101110101110001");
            _charMap.Add('T', "101011101110001");
            _charMap.Add('U', "111000101010111");
            _charMap.Add('V', "100011101010111");
            _charMap.Add('W', "111000111010101");
            _charMap.Add('X', "100010111010111");
            _charMap.Add('Y', "111000101110101");
            _charMap.Add('Z', "100011101110101");
            _charMap.Add('-', "100010101110111");
            _charMap.Add('.', "111000101011101");
            _charMap.Add(' ', "100011101011101");
            _charMap.Add('$', "100010001000101");
            _charMap.Add('/', "100010001010001");
            _charMap.Add('+', "100010100010001");
            _charMap.Add('%', "101000100010001");
            _charMap.Add('*', "100010111011101");
        }

        public string Encode(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text.Length == 0)
                throw new ArgumentException("Input cannot be empty", nameof(text));

            var builder = new StringBuilder();
            builder.Append(_charMap.GetCode(_startStopTag));
            builder.Append(_space);

            foreach (var c in text)
            {
                if (c == _startStopTag || !_charMap.TryGetCode(c, out var code))
                    throw new Code39Exception($"This character cannot be encoded: '{c}'.");
                builder.Append(code);
                builder.Append(_space);
            }

            builder.Append(_charMap.GetCode(_startStopTag));

            return builder.ToString();
        }

        public string Decode(string barcode)
        {
            if (barcode == null)
                throw new ArgumentNullException(nameof(barcode));

            if (barcode.Length == 0)
                throw new ArgumentException("Input cannot be empty", nameof(barcode));

            var startStopCode = _charMap.GetCode(_startStopTag);
            var reader = new Code39CharCodeReader(barcode);

            var code = reader.ReadNextCharCode();
            if (code == startStopCode)
                return DecodeDirectBarcode(reader, barcode.Length);

            // try to read reverse barcode
            return DecodeRevertBarcode(reader, startStopCode, barcode.Length);
        }

        private string DecodeDirectBarcode(Code39CharCodeReader reader, int barcodeLength)
        {
            var builder = new StringBuilder();

            while (reader.Position < barcodeLength)
            {
                var code = reader.ReadNextCharCode();
                if (!_charMap.TryGetChar(code, out var ch))
                    throw new Code39Exception($"This charcode cannot be decoded: '{code}'.");

                if (ch == _startStopTag)
                    return builder.ToString();

                builder.Append(ch);
            }
            throw new Code39Exception($"Expected '{_startStopTag}' in the end of the barcode.");
        }

        private string DecodeRevertBarcode(Code39CharCodeReader reader, string startStopCode, int barcodeLength)
        {
            var builder = new StringBuilder();

            // set position to the end
            reader.Position = barcodeLength - 1;
            var code = reader.ReadNextRevertCharCode();
            if (code != startStopCode)
                throw new Code39Exception($"Expected '{_startStopTag}' in the beginning of the barcode.");

            while (reader.Position > 0)
            {
                code = reader.ReadNextRevertCharCode();
                if (!_charMap.TryGetChar(code, out var ch))
                    throw new Code39Exception($"This charcode cannot be decoded: '{code}'.");

                if (ch == _startStopTag)
                    return builder.ToString();

                builder.Append(ch);
            }
            throw new Code39Exception($"Expected '{_startStopTag}' in the end of the barcode.");
        }
    }
}
