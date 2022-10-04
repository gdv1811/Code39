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

namespace DG.Code39.StateMachine;

internal partial class Code39CharCodeReader
{
    private class CharCodeState : State
    {
        private readonly StringBuilder _builder = new();
        private int _spaces;

        protected override void ProcessZero(Code39CharCodeReader machine)
        {
            _spaces++;
            if (_spaces > 3)
            {
                machine.SetQuiteZoneState();
                return;
            }

            Append('0', machine);
        }

        protected override void ProcessOne(Code39CharCodeReader machine)
        {
            _spaces = 0;
            Append('1', machine);
        }

        protected override void ProcessTrash(Code39CharCodeReader machine)
        {
            machine.SetTrashState();
        }

        public void Append(char c, Code39CharCodeReader machine)
        {
            _builder.Append(c);
            if (_builder.Length < CharLength)
                return;
            if (_builder.Equals(machine._startStopTag) || _builder.Equals(machine._reverseStartStopTag))
            {
                machine._result = _builder.ToString();
                return;
            }

            machine.SetTrashState(_spaces);
        }

        public override void Reset()
        {
            _builder.Clear();
            _spaces = 0;
        }
    }
}