using System;
using SFML.Window;

namespace ManagedDoom
{
    public static class KeyMap
    {
        public static int ToDoomKeyCode(Keyboard.Key sfmlKey)
        {
            switch (sfmlKey)
            {
                case Keyboard.Key.Right:
                    return KEY_RIGHTARROW;
                case Keyboard.Key.Left:
                    return KEY_LEFTARROW;
                case Keyboard.Key.Up:
                    return KEY_UPARROW;
                case Keyboard.Key.Down:
                    return KEY_DOWNARROW;
                case Keyboard.Key.Escape:
                    return KEY_ESCAPE;
                case Keyboard.Key.Enter:
                    return KEY_ENTER;
                case Keyboard.Key.Tab:
                    return KEY_TAB;
                case Keyboard.Key.F1:
                    return KEY_F1;
                case Keyboard.Key.F2:
                    return KEY_F2;
                case Keyboard.Key.F3:
                    return KEY_F3;
                case Keyboard.Key.F4:
                    return KEY_F4;
                case Keyboard.Key.F5:
                    return KEY_F5;
                case Keyboard.Key.F6:
                    return KEY_F6;
                case Keyboard.Key.F7:
                    return KEY_F7;
                case Keyboard.Key.F8:
                    return KEY_F8;
                case Keyboard.Key.F9:
                    return KEY_F9;
                case Keyboard.Key.F10:
                    return KEY_F10;
                case Keyboard.Key.F11:
                    return KEY_F11;
                case Keyboard.Key.F12:
                    return KEY_F12;
                case Keyboard.Key.Backspace:
                    return KEY_BACKSPACE;
                case Keyboard.Key.Pause:
                    return KEY_PAUSE;
                case Keyboard.Key.Equal:
                    return KEY_EQUALS;
                case Keyboard.Key.Hyphen:
                    return KEY_MINUS;
                case Keyboard.Key.LShift:
                case Keyboard.Key.RShift:
                    return KEY_RSHIFT;
                case Keyboard.Key.LControl:
                case Keyboard.Key.RControl:
                    return KEY_RCTRL;
                case Keyboard.Key.LAlt:
                case Keyboard.Key.RAlt:
                    return KEY_RALT;
                default:
                    if (Keyboard.Key.A <= sfmlKey && sfmlKey <= Keyboard.Key.Z)
                    {
                        return 'A' + sfmlKey - Keyboard.Key.A;
                    }
                    else if (Keyboard.Key.Num0 <= sfmlKey && sfmlKey <= Keyboard.Key.Num9)
                    {
                        return '0' + sfmlKey - Keyboard.Key.Num0;
                    }
                    else if (Keyboard.Key.Numpad0 <= sfmlKey && sfmlKey <= Keyboard.Key.Numpad9)
                    {
                        return '0' + sfmlKey - Keyboard.Key.Numpad0;
                    }
                    else
                    {
                        return ' ';
                    }
            }
        }

        //
        // DOOM keyboard definition.
        // This is the stuff configured by Setup.Exe.
        // Most key data are simple ascii (uppercased).
        //
        public static readonly int KEY_RIGHTARROW = 0xae;
        public static readonly int KEY_LEFTARROW = 0xac;
        public static readonly int KEY_UPARROW = 0xad;
        public static readonly int KEY_DOWNARROW = 0xaf;
        public static readonly int KEY_ESCAPE = 27;
        public static readonly int KEY_ENTER = 13;
        public static readonly int KEY_TAB = 9;
        public static readonly int KEY_F1 = (0x80 + 0x3b);
        public static readonly int KEY_F2 = (0x80 + 0x3c);
        public static readonly int KEY_F3 = (0x80 + 0x3d);
        public static readonly int KEY_F4 = (0x80 + 0x3e);
        public static readonly int KEY_F5 = (0x80 + 0x3f);
        public static readonly int KEY_F6 = (0x80 + 0x40);
        public static readonly int KEY_F7 = (0x80 + 0x41);
        public static readonly int KEY_F8 = (0x80 + 0x42);
        public static readonly int KEY_F9 = (0x80 + 0x43);
        public static readonly int KEY_F10 = (0x80 + 0x44);
        public static readonly int KEY_F11 = (0x80 + 0x57);
        public static readonly int KEY_F12 = (0x80 + 0x58);

        public static readonly int KEY_BACKSPACE = 127;
        public static readonly int KEY_PAUSE = 0xff;

        public static readonly int KEY_EQUALS = 0x3d;
        public static readonly int KEY_MINUS = 0x2d;

        public static readonly int KEY_RSHIFT = (0x80 + 0x36);
        public static readonly int KEY_RCTRL = (0x80 + 0x1d);
        public static readonly int KEY_RALT = (0x80 + 0x38);

        public static readonly int KEY_LALT = KEY_RALT;
    }
}
