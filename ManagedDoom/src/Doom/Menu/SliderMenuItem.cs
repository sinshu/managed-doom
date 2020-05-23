using System;

namespace ManagedDoom
{
    public class SliderMenuItem : MenuItem
    {
        private string name;
        private int itemX;
        private int itemY;

        private int sliderLength;

        private int sliderPosition;

        public SliderMenuItem(
            string name,
            int skullX, int skullY,
            int itemX, int itemY,
            int sliderLength,
            int firstPosition)
            : base(skullX, skullY, null)
        {
            this.name = name;
            this.itemX = itemX;
            this.itemY = itemY;

            this.sliderLength = sliderLength;

            sliderPosition = firstPosition;
        }

        public void Up()
        {
            if (sliderPosition < SliderLength - 1)
            {
                sliderPosition++;
            }
        }

        public void Down()
        {
            if (sliderPosition > 0)
            {
                sliderPosition--;
            }
        }

        public string Name => name;
        public int ItemX => itemX;
        public int ItemY => itemY;

        public int SliderX => itemX;
        public int SliderY => itemY + 16;
        public int SliderLength => sliderLength;
        public int SliderPosition => sliderPosition;
    }
}
