using System;

namespace ManagedDoom
{
    public sealed class SpriteFrame
    {
        private bool rotate;
        private Patch[] patches;
        private bool[] flip;

        public SpriteFrame(bool rotate, Patch[] patches, bool[] flip)
        {
            this.rotate = rotate;
            this.patches = patches;
            this.flip = flip;
        }

        public bool Rotate => rotate;
        public Patch[] Patches => patches;
        public bool[] Flip => flip;
    }
}
