using System;

namespace ManagedDoom
{
    public abstract class MenuItem
    {
        private int skullX;
        private int skullY;
        private MenuDef next;

        private MenuItem()
        {
        }

        public MenuItem(int skullX, int skullY, MenuDef next)
        {
            this.skullX = skullX;
            this.skullY = skullY;
            this.next = next;
        }

        public int SkullX => skullX;
        public int SkullY => skullY;
        public MenuDef Next => next;
    }
}
