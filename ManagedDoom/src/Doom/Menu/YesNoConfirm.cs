using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class YesNoConfirm : MenuDef
    {
        private string[] text;
        private Action action;

        public YesNoConfirm(DoomMenu menu, string text, Action action) : base(menu)
        {
            this.text = text.Split('\n');
            this.action = action;
        }

        public override bool DoEvent(DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
            {
                return true;
            }

            if (e.Key == DoomKeys.Y ||
                e.Key == DoomKeys.Enter ||
                e.Key == DoomKeys.Space)
            {
                action();
                Menu.Close();
            }

            if (e.Key == DoomKeys.N ||
                e.Key == DoomKeys.Escape)
            {
                Menu.Close();
            }

            return true;
        }

        public IReadOnlyList<string> Text => text;
    }
}
