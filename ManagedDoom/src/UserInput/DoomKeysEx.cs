//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



﻿using System;

namespace ManagedDoom
{
    public static class DoomKeysEx
    {
        public static string GetName(this DoomKeys key)
        {
            switch (key)
            {
                case DoomKeys.A:
                    return "a";
                case DoomKeys.B:
                    return "b";
                case DoomKeys.C:
                    return "c";
                case DoomKeys.D:
                    return "d";
                case DoomKeys.E:
                    return "e";
                case DoomKeys.F:
                    return "f";
                case DoomKeys.G:
                    return "g";
                case DoomKeys.H:
                    return "h";
                case DoomKeys.I:
                    return "i";
                case DoomKeys.J:
                    return "j";
                case DoomKeys.K:
                    return "k";
                case DoomKeys.L:
                    return "l";
                case DoomKeys.M:
                    return "m";
                case DoomKeys.N:
                    return "n";
                case DoomKeys.O:
                    return "o";
                case DoomKeys.P:
                    return "p";
                case DoomKeys.Q:
                    return "q";
                case DoomKeys.R:
                    return "r";
                case DoomKeys.S:
                    return "s";
                case DoomKeys.T:
                    return "t";
                case DoomKeys.U:
                    return "u";
                case DoomKeys.V:
                    return "v";
                case DoomKeys.W:
                    return "w";
                case DoomKeys.X:
                    return "x";
                case DoomKeys.Y:
                    return "y";
                case DoomKeys.Z:
                    return "z";
                case DoomKeys.Num0:
                    return "num0";
                case DoomKeys.Num1:
                    return "num1";
                case DoomKeys.Num2:
                    return "num2";
                case DoomKeys.Num3:
                    return "num3";
                case DoomKeys.Num4:
                    return "num4";
                case DoomKeys.Num5:
                    return "num5";
                case DoomKeys.Num6:
                    return "num6";
                case DoomKeys.Num7:
                    return "num7";
                case DoomKeys.Num8:
                    return "num8";
                case DoomKeys.Num9:
                    return "num9";
                case DoomKeys.Escape:
                    return "escape";
                case DoomKeys.LControl:
                    return "lcontrol";
                case DoomKeys.LShift:
                    return "lshift";
                case DoomKeys.LAlt:
                    return "lalt";
                case DoomKeys.LSystem:
                    return "lsystem";
                case DoomKeys.RControl:
                    return "rcontrol";
                case DoomKeys.RShift:
                    return "rshift";
                case DoomKeys.RAlt:
                    return "ralt";
                case DoomKeys.RSystem:
                    return "rsystem";
                case DoomKeys.Menu:
                    return "menu";
                case DoomKeys.LBracket:
                    return "lbracket";
                case DoomKeys.RBracket:
                    return "rbracket";
                case DoomKeys.Semicolon:
                    return "semicolon";
                case DoomKeys.Comma:
                    return "comma";
                case DoomKeys.Period:
                    return "period";
                case DoomKeys.Quote:
                    return "quote";
                case DoomKeys.Slash:
                    return "slash";
                case DoomKeys.Backslash:
                    return "backslash";
                case DoomKeys.Tilde:
                    return "tilde";
                case DoomKeys.Equal:
                    return "equal";
                case DoomKeys.Hyphen:
                    return "hyphen";
                case DoomKeys.Space:
                    return "space";
                case DoomKeys.Enter:
                    return "enter";
                case DoomKeys.Backspace:
                    return "backspace";
                case DoomKeys.Tab:
                    return "tab";
                case DoomKeys.PageUp:
                    return "pageup";
                case DoomKeys.PageDown:
                    return "pagedown";
                case DoomKeys.End:
                    return "end";
                case DoomKeys.Home:
                    return "home";
                case DoomKeys.Insert:
                    return "insert";
                case DoomKeys.Delete:
                    return "delete";
                case DoomKeys.Add:
                    return "add";
                case DoomKeys.Subtract:
                    return "subtract";
                case DoomKeys.Multiply:
                    return "multiply";
                case DoomKeys.Divide:
                    return "divide";
                case DoomKeys.Left:
                    return "left";
                case DoomKeys.Right:
                    return "right";
                case DoomKeys.Up:
                    return "up";
                case DoomKeys.Down:
                    return "down";
                case DoomKeys.Numpad0:
                    return "numpad0";
                case DoomKeys.Numpad1:
                    return "numpad1";
                case DoomKeys.Numpad2:
                    return "numpad2";
                case DoomKeys.Numpad3:
                    return "numpad3";
                case DoomKeys.Numpad4:
                    return "numpad4";
                case DoomKeys.Numpad5:
                    return "numpad5";
                case DoomKeys.Numpad6:
                    return "numpad6";
                case DoomKeys.Numpad7:
                    return "numpad7";
                case DoomKeys.Numpad8:
                    return "numpad8";
                case DoomKeys.Numpad9:
                    return "numpad9";
                case DoomKeys.F1:
                    return "f1";
                case DoomKeys.F2:
                    return "f2";
                case DoomKeys.F3:
                    return "f3";
                case DoomKeys.F4:
                    return "f4";
                case DoomKeys.F5:
                    return "f5";
                case DoomKeys.F6:
                    return "f6";
                case DoomKeys.F7:
                    return "f7";
                case DoomKeys.F8:
                    return "f8";
                case DoomKeys.F9:
                    return "f9";
                case DoomKeys.F10:
                    return "f10";
                case DoomKeys.F11:
                    return "f11";
                case DoomKeys.F12:
                    return "f12";
                case DoomKeys.F13:
                    return "f13";
                case DoomKeys.F14:
                    return "f14";
                case DoomKeys.F15:
                    return "f15";
                case DoomKeys.Pause:
                    return "pause";
                default:
                    return "unknown";
            }
        }

        public static char GetChar(this DoomKeys key)
        {
            switch (key)
            {
                case DoomKeys.A:
                    return 'a';
                case DoomKeys.B:
                    return 'b';
                case DoomKeys.C:
                    return 'c';
                case DoomKeys.D:
                    return 'd';
                case DoomKeys.E:
                    return 'e';
                case DoomKeys.F:
                    return 'f';
                case DoomKeys.G:
                    return 'g';
                case DoomKeys.H:
                    return 'h';
                case DoomKeys.I:
                    return 'i';
                case DoomKeys.J:
                    return 'j';
                case DoomKeys.K:
                    return 'k';
                case DoomKeys.L:
                    return 'l';
                case DoomKeys.M:
                    return 'm';
                case DoomKeys.N:
                    return 'n';
                case DoomKeys.O:
                    return 'o';
                case DoomKeys.P:
                    return 'p';
                case DoomKeys.Q:
                    return 'q';
                case DoomKeys.R:
                    return 'r';
                case DoomKeys.S:
                    return 's';
                case DoomKeys.T:
                    return 't';
                case DoomKeys.U:
                    return 'u';
                case DoomKeys.V:
                    return 'v';
                case DoomKeys.W:
                    return 'w';
                case DoomKeys.X:
                    return 'x';
                case DoomKeys.Y:
                    return 'y';
                case DoomKeys.Z:
                    return 'z';
                case DoomKeys.Num0:
                    return '0';
                case DoomKeys.Num1:
                    return '1';
                case DoomKeys.Num2:
                    return '2';
                case DoomKeys.Num3:
                    return '3';
                case DoomKeys.Num4:
                    return '4';
                case DoomKeys.Num5:
                    return '5';
                case DoomKeys.Num6:
                    return '6';
                case DoomKeys.Num7:
                    return '7';
                case DoomKeys.Num8:
                    return '8';
                case DoomKeys.Num9:
                    return '9';
                case DoomKeys.LBracket:
                    return '[';
                case DoomKeys.RBracket:
                    return ']';
                case DoomKeys.Semicolon:
                    return ';';
                case DoomKeys.Comma:
                    return ',';
                case DoomKeys.Period:
                    return '.';
                case DoomKeys.Quote:
                    return '"';
                case DoomKeys.Slash:
                    return '/';
                case DoomKeys.Backslash:
                    return '\\';
                case DoomKeys.Equal:
                    return '=';
                case DoomKeys.Hyphen:
                    return '-';
                case DoomKeys.Space:
                    return ' ';
                case DoomKeys.Add:
                    return '+';
                case DoomKeys.Subtract:
                    return '-';
                case DoomKeys.Multiply:
                    return '*';
                case DoomKeys.Divide:
                    return '/';
                case DoomKeys.Numpad0:
                    return '0';
                case DoomKeys.Numpad1:
                    return '1';
                case DoomKeys.Numpad2:
                    return '2';
                case DoomKeys.Numpad3:
                    return '3';
                case DoomKeys.Numpad4:
                    return '4';
                case DoomKeys.Numpad5:
                    return '5';
                case DoomKeys.Numpad6:
                    return '6';
                case DoomKeys.Numpad7:
                    return '7';
                case DoomKeys.Numpad8:
                    return '8';
                case DoomKeys.Numpad9:
                    return '9';
                default:
                    return '\0';
            }
        }
    }
}
