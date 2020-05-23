using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class MenuDef
    {
        DoomMenu menu;
        private string[] name;
        private int[] titleX;
        private int[] titleY;
        private MenuItem[] items;

        private int index;
        private MenuItem choice;

        public MenuDef(
            DoomMenu menu,
            string name, int titleX, int titleY,
            int firstChoice,
            params MenuItem[] items)
        {
            this.menu = menu;
            this.name = new[] { name };
            this.titleX = new[] { titleX };
            this.titleY = new[] { titleY };
            this.items = items;

            index = firstChoice;
            choice = items[index];
        }

        public MenuDef(
            DoomMenu menu,
            string name1, int titleX1, int titleY1,
            string name2, int titleX2, int titleY2,
            int firstChoice,
            params MenuItem[] items)
        {
            this.menu = menu;
            this.name = new[] { name1, name2 };
            this.titleX = new[] { titleX1, titleX2 };
            this.titleY = new[] { titleY1, titleY2 };
            this.items = items;

            index = firstChoice;
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

            if (e.Key == SFML.Window.Keyboard.Key.Left)
            {
                var slider = choice as SliderMenuItem;
                if (slider != null)
                {
                    slider.Down();
                }
            }

            if (e.Key == SFML.Window.Keyboard.Key.Right)
            {
                var slider = choice as SliderMenuItem;
                if (slider != null)
                {
                    slider.Up();
                }
            }

            if (e.Key == SFML.Window.Keyboard.Key.Enter)
            {
                if (choice.Next != null)
                {
                    menu.SetCurrent(choice.Next);
                }
            }

            if (e.Key == SFML.Window.Keyboard.Key.Escape)
            {
                menu.Close();
            }

            return true;
        }

        public IReadOnlyList<string> Name => name;
        public IReadOnlyList<int> TitleX => titleX;
        public IReadOnlyList<int> TitleY => titleY;
        public IReadOnlyList<MenuItem> Items => items;
        public MenuItem Choice => choice;
    }
}
