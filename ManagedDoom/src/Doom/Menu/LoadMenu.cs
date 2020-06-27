using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public sealed class LoadMenu : MenuDef
    {
        private string[] name;
        private int[] titleX;
        private int[] titleY;
        private TextBoxMenuItem[] items;

        private int index;
        private TextBoxMenuItem choice;

        private TextInput textInput;

        public LoadMenu(
            DoomMenu menu,
            string name, int titleX, int titleY,
            int firstChoice,
            params TextBoxMenuItem[] items) : base(menu)
        {
            this.name = new[] { name };
            this.titleX = new[] { titleX };
            this.titleY = new[] { titleY };
            this.items = items;

            index = firstChoice;
            choice = items[index];
        }

        public override void Open()
        {
            for (var i = 0; i < items.Length; i++)
            {
                items[i].SetText(Menu.SaveSlots[i]);
            }
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

        public override bool DoEvent(DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
            {
                return true;
            }

            if (textInput != null)
            {
                var result = textInput.DoEvent(e);

                if (textInput.State == TextInputState.Canceled)
                {
                    textInput = null;
                }
                else if (textInput.State == TextInputState.Finished)
                {
                    textInput = null;
                }

                if (result)
                {
                    return true;
                }
            }

            if (e.Key == DoomKeys.Up)
            {
                Up();
            }

            if (e.Key == DoomKeys.Down)
            {
                Down();
            }

            if (e.Key == DoomKeys.Enter)
            {
                DoLoad(index);
            }

            if (e.Key == DoomKeys.Escape)
            {
                Menu.Close();
            }

            return true;
        }

        private void DoLoad(int slotNumber)
        {
            if (Menu.SaveSlots[slotNumber] != null)
            {
                Console.WriteLine("LOAD " + slotNumber + ": " + Menu.SaveSlots[slotNumber]);
            }
        }

        public IReadOnlyList<string> Name => name;
        public IReadOnlyList<int> TitleX => titleX;
        public IReadOnlyList<int> TitleY => titleY;
        public IReadOnlyList<MenuItem> Items => items;
        public MenuItem Choice => choice;
    }
}
