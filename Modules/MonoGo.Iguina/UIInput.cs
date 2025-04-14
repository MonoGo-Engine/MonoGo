using Iguina.Defs;
using Iguina.Drivers;
using Microsoft.Xna.Framework.Input;
using MonoGo.Engine;

namespace MonoGo.Iguina
{
    /// <summary>
    /// Provide input for GUI.
    /// </summary>
    internal class UIInput : IInputProvider
    {
        int _lastWheelValue;

        private List<int> _textInput = new();

        private List<int> ConvertToUnicode(string keyboardString)
        {
            List<int> unicodeValues = new();
            foreach (char c in keyboardString)
            {
                if (c.Equals('\b')) continue;               //Backspace
                else if (c.Equals('\u007f')) continue;      //Delete
                else if (c.Equals('\u001b')) continue;      //Escape
                else if (c.Equals('\t')) continue;          //Tab

                unicodeValues.Add(c);
            }
            return unicodeValues;
        }

        public void StartFrame()
        {
            _lastWheelValue = 0;
            _textInput = ConvertToUnicode(Input.KeyboardString);
        }

        public void EndFrame()
        {
            _lastWheelValue = Input.MouseWheelValue;
        }

        public Point GetMousePosition()
        {
            var mouse = Input.ScreenMousePosition;
            return new Point((int)mouse.X, (int)mouse.Y);
        }

        public bool IsMouseButtonDown(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left: return Input.CheckButton(Engine.Buttons.MouseLeft);
                case MouseButton.Right: return Input.CheckButton(Engine.Buttons.RightShift);
                case MouseButton.Wheel: return Input.CheckButton(Engine.Buttons.MouseMiddle);
            }
            return false;
        }

        public int GetMouseWheelChange()
        {
            var res = Math.Sign(Input.MouseWheelValue - _lastWheelValue);
            return res;
        }


        public int[] GetTextInput()
        {
            return _textInput.ToArray();
        }

        public TextInputCommands[] GetTextInputCommands()
        {
            List<TextInputCommands> ret = new();
            var keyboard = Keyboard.GetState();
            var ctrlDown = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            long millisecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            {
                foreach (var value in Enum.GetValues(typeof(TextInputCommands)))
                {
                    var key = _inputTextCommandToKeyboardKey[(int)value];
                    long msPassed = millisecondsSinceEpoch - _timeToAllowNextInputCommand[(int)value];
                    if (keyboard.IsKeyDown(key))
                    {
                        if (msPassed > 0)
                        {
                            _timeToAllowNextInputCommand[(int)value] = (millisecondsSinceEpoch + (msPassed >= 250 ? 450 : 45));
                            var command = (TextInputCommands)value;
                            if ((command == TextInputCommands.MoveCaretEnd) && !ctrlDown) { continue; }
                            if ((command == TextInputCommands.MoveCaretEndOfLine) && ctrlDown) { continue; }
                            if ((command == TextInputCommands.MoveCaretStart) && !ctrlDown) { continue; }
                            if ((command == TextInputCommands.MoveCaretStartOfLine) && ctrlDown) { continue; }
                            ret.Add(command);
                        }
                    }
                    else
                    {
                        _timeToAllowNextInputCommand[(int)value] = 0;
                    }
                }
            }
            return ret.ToArray();
        }

        // to add rate delay and ray limit to input commands
        long[] _timeToAllowNextInputCommand = new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // convert text input command to keyboard key
        /*
         * enum values:
            MoveCaretLeft,
            MoveCaretRight,
            MoveCaretUp,
            MoveCaretDown,
            Backspace,
            Delete,
            End,
            Start,
            EndOfLine,
            StartOfLine
         */
        static Keys[] _inputTextCommandToKeyboardKey = new Keys[]
        {
           Keys.Left,
           Keys.Right,
           Keys.Up,
           Keys.Down,
           Keys.Back,
           Keys.Delete,
           Keys.Enter,
           Keys.End,
           Keys.Home,
           Keys.End,
           Keys.Home
        };

        public KeyboardInteractions? GetKeyboardInteraction()
        {
            if (Input.CheckButton(Engine.Buttons.Left))
            {
                return KeyboardInteractions.MoveLeft;
            }
            if (Input.CheckButton(Engine.Buttons.Right))
            {
                return KeyboardInteractions.MoveRight;
            }
            if (Input.CheckButton(Engine.Buttons.Up))
            {
                return KeyboardInteractions.MoveUp;
            }
            if (Input.CheckButton(Engine.Buttons.Down))
            {
                return KeyboardInteractions.MoveDown;
            }
            if (Input.CheckButton(Engine.Buttons.Space) || Input.CheckButton(Engine.Buttons.Enter))
            {
                return KeyboardInteractions.Select;
            }
            return null;
        }
    }
}
