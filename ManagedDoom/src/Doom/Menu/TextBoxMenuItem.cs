using System;
using System.Collections.Generic;
using System.Globalization;

namespace ManagedDoom
{
    public class TextBoxMenuItem : MenuItem
    {
        private int itemX;
        private int itemY;

        private IReadOnlyList<char> text;
        private TextInput edit;

        public TextBoxMenuItem(int skullX, int skullY, int itemX, int itemY)
            : base(skullX, skullY, null)
        {
            this.itemX = itemX;
            this.itemY = itemY;
        }

        public TextInput Edit(Action finished)
        {
            edit = new TextInput(
                text != null ? text : new char[0],
                cs => { },
                cs => { text = cs; edit = null; finished(); },
                () => { edit = null; });

            return edit;
        }

        public void SetText(string text)
        {
            if (text != null)
            {
                this.text = text.ToCharArray();
            }
        }

        public IReadOnlyList<char> Text
        {
            get
            {
                if (edit == null)
                {
                    return text;
                }
                else
                {
                    return edit.Text;
                }
            }
        }

        public int ItemX => itemX;
        public int ItemY => itemY;
        public bool Editing => edit != null;
    }
}
