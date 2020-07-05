using System;

namespace ManagedDoom
{
    public sealed class AnimationDef
    {
        private bool isTexture;
        private string endName;
        private string startName;
        private int speed;

        public AnimationDef(bool isTexture, string endName, string startName, int speed)
        {
            this.isTexture = isTexture;
            this.endName = endName;
            this.startName = startName;
            this.speed = speed;
        }

        public bool IsTexture => isTexture;
        public string EndName => endName;
        public string StartName => startName;
        public int Speed => speed;
    }
}
