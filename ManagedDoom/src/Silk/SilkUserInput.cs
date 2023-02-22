using System;
using System.Numerics;
using System.Runtime.ExceptionServices;
using Silk.NET.Input;
using Silk.NET.Windowing;
using ManagedDoom.UserInput;

namespace ManagedDoom.Silk
{
    public class SilkUserInput : IUserInput, IDisposable
    {
        private Config config;
        private IWindow window;

        private IInputContext input;
        private IKeyboard keyboard;

        private bool[] weaponKeys;
        private int turnHeld;

        private IMouse mouse;
        private bool mouseGrabbed;
        private float mouseX;
        private float mouseY;
        private float mousePrevX;
        private float mousePrevY;
        private float mouseDeltaX;
        private float mouseDeltaY;

        public SilkUserInput(Config config, IWindow window, SilkDoom doom, bool useMouse)
        {
            try
            {
                Console.Write("Initialize user input: ");

                this.config = config;
                this.window = window;

                input = window.CreateInput();

                keyboard = input.Keyboards[0];
                keyboard.KeyDown += (sender, key, value) => doom.KeyDown(key);
                keyboard.KeyUp += (sender, key, value) => doom.KeyUp(key);

                weaponKeys = new bool[7];
                turnHeld = 0;

                if (useMouse)
                {
                    mouse = input.Mice[0];
                    mouseGrabbed = false;
                }

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void BuildTicCmd(TicCmd cmd)
        {
            var keyForward = IsPressed(keyboard, config.key_forward);
            var keyBackward = IsPressed(keyboard, config.key_backward);
            var keyStrafeLeft = IsPressed(keyboard, config.key_strafeleft);
            var keyStrafeRight = IsPressed(keyboard, config.key_straferight);
            var keyTurnLeft = IsPressed(keyboard, config.key_turnleft);
            var keyTurnRight = IsPressed(keyboard, config.key_turnright);
            var keyFire = IsPressed(keyboard, config.key_fire);
            var keyUse = IsPressed(keyboard, config.key_use);
            var keyRun = IsPressed(keyboard, config.key_run);
            var keyStrafe = IsPressed(keyboard, config.key_strafe);

            weaponKeys[0] = keyboard.IsKeyPressed(Key.Number1);
            weaponKeys[1] = keyboard.IsKeyPressed(Key.Number2);
            weaponKeys[2] = keyboard.IsKeyPressed(Key.Number3);
            weaponKeys[3] = keyboard.IsKeyPressed(Key.Number4);
            weaponKeys[4] = keyboard.IsKeyPressed(Key.Number5);
            weaponKeys[5] = keyboard.IsKeyPressed(Key.Number6);
            weaponKeys[6] = keyboard.IsKeyPressed(Key.Number7);

            cmd.Clear();

            var strafe = keyStrafe;
            var speed = keyRun ? 1 : 0;
            var forward = 0;
            var side = 0;

            if (config.game_alwaysrun)
            {
                speed = 1 - speed;
            }

            if (keyTurnLeft || keyTurnRight)
            {
                turnHeld++;
            }
            else
            {
                turnHeld = 0;
            }

            int turnSpeed;
            if (turnHeld < PlayerBehavior.SlowTurnTics)
            {
                turnSpeed = 2;
            }
            else
            {
                turnSpeed = speed;
            }

            if (strafe)
            {
                if (keyTurnRight)
                {
                    side += PlayerBehavior.SideMove[speed];
                }
                if (keyTurnLeft)
                {
                    side -= PlayerBehavior.SideMove[speed];
                }
            }
            else
            {
                if (keyTurnRight)
                {
                    cmd.AngleTurn -= (short)PlayerBehavior.AngleTurn[turnSpeed];
                }
                if (keyTurnLeft)
                {
                    cmd.AngleTurn += (short)PlayerBehavior.AngleTurn[turnSpeed];
                }
            }

            if (keyForward)
            {
                forward += PlayerBehavior.ForwardMove[speed];
            }
            if (keyBackward)
            {
                forward -= PlayerBehavior.ForwardMove[speed];
            }

            if (keyStrafeLeft)
            {
                side -= PlayerBehavior.SideMove[speed];
            }
            if (keyStrafeRight)
            {
                side += PlayerBehavior.SideMove[speed];
            }

            if (keyFire)
            {
                cmd.Buttons |= TicCmdButtons.Attack;
            }

            if (keyUse)
            {
                cmd.Buttons |= TicCmdButtons.Use;
            }

            // Check weapon keys.
            for (var i = 0; i < weaponKeys.Length; i++)
            {
                if (weaponKeys[i])
                {
                    cmd.Buttons |= TicCmdButtons.Change;
                    cmd.Buttons |= (byte)(i << TicCmdButtons.WeaponShift);
                    break;
                }
            }

            UpdateMouse();
            var ms = 0.5F * config.mouse_sensitivity;
            var mx = (int)MathF.Round(ms * mouseDeltaX);
            var my = (int)MathF.Round(ms * -mouseDeltaY);
            forward += my;
            if (strafe)
            {
                side += mx * 2;
            }
            else
            {
                cmd.AngleTurn -= (short)(mx * 0x8);
            }

            if (forward > PlayerBehavior.MaxMove)
            {
                forward = PlayerBehavior.MaxMove;
            }
            else if (forward < -PlayerBehavior.MaxMove)
            {
                forward = -PlayerBehavior.MaxMove;
            }
            if (side > PlayerBehavior.MaxMove)
            {
                side = PlayerBehavior.MaxMove;
            }
            else if (side < -PlayerBehavior.MaxMove)
            {
                side = -PlayerBehavior.MaxMove;
            }

            cmd.ForwardMove += (sbyte)forward;
            cmd.SideMove += (sbyte)side;
        }

        private bool IsPressed(IKeyboard keyboard, KeyBinding keyBinding)
        {
            foreach (var key in keyBinding.Keys)
            {
                if (keyboard.IsKeyPressed(DoomToSilk(key)))
                {
                    return true;
                }
            }

            if (mouseGrabbed)
            {
                foreach (var mouseButton in keyBinding.MouseButtons)
                {
                    if (mouse.IsButtonPressed((MouseButton)mouseButton))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Reset()
        {
            if (mouse == null)
            {
                return;
            }

            mouseX = mouse.Position.X;
            mouseY = mouse.Position.Y;
            mousePrevX = mouseX;
            mousePrevY = mouseY;
            mouseDeltaX = 0;
            mouseDeltaY = 0;
        }

        public void GrabMouse()
        {
            if (mouse == null)
            {
                return;
            }

            if (!mouseGrabbed)
            {
                mouse.Cursor.CursorMode = CursorMode.Raw;
                mouseGrabbed = true;
                mouseX = mouse.Position.X;
                mouseY = mouse.Position.Y;
                mousePrevX = mouseX;
                mousePrevY = mouseY;
                mouseDeltaX = 0;
                mouseDeltaY = 0;
            }
        }

        public void ReleaseMouse()
        {
            if (mouse == null)
            {
                return;
            }

            if (mouseGrabbed)
            {
                mouse.Cursor.CursorMode = CursorMode.Normal;
                mouse.Position = new Vector2(window.Size.X - 10, window.Size.Y - 10);
                mouseGrabbed = false;
            }
        }

        private void UpdateMouse()
        {
            if (mouse == null)
            {
                return;
            }

            if (mouseGrabbed)
            {
                mousePrevX = mouseX;
                mousePrevY = mouseY;
                mouseX = mouse.Position.X;
                mouseY = mouse.Position.Y;
                mouseDeltaX = mouseX - mousePrevX;
                mouseDeltaY = mouseY - mousePrevY;

                if (config.mouse_disableyaxis)
                {
                    mouseDeltaY = 0;
                }
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown user input.");

            if (input != null)
            {
                input.Dispose();
                input = null;
            }
        }

        public static DoomKey SilkToDoom(Key silkKey)
        {
            switch (silkKey)
            {
                case Key.Space: return DoomKey.Space;
                // case Key.Apostrophe: return DoomKey.Apostrophe;
                case Key.Comma: return DoomKey.Comma;
                case Key.Minus: return DoomKey.Subtract;
                case Key.Period: return DoomKey.Period;
                case Key.Slash: return DoomKey.Slash;
                case Key.Number0: return DoomKey.Num0;
                // case Key.D0: return DoomKey.D0;
                case Key.Number1: return DoomKey.Num1;
                case Key.Number2: return DoomKey.Num2;
                case Key.Number3: return DoomKey.Num3;
                case Key.Number4: return DoomKey.Num4;
                case Key.Number5: return DoomKey.Num5;
                case Key.Number6: return DoomKey.Num6;
                case Key.Number7: return DoomKey.Num7;
                case Key.Number8: return DoomKey.Num8;
                case Key.Number9: return DoomKey.Num9;
                case Key.Semicolon: return DoomKey.Semicolon;
                case Key.Equal: return DoomKey.Equal;
                case Key.A: return DoomKey.A;
                case Key.B: return DoomKey.B;
                case Key.C: return DoomKey.C;
                case Key.D: return DoomKey.D;
                case Key.E: return DoomKey.E;
                case Key.F: return DoomKey.F;
                case Key.G: return DoomKey.G;
                case Key.H: return DoomKey.H;
                case Key.I: return DoomKey.I;
                case Key.J: return DoomKey.J;
                case Key.K: return DoomKey.K;
                case Key.L: return DoomKey.L;
                case Key.M: return DoomKey.M;
                case Key.N: return DoomKey.N;
                case Key.O: return DoomKey.O;
                case Key.P: return DoomKey.P;
                case Key.Q: return DoomKey.Q;
                case Key.R: return DoomKey.R;
                case Key.S: return DoomKey.S;
                case Key.T: return DoomKey.T;
                case Key.U: return DoomKey.U;
                case Key.V: return DoomKey.V;
                case Key.W: return DoomKey.W;
                case Key.X: return DoomKey.X;
                case Key.Y: return DoomKey.Y;
                case Key.Z: return DoomKey.Z;
                case Key.LeftBracket: return DoomKey.LBracket;
                case Key.BackSlash: return DoomKey.Backslash;
                case Key.RightBracket: return DoomKey.RBracket;
                // case Key.GraveAccent: return DoomKey.GraveAccent;
                // case Key.World1: return DoomKey.World1;
                // case Key.World2: return DoomKey.World2;
                case Key.Escape: return DoomKey.Escape;
                case Key.Enter: return DoomKey.Enter;
                case Key.Tab: return DoomKey.Tab;
                case Key.Backspace: return DoomKey.Backspace;
                case Key.Insert: return DoomKey.Insert;
                case Key.Delete: return DoomKey.Delete;
                case Key.Right: return DoomKey.Right;
                case Key.Left: return DoomKey.Left;
                case Key.Down: return DoomKey.Down;
                case Key.Up: return DoomKey.Up;
                case Key.PageUp: return DoomKey.PageUp;
                case Key.PageDown: return DoomKey.PageDown;
                case Key.Home: return DoomKey.Home;
                case Key.End: return DoomKey.End;
                // case Key.CapsLock: return DoomKey.CapsLock;
                // case Key.ScrollLock: return DoomKey.ScrollLock;
                // case Key.NumLock: return DoomKey.NumLock;
                // case Key.PrintScreen: return DoomKey.PrintScreen;
                case Key.Pause: return DoomKey.Pause;
                case Key.F1: return DoomKey.F1;
                case Key.F2: return DoomKey.F2;
                case Key.F3: return DoomKey.F3;
                case Key.F4: return DoomKey.F4;
                case Key.F5: return DoomKey.F5;
                case Key.F6: return DoomKey.F6;
                case Key.F7: return DoomKey.F7;
                case Key.F8: return DoomKey.F8;
                case Key.F9: return DoomKey.F9;
                case Key.F10: return DoomKey.F10;
                case Key.F11: return DoomKey.F11;
                case Key.F12: return DoomKey.F12;
                case Key.F13: return DoomKey.F13;
                case Key.F14: return DoomKey.F14;
                case Key.F15: return DoomKey.F15;
                // case Key.F16: return DoomKey.F16;
                // case Key.F17: return DoomKey.F17;
                // case Key.F18: return DoomKey.F18;
                // case Key.F19: return DoomKey.F19;
                // case Key.F20: return DoomKey.F20;
                // case Key.F21: return DoomKey.F21;
                // case Key.F22: return DoomKey.F22;
                // case Key.F23: return DoomKey.F23;
                // case Key.F24: return DoomKey.F24;
                // case Key.F25: return DoomKey.F25;
                case Key.Keypad0: return DoomKey.Numpad0;
                case Key.Keypad1: return DoomKey.Numpad1;
                case Key.Keypad2: return DoomKey.Numpad2;
                case Key.Keypad3: return DoomKey.Numpad3;
                case Key.Keypad4: return DoomKey.Numpad4;
                case Key.Keypad5: return DoomKey.Numpad5;
                case Key.Keypad6: return DoomKey.Numpad6;
                case Key.Keypad7: return DoomKey.Numpad7;
                case Key.Keypad8: return DoomKey.Numpad8;
                case Key.Keypad9: return DoomKey.Numpad9;
                // case Key.KeypadDecimal: return DoomKey.Decimal;
                case Key.KeypadDivide: return DoomKey.Divide;
                case Key.KeypadMultiply: return DoomKey.Multiply;
                case Key.KeypadSubtract: return DoomKey.Subtract;
                case Key.KeypadAdd: return DoomKey.Add;
                case Key.KeypadEnter: return DoomKey.Enter;
                case Key.KeypadEqual: return DoomKey.Equal;
                case Key.ShiftLeft: return DoomKey.LShift;
                case Key.ControlLeft: return DoomKey.LControl;
                case Key.AltLeft: return DoomKey.LAlt;
                // case Key.SuperLeft: return DoomKey.SuperLeft;
                case Key.ShiftRight: return DoomKey.RShift;
                case Key.ControlRight: return DoomKey.RControl;
                case Key.AltRight: return DoomKey.RAlt;
                // case Key.SuperRight: return DoomKey.SuperRight;
                case Key.Menu: return DoomKey.Menu;
                default: return DoomKey.Unknown;
            }
        }

        public static Key DoomToSilk(DoomKey key)
        {
            switch (key)
            {
                case DoomKey.A: return Key.A;
                case DoomKey.B: return Key.B;
                case DoomKey.C: return Key.C;
                case DoomKey.D: return Key.D;
                case DoomKey.E: return Key.E;
                case DoomKey.F: return Key.F;
                case DoomKey.G: return Key.G;
                case DoomKey.H: return Key.H;
                case DoomKey.I: return Key.I;
                case DoomKey.J: return Key.J;
                case DoomKey.K: return Key.K;
                case DoomKey.L: return Key.L;
                case DoomKey.M: return Key.M;
                case DoomKey.N: return Key.N;
                case DoomKey.O: return Key.O;
                case DoomKey.P: return Key.P;
                case DoomKey.Q: return Key.Q;
                case DoomKey.R: return Key.R;
                case DoomKey.S: return Key.S;
                case DoomKey.T: return Key.T;
                case DoomKey.U: return Key.U;
                case DoomKey.V: return Key.V;
                case DoomKey.W: return Key.W;
                case DoomKey.X: return Key.X;
                case DoomKey.Y: return Key.Y;
                case DoomKey.Z: return Key.Z;
                case DoomKey.Num0: return Key.Number0;
                case DoomKey.Num1: return Key.Number1;
                case DoomKey.Num2: return Key.Number2;
                case DoomKey.Num3: return Key.Number3;
                case DoomKey.Num4: return Key.Number4;
                case DoomKey.Num5: return Key.Number5;
                case DoomKey.Num6: return Key.Number6;
                case DoomKey.Num7: return Key.Number7;
                case DoomKey.Num8: return Key.Number8;
                case DoomKey.Num9: return Key.Number9;
                case DoomKey.Escape: return Key.Escape;
                case DoomKey.LControl: return Key.ControlLeft;
                case DoomKey.LShift: return Key.ShiftLeft;
                case DoomKey.LAlt: return Key.AltLeft;
                // case DoomKey.LSystem: return Key.LSystem;
                case DoomKey.RControl: return Key.ControlRight;
                case DoomKey.RShift: return Key.ShiftRight;
                case DoomKey.RAlt: return Key.AltRight;
                // case DoomKey.RSystem: return Key.RSystem;
                case DoomKey.Menu: return Key.Menu;
                case DoomKey.LBracket: return Key.LeftBracket;
                case DoomKey.RBracket: return Key.RightBracket;
                case DoomKey.Semicolon: return Key.Semicolon;
                case DoomKey.Comma: return Key.Comma;
                case DoomKey.Period: return Key.Period;
                // case DoomKey.Quote: return Key.Quote;
                case DoomKey.Slash: return Key.Slash;
                case DoomKey.Backslash: return Key.BackSlash;
                // case DoomKey.Tilde: return Key.Tilde;
                case DoomKey.Equal: return Key.Equal;
                // case DoomKey.Hyphen: return Key.Hyphen;
                case DoomKey.Space: return Key.Space;
                case DoomKey.Enter: return Key.Enter;
                case DoomKey.Backspace: return Key.Backspace;
                case DoomKey.Tab: return Key.Tab;
                case DoomKey.PageUp: return Key.PageUp;
                case DoomKey.PageDown: return Key.PageDown;
                case DoomKey.End: return Key.End;
                case DoomKey.Home: return Key.Home;
                case DoomKey.Insert: return Key.Insert;
                case DoomKey.Delete: return Key.Delete;
                case DoomKey.Add: return Key.KeypadAdd;
                case DoomKey.Subtract: return Key.KeypadSubtract;
                case DoomKey.Multiply: return Key.KeypadMultiply;
                case DoomKey.Divide: return Key.KeypadDivide;
                case DoomKey.Left: return Key.Left;
                case DoomKey.Right: return Key.Right;
                case DoomKey.Up: return Key.Up;
                case DoomKey.Down: return Key.Down;
                case DoomKey.Numpad0: return Key.Number0;
                case DoomKey.Numpad1: return Key.Number1;
                case DoomKey.Numpad2: return Key.Number2;
                case DoomKey.Numpad3: return Key.Number3;
                case DoomKey.Numpad4: return Key.Number4;
                case DoomKey.Numpad5: return Key.Number5;
                case DoomKey.Numpad6: return Key.Number6;
                case DoomKey.Numpad7: return Key.Number7;
                case DoomKey.Numpad8: return Key.Number8;
                case DoomKey.Numpad9: return Key.Number9;
                case DoomKey.F1: return Key.F1;
                case DoomKey.F2: return Key.F2;
                case DoomKey.F3: return Key.F3;
                case DoomKey.F4: return Key.F4;
                case DoomKey.F5: return Key.F5;
                case DoomKey.F6: return Key.F6;
                case DoomKey.F7: return Key.F7;
                case DoomKey.F8: return Key.F8;
                case DoomKey.F9: return Key.F9;
                case DoomKey.F10: return Key.F10;
                case DoomKey.F11: return Key.F11;
                case DoomKey.F12: return Key.F12;
                case DoomKey.F13: return Key.F13;
                case DoomKey.F14: return Key.F14;
                case DoomKey.F15: return Key.F15;
                case DoomKey.Pause: return Key.Pause;
                default: return Key.Unknown;
            }
        }

        public int MaxMouseSensitivity
        {
            get
            {
                return 15;
            }
        }

        public int MouseSensitivity
        {
            get
            {
                return config.mouse_sensitivity;
            }

            set
            {
                config.mouse_sensitivity = value;
            }
        }
    }
}
