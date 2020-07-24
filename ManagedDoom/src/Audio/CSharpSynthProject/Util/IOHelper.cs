namespace AudioSynthesis.Util
{
    using System.Text;
    using System.IO;
    using System;
    /// <summary>
    /// Provides methods for reading and writing data in little endian form.
    /// </summary>
    public class IOHelper
    {
        public static char Read8BitChar(BinaryReader reader)
        {
            return (char)reader.ReadByte();
        }
        public static char[] Read8BitChars(BinaryReader reader, int length)
        {
            char[] chars = new char[length];
            for (int x = 0; x < chars.Length; x++)
                chars[x] = (char)reader.ReadByte();
            return chars;
        }
        public static string Read8BitString(BinaryReader reader)
        {
            StringBuilder sbuild = new StringBuilder();
            char c = (char)reader.ReadByte();
            while (c != '\0')
            {
                sbuild.Append(c);
                c = (char)reader.ReadByte();
            }
            return sbuild.ToString();
        }
        public static string Read8BitString(BinaryReader reader, int length)
        {
            char[] chars = new char[length];
            for (int x = 0; x < chars.Length; x++)
                chars[x] = (char)reader.ReadByte();
            string s = new string(chars);
            int i = s.IndexOf('\0');
            if (i >= 0)
                return s.Remove(i);
            return s;
        }
        public static short ReadInt16(byte[] input, int index)
        {
            if (BitConverter.IsLittleEndian)
                return (short)(input[index] | (input[index + 1] << 8));
            return (short)((input[index] << 8) | input[index + 1]);
        }
        public static int ReadInt24(byte[] input, int index)
        {
            int i;
            if (BitConverter.IsLittleEndian)
            {
                i = input[index] | (input[index + 1] << 8) | (input[index + 2] << 16);
                if ((i & 0x800000) == 0x800000)
                    i = i | (0xFF << 24);
            }
            else
            {
                i = (input[index] << 16) | (input[index + 1] << 8) | input[index + 2];
                if ((i & 0x100) == 0x100)
                    i = i | 0xFF;
            }
            return i;
        }
        public static int ReadInt32(byte[] input, int index)
        {
            if (BitConverter.IsLittleEndian)
                return input[index] | (input[index + 1] << 8) | (input[index + 2] << 16) | (input[index + 3] << 24);
            return (input[index] << 24) | (input[index + 1] << 16) | (input[index + 2] << 8) | input[index + 3];
        }
        public static void WriteInt16(short value, byte[] output, int index)
        {
            uint uvalue = (uint)value;
            if (BitConverter.IsLittleEndian)
            {
                output[index] = (byte)(uvalue & 0xFF);
                output[index + 1] = (byte)(uvalue >> 8);
            }
            else
            {
                output[index] = (byte)(uvalue >> 8);
                output[index + 1] = (byte)(0xFF & uvalue);
            }
        }
        public static void WriteInt24(int value, byte[] output, int index)
        {
            uint uvalue = (uint)value;
            if (BitConverter.IsLittleEndian)
            {
                output[index] = (byte)(uvalue & 0xFF);
                output[index + 1] = (byte)((uvalue >> 8) & 0xFF);
                output[index + 2] = (byte)(uvalue >> 16);
            }
            else
            {
                output[index] = (byte)(uvalue >> 16);
                output[index + 1] = (byte)((uvalue >> 8) & 0xFF);
                output[index + 2] = (byte)(uvalue & 0xFF);
            }
        }
        public static void WriteInt32(int value, byte[] output, int index)
        {
            uint uvalue = (uint)value;
            if (BitConverter.IsLittleEndian)
            {
                output[index] = (byte)(uvalue & 0xFF);
                output[index + 1] = (byte)((uvalue >> 8) & 0xFF);
                output[index + 2] = (byte)((uvalue >> 16) & 0xFF);
                output[index + 3] = (byte)(uvalue >> 24);
            }
            else
            {
                output[index] = (byte)(uvalue >> 24);
                output[index + 1] = (byte)((uvalue >> 16) & 0xFF);
                output[index + 2] = (byte)((uvalue >> 8) & 0xFF);
                output[index + 3] = (byte)(uvalue & 0xFF);
            }
        }
        public static void Write8BitString(BinaryWriter writer, string str, int length)
        {
            if (str.Length < length)
            {
                int x;
                for (x = 0; x < str.Length; x++)
                    writer.Write((byte)str[x]);
                x = length - str.Length;
                while (x > 0)
                {
                    writer.Write((byte)'\0');
                    x--;
                }
            }
            else
            {
                for (int x = 0; x < length; x++)
                    writer.Write((byte)str[x]);
            }            
        }
    }
}
