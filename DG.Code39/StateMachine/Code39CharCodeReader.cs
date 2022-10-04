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

namespace DG.Code39.StateMachine
{
    // State machine class
    internal partial class Code39CharCodeReader
    {
        public static int CharLength = 15;

        private State _currentState;
        private readonly QuiteZoneState _quiteZoneState;
        private readonly CharCodeState _charCodeState;
        private readonly TrashState _trashState;

        private string? _result;
        private string _startStopTag = "0";
        private string _reverseStartStopTag = "0";

        private readonly string _text;

        private int _position;

        private bool _isReverseDirection;
        
        public Code39CharCodeReader(string text)
        {
            _currentState = _quiteZoneState = new QuiteZoneState();
            _charCodeState = new CharCodeState();
            _trashState = new TrashState();

            _text = text;
        }

        public void FindStartStopTag(string startStopTag, string reverseTag)
        {
            _currentState.Reset();
            _currentState = _quiteZoneState;
            _result = null;
            _startStopTag = startStopTag;
            _reverseStartStopTag = reverseTag;

            for (_position=0; _position < _text.Length; _position++)
            {
                if (_result != null)
                {
                    _position++; // skip space
                    if (_result != reverseTag) 
                        return;
                    _isReverseDirection = true;
                    FindStartStopTagRevert();
                    return;
                }

                _currentState.Process(_text[_position], this);
            }

            throw new Code39Exception($"Unable to find '*'-tag.");
        }

        private void FindStartStopTagRevert()
        {
            _currentState.Reset();
            _currentState = _quiteZoneState;
            _result = null;
            for (_position=_text.Length-1; _position >= 0; _position--)
            {
                if (_result != null)
                {
                    _position--; //skip space
                    return;
                }

                _currentState.Process(_text[_position], this);
            }

            throw new Code39Exception($"Unable to find '*'-tag.");
        }

        public string ReadNextCharCode()
        {
            return _isReverseDirection ? ReadNextRevertCharCode() : ReadNextDirectCharCode();
        }

        private string ReadNextDirectCharCode()
        {
            var builder = new StringBuilder();
            if (_position + CharLength > _text.Length)
                throw new Code39Exception("Barcode character is shorter (or longer) than expected.");

            for (int i = _position; i < _position + CharLength; i++)
            {
                var ch = _text[i];
                if (ch == '0' || ch == '1')
                    builder.Append(ch);
                else
                    throw new Code39Exception($"Unexpected character in barcode: \'{ch}\'.");
            }

            _position = Math.Min(_text.Length, _position + CharLength + 1); // +1 - means read space 

            return builder.ToString();
        }
        
        private string ReadNextRevertCharCode()
        {
            var builder = new StringBuilder();
            if (_position - CharLength < -1)
                throw new Code39Exception("Barcode character is shorter (or longer) than expected.");

            for (int i = _position; i > _position - CharLength; i--)
            {
                var ch = _text[i];
                if (ch == '0' || ch == '1')
                    builder.Append(ch);
                else
                    throw new Code39Exception($"Unexpected character in barcode: \'{ch}\'.");
            }

            _position = Math.Max(0, _position - CharLength - 1); // -1 - means read space 

            return builder.ToString();
        }

        private void SetQuiteZoneState()
        {
            _currentState.Reset();
            _currentState = _quiteZoneState;
        }

        private void SetCharCodeState(char c = default)
        {
            _currentState.Reset();
            _currentState = _charCodeState;
            if (c != default)
                _charCodeState.Append(c, this);
        }

        private void SetTrashState(int spaces = 0)
        {
            _currentState.Reset();
            _currentState = _trashState;
            _trashState.Spaces = spaces;
        }
    }
}