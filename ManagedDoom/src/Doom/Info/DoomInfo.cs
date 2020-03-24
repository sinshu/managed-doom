using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        // This is to ensure the static members initialized.
        public static void Initialize()
        {
            SpriteNames[0].Equals(null);
        }
    }
}
