using System;

namespace ManagedDoom
{
    public sealed class DoomEvent
    {
        public EventType Type;
        public int Data1;
        public int Data2;
        public int Data3;

        public SFML.Window.Keyboard.Key Key;
    }
}
