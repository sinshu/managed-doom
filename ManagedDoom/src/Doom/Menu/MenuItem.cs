using System;

namespace ManagedDoom
{
    public abstract class MenuItem
    {
        private int skullX;
        private int skullY;

        private MenuItem()
        {
        }

        public MenuItem(int skullX, int skullY)
        {
            this.skullX = skullX;
            this.skullY = skullY;
        }

        public int SkullX => skullX;
        public int SkullY => skullY;
    }
}
