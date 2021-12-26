using System;
using System.Runtime.ExceptionServices;
using Raylib_CsLo;
using ManagedDoom.UserInput;

namespace ManagedDoom.Raylib_CsLo
{
    public sealed class RaylibUserInput : IUserInput, IDisposable
    {
        private Config config;

        private bool useMouse;

        private bool[] weaponKeys;
        private int turnHeld;

        private bool mouseGrabbed;

        public RaylibUserInput(Config config, bool useMouse)
        {
            try
            {
                Console.Write("Initialize user input: ");

                this.config = config;

                config.mouse_sensitivity = Math.Max(config.mouse_sensitivity, 0);

                this.useMouse = useMouse;

                weaponKeys = new bool[7];
                turnHeld = 0;

                mouseGrabbed = false;

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
            var keyForward = IsPressed(config.key_forward);
            var keyBackward = IsPressed(config.key_backward);
            var keyStrafeLeft = IsPressed(config.key_strafeleft);
            var keyStrafeRight = IsPressed(config.key_straferight);
            var keyTurnLeft = IsPressed(config.key_turnleft);
            var keyTurnRight = IsPressed(config.key_turnright);
            var keyFire = IsPressed(config.key_fire);
            var keyUse = IsPressed(config.key_use);
            var keyRun = IsPressed(config.key_run);
            var keyStrafe = IsPressed(config.key_strafe);

            weaponKeys[0] = Raylib.IsKeyDown(KeyboardKey.KEY_ONE);
            weaponKeys[1] = Raylib.IsKeyDown(KeyboardKey.KEY_TWO);
            weaponKeys[2] = Raylib.IsKeyDown(KeyboardKey.KEY_THREE);
            weaponKeys[3] = Raylib.IsKeyDown(KeyboardKey.KEY_FOUR);
            weaponKeys[4] = Raylib.IsKeyDown(KeyboardKey.KEY_FIVE);
            weaponKeys[5] = Raylib.IsKeyDown(KeyboardKey.KEY_SIX);
            weaponKeys[6] = Raylib.IsKeyDown(KeyboardKey.KEY_SEVEN);

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

            var mouseDelta = Raylib.GetMouseDelta();
            var ms = 0.5F * config.mouse_sensitivity;
            var mx = (int)MathF.Round(ms * mouseDelta.X);
            var my = (int)MathF.Round(-ms * mouseDelta.Y);
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

        private bool IsPressed(KeyBinding keyBinding)
        {
            foreach (var key in keyBinding.Keys)
            {
                if (Raylib.IsKeyDown(DoomToRay(key)))
                {
                    return true;
                }
            }

            if (mouseGrabbed)
            {
                foreach (var mouseButton in keyBinding.MouseButtons)
                {
                    if (Raylib.IsMouseButtonDown((MouseButton)mouseButton))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Reset()
        {
        }

        public void GrabMouse()
        {
            if (useMouse && !mouseGrabbed)
            {
                Raylib.DisableCursor();
                mouseGrabbed = true;
            }
        }

        public void ReleaseMouse()
        {
            if (useMouse && mouseGrabbed)
            {
                Raylib.EnableCursor();
                mouseGrabbed = false;
            }
        }

        public static DoomKey RayToDoom(KeyboardKey rayKey)
        {
            switch (rayKey)
            {
                case KeyboardKey.KEY_BACKSPACE: return DoomKey.Backspace;
                case KeyboardKey.KEY_TAB: return DoomKey.Tab;
                case KeyboardKey.KEY_ENTER: return DoomKey.Enter;
                case KeyboardKey.KEY_PAUSE: return DoomKey.Pause;
                case KeyboardKey.KEY_ESCAPE: return DoomKey.Escape;
                case KeyboardKey.KEY_SPACE: return DoomKey.Space;
                case KeyboardKey.KEY_PAGE_UP: return DoomKey.PageUp;
                case KeyboardKey.KEY_PAGE_DOWN: return DoomKey.PageDown;
                case KeyboardKey.KEY_END: return DoomKey.End;
                case KeyboardKey.KEY_HOME: return DoomKey.Home;
                case KeyboardKey.KEY_LEFT: return DoomKey.Left;
                case KeyboardKey.KEY_UP: return DoomKey.Up;
                case KeyboardKey.KEY_RIGHT: return DoomKey.Right;
                case KeyboardKey.KEY_DOWN: return DoomKey.Down;
                case KeyboardKey.KEY_INSERT: return DoomKey.Insert;
                case KeyboardKey.KEY_DELETE: return DoomKey.Delete;
                case KeyboardKey.KEY_ZERO: return DoomKey.Num0;
                case KeyboardKey.KEY_ONE: return DoomKey.Num1;
                case KeyboardKey.KEY_TWO: return DoomKey.Num2;
                case KeyboardKey.KEY_THREE: return DoomKey.Num3;
                case KeyboardKey.KEY_FOUR: return DoomKey.Num4;
                case KeyboardKey.KEY_FIVE: return DoomKey.Num5;
                case KeyboardKey.KEY_SIX: return DoomKey.Num6;
                case KeyboardKey.KEY_SEVEN: return DoomKey.Num7;
                case KeyboardKey.KEY_EIGHT: return DoomKey.Num8;
                case KeyboardKey.KEY_NINE: return DoomKey.Num9;
                case KeyboardKey.KEY_A: return DoomKey.A;
                case KeyboardKey.KEY_B: return DoomKey.B;
                case KeyboardKey.KEY_C: return DoomKey.C;
                case KeyboardKey.KEY_D: return DoomKey.D;
                case KeyboardKey.KEY_E: return DoomKey.E;
                case KeyboardKey.KEY_F: return DoomKey.F;
                case KeyboardKey.KEY_G: return DoomKey.G;
                case KeyboardKey.KEY_H: return DoomKey.H;
                case KeyboardKey.KEY_I: return DoomKey.I;
                case KeyboardKey.KEY_J: return DoomKey.J;
                case KeyboardKey.KEY_K: return DoomKey.K;
                case KeyboardKey.KEY_L: return DoomKey.L;
                case KeyboardKey.KEY_M: return DoomKey.M;
                case KeyboardKey.KEY_N: return DoomKey.N;
                case KeyboardKey.KEY_O: return DoomKey.O;
                case KeyboardKey.KEY_P: return DoomKey.P;
                case KeyboardKey.KEY_Q: return DoomKey.Q;
                case KeyboardKey.KEY_R: return DoomKey.R;
                case KeyboardKey.KEY_S: return DoomKey.S;
                case KeyboardKey.KEY_T: return DoomKey.T;
                case KeyboardKey.KEY_U: return DoomKey.U;
                case KeyboardKey.KEY_V: return DoomKey.V;
                case KeyboardKey.KEY_W: return DoomKey.W;
                case KeyboardKey.KEY_X: return DoomKey.X;
                case KeyboardKey.KEY_Y: return DoomKey.Y;
                case KeyboardKey.KEY_Z: return DoomKey.Z;
                case KeyboardKey.KEY_KP_0: return DoomKey.Numpad0;
                case KeyboardKey.KEY_KP_1: return DoomKey.Numpad1;
                case KeyboardKey.KEY_KP_2: return DoomKey.Numpad2;
                case KeyboardKey.KEY_KP_3: return DoomKey.Numpad3;
                case KeyboardKey.KEY_KP_4: return DoomKey.Numpad4;
                case KeyboardKey.KEY_KP_5: return DoomKey.Numpad5;
                case KeyboardKey.KEY_KP_6: return DoomKey.Numpad6;
                case KeyboardKey.KEY_KP_7: return DoomKey.Numpad7;
                case KeyboardKey.KEY_KP_8: return DoomKey.Numpad8;
                case KeyboardKey.KEY_KP_9: return DoomKey.Numpad9;
                case KeyboardKey.KEY_KP_MULTIPLY: return DoomKey.Multiply;
                case KeyboardKey.KEY_KP_ADD: return DoomKey.Add;
                case KeyboardKey.KEY_KP_SUBTRACT: return DoomKey.Subtract;
                case KeyboardKey.KEY_KP_DIVIDE: return DoomKey.Divide;
                case KeyboardKey.KEY_F1: return DoomKey.F1;
                case KeyboardKey.KEY_F2: return DoomKey.F2;
                case KeyboardKey.KEY_F3: return DoomKey.F3;
                case KeyboardKey.KEY_F4: return DoomKey.F4;
                case KeyboardKey.KEY_F5: return DoomKey.F5;
                case KeyboardKey.KEY_F6: return DoomKey.F6;
                case KeyboardKey.KEY_F7: return DoomKey.F7;
                case KeyboardKey.KEY_F8: return DoomKey.F8;
                case KeyboardKey.KEY_F9: return DoomKey.F9;
                case KeyboardKey.KEY_F10: return DoomKey.F10;
                case KeyboardKey.KEY_F11: return DoomKey.F11;
                case KeyboardKey.KEY_F12: return DoomKey.F12;
                case KeyboardKey.KEY_LEFT_SHIFT: return DoomKey.LShift;
                case KeyboardKey.KEY_RIGHT_SHIFT: return DoomKey.RShift;
                case KeyboardKey.KEY_LEFT_CONTROL: return DoomKey.LControl;
                case KeyboardKey.KEY_RIGHT_CONTROL: return DoomKey.RControl;
                case KeyboardKey.KEY_LEFT_ALT: return DoomKey.LAlt;
                case KeyboardKey.KEY_RIGHT_ALT: return DoomKey.RAlt;
                default: return DoomKey.Unknown;
            }
        }

        public static KeyboardKey DoomToRay(DoomKey key)
        {
            switch (key)
            {
                case DoomKey.Backspace: return KeyboardKey.KEY_BACKSPACE;
                case DoomKey.Tab: return KeyboardKey.KEY_TAB;
                case DoomKey.Enter: return KeyboardKey.KEY_ENTER;
                case DoomKey.Pause: return KeyboardKey.KEY_PAUSE;
                case DoomKey.Escape: return KeyboardKey.KEY_ESCAPE;
                case DoomKey.Space: return KeyboardKey.KEY_SPACE;
                case DoomKey.PageUp: return KeyboardKey.KEY_PAGE_UP;
                case DoomKey.PageDown: return KeyboardKey.KEY_PAGE_DOWN;
                case DoomKey.End: return KeyboardKey.KEY_END;
                case DoomKey.Home: return KeyboardKey.KEY_HOME;
                case DoomKey.Left: return KeyboardKey.KEY_LEFT;
                case DoomKey.Up: return KeyboardKey.KEY_UP;
                case DoomKey.Right: return KeyboardKey.KEY_RIGHT;
                case DoomKey.Down: return KeyboardKey.KEY_DOWN;
                case DoomKey.Insert: return KeyboardKey.KEY_INSERT;
                case DoomKey.Delete: return KeyboardKey.KEY_DELETE;
                case DoomKey.Num0: return KeyboardKey.KEY_ZERO;
                case DoomKey.Num1: return KeyboardKey.KEY_ONE;
                case DoomKey.Num2: return KeyboardKey.KEY_TWO;
                case DoomKey.Num3: return KeyboardKey.KEY_THREE;
                case DoomKey.Num4: return KeyboardKey.KEY_FOUR;
                case DoomKey.Num5: return KeyboardKey.KEY_FIVE;
                case DoomKey.Num6: return KeyboardKey.KEY_SIX;
                case DoomKey.Num7: return KeyboardKey.KEY_SEVEN;
                case DoomKey.Num8: return KeyboardKey.KEY_EIGHT;
                case DoomKey.Num9: return KeyboardKey.KEY_NINE;
                case DoomKey.A: return KeyboardKey.KEY_A;
                case DoomKey.B: return KeyboardKey.KEY_B;
                case DoomKey.C: return KeyboardKey.KEY_C;
                case DoomKey.D: return KeyboardKey.KEY_D;
                case DoomKey.E: return KeyboardKey.KEY_E;
                case DoomKey.F: return KeyboardKey.KEY_F;
                case DoomKey.G: return KeyboardKey.KEY_G;
                case DoomKey.H: return KeyboardKey.KEY_H;
                case DoomKey.I: return KeyboardKey.KEY_I;
                case DoomKey.J: return KeyboardKey.KEY_J;
                case DoomKey.K: return KeyboardKey.KEY_K;
                case DoomKey.L: return KeyboardKey.KEY_L;
                case DoomKey.M: return KeyboardKey.KEY_M;
                case DoomKey.N: return KeyboardKey.KEY_N;
                case DoomKey.O: return KeyboardKey.KEY_O;
                case DoomKey.P: return KeyboardKey.KEY_P;
                case DoomKey.Q: return KeyboardKey.KEY_Q;
                case DoomKey.R: return KeyboardKey.KEY_R;
                case DoomKey.S: return KeyboardKey.KEY_S;
                case DoomKey.T: return KeyboardKey.KEY_T;
                case DoomKey.U: return KeyboardKey.KEY_U;
                case DoomKey.V: return KeyboardKey.KEY_V;
                case DoomKey.W: return KeyboardKey.KEY_W;
                case DoomKey.X: return KeyboardKey.KEY_X;
                case DoomKey.Y: return KeyboardKey.KEY_Y;
                case DoomKey.Z: return KeyboardKey.KEY_Z;
                case DoomKey.Numpad0: return KeyboardKey.KEY_KP_0;
                case DoomKey.Numpad1: return KeyboardKey.KEY_KP_1;
                case DoomKey.Numpad2: return KeyboardKey.KEY_KP_2;
                case DoomKey.Numpad3: return KeyboardKey.KEY_KP_3;
                case DoomKey.Numpad4: return KeyboardKey.KEY_KP_4;
                case DoomKey.Numpad5: return KeyboardKey.KEY_KP_5;
                case DoomKey.Numpad6: return KeyboardKey.KEY_KP_6;
                case DoomKey.Numpad7: return KeyboardKey.KEY_KP_7;
                case DoomKey.Numpad8: return KeyboardKey.KEY_KP_8;
                case DoomKey.Numpad9: return KeyboardKey.KEY_KP_9;
                case DoomKey.Multiply: return KeyboardKey.KEY_KP_MULTIPLY;
                case DoomKey.Add: return KeyboardKey.KEY_KP_ADD;
                case DoomKey.Subtract: return KeyboardKey.KEY_KP_SUBTRACT;
                case DoomKey.Divide: return KeyboardKey.KEY_KP_DIVIDE;
                case DoomKey.F1: return KeyboardKey.KEY_F1;
                case DoomKey.F2: return KeyboardKey.KEY_F2;
                case DoomKey.F3: return KeyboardKey.KEY_F3;
                case DoomKey.F4: return KeyboardKey.KEY_F4;
                case DoomKey.F5: return KeyboardKey.KEY_F5;
                case DoomKey.F6: return KeyboardKey.KEY_F6;
                case DoomKey.F7: return KeyboardKey.KEY_F7;
                case DoomKey.F8: return KeyboardKey.KEY_F8;
                case DoomKey.F9: return KeyboardKey.KEY_F9;
                case DoomKey.F10: return KeyboardKey.KEY_F10;
                case DoomKey.F11: return KeyboardKey.KEY_F11;
                case DoomKey.F12: return KeyboardKey.KEY_F12;
                case DoomKey.LShift: return KeyboardKey.KEY_LEFT_SHIFT;
                case DoomKey.RShift: return KeyboardKey.KEY_RIGHT_SHIFT;
                case DoomKey.LControl: return KeyboardKey.KEY_LEFT_CONTROL;
                case DoomKey.RControl: return KeyboardKey.KEY_RIGHT_CONTROL;
                case DoomKey.LAlt: return KeyboardKey.KEY_LEFT_ALT;
                case DoomKey.RAlt: return KeyboardKey.KEY_RIGHT_ALT;
                default: return KeyboardKey.KEY_NULL;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown user input.");

            ReleaseMouse();
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
