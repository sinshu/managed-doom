using System;
using System.Collections.Generic;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class MenuRenderer
    {
        private static readonly char[] cursor = { '_' };

        private Wad wad;
        private DrawScreen screen;

        private Dictionary<string, Patch> patches;

        public MenuRenderer(Wad wad, DrawScreen screen)
        {
            this.wad = wad;
            this.screen = screen;

            patches = new Dictionary<string, Patch>();
        }

        public void Render(DoomMenu menu)
        {
            var current = menu.Current;
            for (var i = 0; i < current.Name.Count; i++)
            {
                DrawMenuPatch(current.Name[i], current.TitleX[i], current.TitleY[i]);
            }

            foreach (var item in current.Items)
            {
                DrawMenuItem(menu, item);
            }

            var choice = current.Choice;
            var skull = menu.Tics / 8 % 2 == 0 ? "M_SKULL1" : "M_SKULL2";
            DrawMenuPatch(skull, choice.SkullX, choice.SkullY);
        }

        private void DrawMenuItem(DoomMenu menu, MenuItem item)
        {
            var simple = item as SimpleMenuItem;
            if (simple != null)
            {
                DrawSimpleMenuItem(simple);
            }

            var toggle = item as ToggleMenuItem;
            if (toggle != null)
            {
                DrawToggleMenuItem(toggle);
            }

            var slider = item as SliderMenuItem;
            if (slider != null)
            {
                DrawSliderMenuItem(slider);
            }

            var textBox = item as TextBoxMenuItem;
            if (textBox != null)
            {
                DrawTextBoxMenuItem(textBox, menu.Tics);
            }
        }

        private void DrawMenuPatch(string name, int x, int y)
        {
            Patch patch;
            if (!patches.TryGetValue(name, out patch))
            {
                Console.WriteLine("Patch loaded: " + name);
                patch = Patch.FromWad(name, wad);
                patches.Add(name, patch);
            }

            var scale = screen.Width / 320;
            screen.DrawPatch(patch, scale * x, scale * y, scale);
        }

        private void DrawMenuText(IReadOnlyList<char> text, int x, int y)
        {
            var scale = screen.Width / 320;
            screen.DrawText(text, scale * x, scale * y, scale);
        }

        private void DrawSimpleMenuItem(SimpleMenuItem item)
        {
            DrawMenuPatch(item.Name, item.ItemX, item.ItemY);
        }

        private void DrawToggleMenuItem(ToggleMenuItem item)
        {
            DrawMenuPatch(item.Name, item.ItemX, item.ItemY);
            DrawMenuPatch(item.State, item.StateX, item.ItemY);
        }

        private void DrawSliderMenuItem(SliderMenuItem item)
        {
            DrawMenuPatch(item.Name, item.ItemX, item.ItemY);

            DrawMenuPatch("M_THERML", item.SliderX, item.SliderY);
            for (var i = 0; i < item.SliderLength; i++)
            {
                var x = item.SliderX + 8 * (1 + i);
                DrawMenuPatch("M_THERMM", x, item.SliderY);
            }

            var end = item.SliderX + 8 * (1 + item.SliderLength);
            DrawMenuPatch("M_THERMR", end, item.SliderY);

            var pos = item.SliderX + 8 * (1 + item.SliderPosition);
            DrawMenuPatch("M_THERMO", pos, item.SliderY);
        }

        private void DrawTextBoxMenuItem(TextBoxMenuItem item, int tics)
        {
            var length = 24;
            DrawMenuPatch("M_LSLEFT", item.ItemX, item.ItemY);
            for (var i = 0; i < length; i++)
            {
                var x = item.ItemX + 8 * (1 + i);
                DrawMenuPatch("M_LSCNTR", x, item.ItemY);
            }
            DrawMenuPatch("M_LSRGHT", item.ItemX + 8 * (1 + length), item.ItemY);

            DrawMenuText(item.Text, item.ItemX + 8, item.ItemY);

            if (item.Editing && tics / 3 % 2 == 0)
            {
                var textWidth = screen.MeasureText(item.Text, 1);
                DrawMenuText(cursor, item.ItemX + 8 + textWidth, item.ItemY);
            }
        }
    }
}
