using Overkill.Common.Enums;
using Overkill.Common.Exceptions;
using Overkill.Core.Interfaces;
using Overkill.Core.Topics;
using Overkill.Core.Topics.Input;
using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overkill.Core
{
    /// <summary>
    /// Manages registered inputs for manual driving by users through a front-end application.
    /// </summary>
    public class InputService : IInputService
    {
        private IOverkillConfiguration config;
        
        private Dictionary<string, KeyboardKey> defaultKeyboardBindings;
        private Dictionary<string, GamepadInput> defaultGamepadBindings;

        private Dictionary<string, Action<InputState>> keyboardBindings;
        private Dictionary<string, Action<(bool isPressed, float x, float y)>> gamepadJoystickBindings;
        private Dictionary<string, Action<InputState>> gamepadButtonBindings;
        private Dictionary<string, Action<float>> gamepadTriggerBindings;

        public InputService(IPubSubService pubSub, IOverkillConfiguration _config)
        {
            config = _config;

            defaultKeyboardBindings = new Dictionary<string, KeyboardKey>();
            defaultGamepadBindings = new Dictionary<string, GamepadInput>();

            keyboardBindings = new Dictionary<string, Action<InputState>>();
            gamepadJoystickBindings = new Dictionary<string, Action<(bool isPressed, float x, float y)>>();
            gamepadButtonBindings = new Dictionary<string, Action<InputState>>();
            gamepadTriggerBindings = new Dictionary<string, Action<float>>();

            pubSub.Subscribe<KeyboardInputTopic>(topic =>
            {
                EmitKeyboardEvent(topic.Name, topic.IsPressed ? InputState.Pressed : InputState.Released);
            });

            pubSub.Subscribe<GamepadJoystickInputTopic>(topic =>
            {
                EmitGamepadJoystickEvent(topic.Name, topic.IsPressed, topic.X, topic.Y);
            });

            pubSub.Subscribe<GamepadButtonInputTopic>(topic =>
            {
                EmitGamepadButtonEvent(topic.Name, topic.State);
            });

            pubSub.Subscribe<GamepadTriggerInputTopic>(topic =>
            {
                EmitGamepadTriggerEvent(topic.Name, topic.Value);
            });
        }

        /// <summary>
        /// Binds a keyboard key to an action, by a reference name.
        /// Throws an exception if an action already exists for the input, it is not defined in the configuration, or the value is invalid.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultBinding"></param>
        /// <param name="action"></param>
        public void Keyboard(string name, KeyboardKey defaultBinding, Action<InputState> action)
        {
            if (!config.Input.Keyboard.ContainsKey(name) || !Enum.IsDefined(typeof(KeyboardKey), config.Input.Keyboard[name])) throw new InvalidInputConfigurationException(name);
            if (keyboardBindings.ContainsKey(name)) throw new InputAlreadyBoundException(name);
            defaultKeyboardBindings.Add(name, defaultBinding);
            keyboardBindings.Add(name, action);
        }

        /// <summary>
        /// Binds a joystick event to an action, by a reference name.
        /// Throws an exception if an action already exists for the input, it is not defined in the configuration, or the value is invalid.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void GamepadJoystick(string name, GamepadInput defaultBinding, Action<(bool isPressed, float x, float y)> action)
        {
            if (!config.Input.Gamepad.ContainsKey(name) || !Enum.IsDefined(typeof(GamepadInput), config.Input.Gamepad[name])) throw new InvalidInputConfigurationException(name);
            if (gamepadJoystickBindings.ContainsKey(name)) throw new InputAlreadyBoundException(name);
            defaultGamepadBindings.Add(name, defaultBinding);
            gamepadJoystickBindings.Add(name, action);
        }

        /// <summary>
        /// Binds a gamepad trigger to an action, by a reference name.
        /// Throws an exception if an action already exists for the input, it is not defined in the configuration, or the value is invalid.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultBinding"></param>
        /// <param name="action"></param>
        public void GamepadTrigger(string name, GamepadInput defaultBinding, Action<float> action)
        {
            if (!config.Input.Gamepad.ContainsKey(name) || !Enum.IsDefined(typeof(GamepadInput), config.Input.Gamepad[name])) throw new InvalidInputConfigurationException(name);
            if (gamepadTriggerBindings.ContainsKey(name)) throw new InputAlreadyBoundException(name);
            defaultGamepadBindings.Add(name, defaultBinding);
            gamepadTriggerBindings.Add(name, action);
        }

        /// <summary>
        /// Binds a gamepad button to an action, by a reference name.
        /// Throws an exception if an action already exists for the input, it is not defined in the configuration, or the value is invalid.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void GamepadButton(string name, GamepadInput defaultBinding, Action<InputState> action)
        {
            if (!config.Input.Gamepad.ContainsKey(name) || !Enum.IsDefined(typeof(GamepadInput), config.Input.Gamepad[name])) throw new InvalidInputConfigurationException(name);
            if (gamepadButtonBindings.ContainsKey(name)) throw new InputAlreadyBoundException(name);
            defaultGamepadBindings.Add(name, defaultBinding);
            gamepadButtonBindings.Add(name, action);
        }

        /// <summary>
        /// Returns a dictionary of (input name, default key code)
        /// </summary>
        public Dictionary<string, int> GetKeyboardInputs()
        {
            return keyboardBindings.ToDictionary(x => x.Key, x => (int)defaultKeyboardBindings[x.Key]);
        }

        /// <summary>
        /// Returns a dictionary of gamepad inputs (input name, default button index)
        /// </summary>
        public Dictionary<string, int> GetGamepadInputs()
        {
            return gamepadJoystickBindings.ToDictionary(x => x.Key, x => (int)defaultGamepadBindings[x.Key]);
        }

        /// <summary>
        /// Called internally via PubSub subscription, used to invoke a callback action
        /// </summary>
        /// <param name="name">The friendly input name</param>
        /// <param name="state">The state of the keyboard key, pressed or released</param>
        private void EmitKeyboardEvent(string name, InputState state)
        {
            if (!keyboardBindings.ContainsKey(name)) return;
            keyboardBindings[name](state);
        }

        /// <summary>
        /// Called internally via PubSub subscription, used to invoke a callback function
        /// </summary>
        /// <param name="name">The friendly input name</param>
        /// <param name="x">The X axis of the joystick</param>
        /// <param name="y">The Y axis of the joystick</param>
        private void EmitGamepadJoystickEvent(string name, bool isPressed, float x, float y)
        {
            if (!gamepadJoystickBindings.ContainsKey(name)) return;
            gamepadJoystickBindings[name]((isPressed, x, y));
        }

        /// <summary>
        /// Called internally via PubSub subscription, used to invoke a callback function
        /// </summary>
        /// <param name="name">The friendly input name</param>
        /// <param name="state">The state of the gamepad button, pressed or released</param>
        private void EmitGamepadButtonEvent(string name, InputState state)
        {
            if (!gamepadButtonBindings.ContainsKey(name)) return;
            gamepadButtonBindings[name](state);
        }

        /// <summary>
        /// Called internally via PubSub subscription, used to invoke a callback function
        /// </summary>
        /// <param name="name">The friendly input name</param>
        /// <param name="value">The value of the trigger (how held down it is)</param>
        private void EmitGamepadTriggerEvent(string name, float value)
        {
            if (!gamepadTriggerBindings.ContainsKey(name)) return;
            gamepadTriggerBindings[name](value);
        }
    }
}
