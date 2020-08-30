using Overkill.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Interfaces
{
    public interface IInputService
    {
        void Keyboard(string name, KeyboardKey key, Action<InputState> action);
        void GamepadJoystick(string name, GamepadInput input, Action<(bool isPressed, float x, float y)> action);
        void GamepadTrigger(string name, GamepadInput input, Action<float> action);
        void GamepadButton(string name, GamepadInput input, Action<InputState> action);
        Dictionary<string, int> GetKeyboardInputs();
        Dictionary<string, int> GetGamepadInputs();
    }
}
