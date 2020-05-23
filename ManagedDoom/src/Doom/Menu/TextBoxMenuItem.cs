using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public class TextBoxMenuItem : MenuItem
    {
        private IReadOnlyList<char> text;
        private int itemX;
        private int itemY;

        private TextInput edit;

        public TextBoxMenuItem(string text, int skullX, int skullY, int itemX, int itemY)
            : base(skullX, skullY, null)
        {
            this.text = text.ToCharArray();
            this.itemX = itemX;
            this.itemY = itemY;
        }

        public TextInput Edit()
        {
            edit = new TextInput(
                text,
                cs => { },
                cs => { text = cs; edit = null; },
                () => { edit = null; });

            return edit;
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
