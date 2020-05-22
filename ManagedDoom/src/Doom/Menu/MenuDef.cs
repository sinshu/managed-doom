using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class MenuDef
    {
        private string name;
        private int titleX;
        private int titleY;
        private MenuItem[] items;

        public MenuDef(string name, int titleX, int titleY, params MenuItem[] items)
        {
            this.name = name;
            this.titleX = titleX;
            this.titleY = titleY;
            this.items = items;
        }

        public string Name => name;
        public int TitleX => titleX;
        public int TitleY => titleY;
        public IReadOnlyList<MenuItem> Items => items;
    }
}
