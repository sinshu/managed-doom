using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagedDoom
{
    public static class DoomInterop
    {
        public static string ToString(byte[] data, int offset, int maxLength)
        {
            var length = 0;
            for (var i = 0; i < maxLength; i++)
            {
                if (data[offset + i] == 0)
                {
                    break;
                }
                length++;
            }
            var chars = new char[length];
            for (var i = 0; i < chars.Length; i++)
            {
                var c = data[offset + i];
                if ('a' <= c && c <= 'z')
                {
                    c -= 0x20;
                }
                chars[i] = (char)c;
            }
            return new string(chars);
        }
    }
}
