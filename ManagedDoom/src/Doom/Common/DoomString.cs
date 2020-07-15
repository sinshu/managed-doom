using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class DoomString
    {
        private static Dictionary<string, DoomString> table = new Dictionary<string, DoomString>();

        private string original;
        private string replaced;

        public DoomString(string original)
        {
            this.original = original;
            replaced = original;

            if (!table.ContainsKey(original))
            {
                table.Add(original, this);
            }
        }

        public override string ToString()
        {
            return replaced;
        }

        public char this[int index]
        {
            get
            {
                return replaced[index];
            }
        }

        public static implicit operator string(DoomString ds)
        {
            return ds.replaced;
        }

        public static void Replace(string original, string replaced)
        {
            DoomString ds;
            if (table.TryGetValue(original, out ds))
            {
                ds.replaced = replaced;
            }
        }
    }
}
