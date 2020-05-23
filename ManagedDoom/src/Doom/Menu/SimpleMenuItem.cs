using System;

namespace ManagedDoom
{
    public class SimpleMenuItem : MenuItem
    {
        private string name;
        private int itemX;
        private int itemY;

        public SimpleMenuItem(string name, int skullX, int skullY, int itemX, int itemY, MenuDef next)
            : base(skullX, skullY, next)
        {
            this.name = name;
            this.itemX = itemX;
            this.itemY = itemY;
        }

        public string Name => name;
        public int ItemX => itemX;
        public int ItemY => itemY;
    }
}
