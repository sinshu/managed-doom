using System;

namespace ManagedDoom
{
    public sealed class TextureAnimationInfo
    {
        private bool isTexture;
        private int picNum;
        private int basePic;
        private int numPics;
        private int speed;

        public TextureAnimationInfo(bool isTexture, int picNum, int basePic, int numPics, int speed)
        {
            this.isTexture = isTexture;
            this.picNum = picNum;
            this.basePic = basePic;
            this.numPics = numPics;
            this.speed = speed;
        }

        public bool IsTexture => isTexture;
        public int PicNum => picNum;
        public int BasePic => basePic;
        public int NumPics => numPics;
        public int Speed => speed;
    }
}
