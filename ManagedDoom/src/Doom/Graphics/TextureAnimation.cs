using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class TextureAnimation
    {
        private TextureAnimationInfo[] animations;

        public TextureAnimation(TextureLookup textures, FlatLookup flats)
        {
            var list = new List<TextureAnimationInfo>();

            foreach (var animDef in DoomInfo.TextureAnimation)
            {
                int picNum;
                int basePic;
                if (animDef.IsTexture)
                {
                    if (textures.GetNumber(animDef.StartName) == -1)
                    {
                        continue;
                    }

                    picNum = textures.GetNumber(animDef.EndName);
                    basePic = textures.GetNumber(animDef.StartName);
                }
                else
                {
                    if (flats.GetNumber(animDef.StartName) == -1)
                    {
                        continue;
                    }

                    picNum = flats.GetNumber(animDef.EndName);
                    basePic = flats.GetNumber(animDef.StartName);
                }

                var anim = new TextureAnimationInfo(
                    animDef.IsTexture,
                    picNum,
                    basePic,
                    picNum - basePic + 1,
                    animDef.Speed);

                if (anim.NumPics < 2)
                {
                    throw new Exception("Bad animation cycle from " + animDef.StartName + " to " + animDef.EndName + "!");
                }

                list.Add(anim);
            }

            animations = list.ToArray();
        }

        public TextureAnimationInfo[] Animations => animations;
    }
}
