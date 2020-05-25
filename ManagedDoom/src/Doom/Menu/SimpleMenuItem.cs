using System;

namespace ManagedDoom
{
    public class SimpleMenuItem : MenuItem
    {
        private string name;
        private int itemX;
        private int itemY;
        private Action action;

        public SimpleMenuItem(string name, int skullX, int skullY, int itemX, int itemY, Action action, MenuDef next)
            : base(skullX, skullY, next)
        {
            this.name = name;
            this.itemX = itemX;
            this.itemY = itemY;
            this.action = action;
        }

        public string Name => name;
        public int ItemX => itemX;
        public int ItemY => itemY;
        public Action Action => action;
    }
}
