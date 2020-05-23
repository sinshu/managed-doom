using System;

namespace ManagedDoom
{
    public class TextBoxMenuItem : MenuItem
    {
        private string text;
        private int itemX;
        private int itemY;

        public TextBoxMenuItem(string text, int skullX, int skullY, int itemX, int itemY)
            : base(skullX, skullY, null)
        {
            this.text = text;
            this.itemX = itemX;
            this.itemY = itemY;
        }

        public string Text => text;
        public int ItemX => itemX;
        public int ItemY => itemY;
    }
}
