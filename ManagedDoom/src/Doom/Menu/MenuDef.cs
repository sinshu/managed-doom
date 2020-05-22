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

        private int index;
        private MenuItem choice;

        public MenuDef(string name, int titleX, int titleY, params MenuItem[] items)
        {
            this.name = name;
            this.titleX = titleX;
            this.titleY = titleY;
            this.items = items;

            index = 0;
            choice = items[index];
        }

        private void Up()
        {
            index--;
            if (index < 0)
            {
                index = items.Length - 1;
            }

            choice = items[index];
        }

        private void Down()
        {
            index++;
            if (index >= items.Length)
            {
                index = 0;
            }

            choice = items[index];
        }

        public bool DoEvent(DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
            {
                return true;
            }

            if (e.Key == SFML.Window.Keyboard.Key.Up)
            {
                Up();
            }

            if (e.Key == SFML.Window.Keyboard.Key.Down)
            {
                Down();
            }

            return true;
        }

        public string Name => name;
        public int TitleX => titleX;
        public int TitleY => titleY;
        public IReadOnlyList<MenuItem> Items => items;
        public MenuItem Choice => choice;
    }
}
