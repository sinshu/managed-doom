using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public class Specials
    {
        private static readonly int maxButtonCount = 32;
        private static readonly int buttonTime = 35;

        private World world;

        private bool levelTimer;
        private int levelTimeCount;

        private Button[] buttonList;

        private int[] textureTranslation;
        private int[] flatTranslation;

        private LineDef[] scrollLines;

        public Specials(World world)
        {
            this.world = world;

            levelTimer = false;

            buttonList = new Button[maxButtonCount];
            for (var i = 0; i < buttonList.Length; i++)
            {
                buttonList[i] = new Button();
            }

            textureTranslation = new int[world.Map.Textures.Count];
            for (var i = 0; i < textureTranslation.Length; i++)
            {
                textureTranslation[i] = i;
            }

            flatTranslation = new int[world.Map.Flats.Count];
            for (var i = 0; i < flatTranslation.Length; i++)
            {
                flatTranslation[i] = i;
            }
        }

        /// <summary>
        /// After the map has been loaded, scan for specials that spawn thinkers.
        /// </summary>
        public void SpawnSpecials(int levelTimeCount)
        {
            levelTimer = true;
            this.levelTimeCount = levelTimeCount;
            SpawnSpecials();
        }

        /// <summary>
        /// After the map has been loaded, scan for specials that spawn thinkers.
        /// </summary>
        public void SpawnSpecials()
        {
            // Init special sectors.
            var lc = world.LightingChange;
            var sa = world.SectorAction;
            foreach (var sector in world.Map.Sectors)
            {
                if (sector.Special == 0)
                {
                    continue;
                }

                switch ((int)sector.Special)
                {
                    case 1:
                        // Flickering lights.
                        lc.SpawnLightFlash(sector);
                        break;

                    case 2:
                        // Strobe fast.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, 0);
                        break;

                    case 3:
                        // Strobe slow.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.SlowDark, 0);
                        break;

                    case 4:
                        // Strobe fast / death slime.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, 0);
                        sector.Special = (SectorSpecial)4;
                        break;

                    case 8:
                        // Glowing light.
                        lc.SpawnGlowingLight(sector);
                        break;
                    case 9:
                        // Secret sector.
                        world.totalSecrets++;
                        break;

                    case 10:
                        // Door close in 30 seconds.
                        sa.SpawnDoorCloseIn30(sector);
                        break;

                    case 12:
                        // Sync strobe slow.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.SlowDark, 1);
                        break;

                    case 13:
                        // Sync strobe fast.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, 1);
                        break;

                    case 14:
                        // Door raise in 5 minutes.
                        sa.SpawnDoorRaiseIn5Mins(sector);
                        break;

                    case 17:
                        lc.SpawnFireFlicker(sector);
                        break;
                }
            }

            var scrollList = new List<LineDef>();
            foreach (var line in world.Map.Lines)
            {
                switch ((int)line.Special)
                {
                    case 48:
                        // Texture scroll.
                        scrollList.Add(line);
                        break;
                }
            }
            scrollLines = scrollList.ToArray();
        }

        public void ChangeSwitchTexture(LineDef line, bool useAgain)
        {
            if (!useAgain)
            {
                line.Special = 0;
            }

            var frontSide = line.Side0;
            var topTexture = frontSide.TopTexture;
            var middleTexture = frontSide.MiddleTexture;
            var bottomTexture = frontSide.BottomTexture;

            var sound = Sfx.SWTCHN;

            // Exit switch?
            if ((int)line.Special == 11)
            {
                sound = Sfx.SWTCHX;
            }

            var switchList = world.Map.Textures.SwitchList;

            for (var i = 0; i < switchList.Length; i++)
            {
                if (switchList[i] == topTexture)
                {
                    world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                    frontSide.TopTexture = switchList[i ^ 1];

                    if (useAgain)
                    {
                        StartButton(line, ButtonPosition.Top, switchList[i], buttonTime);
                    }

                    return;
                }
                else
                {
                    if (switchList[i] == middleTexture)
                    {
                        world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                        frontSide.MiddleTexture = switchList[i ^ 1];

                        if (useAgain)
                        {
                            StartButton(line, ButtonPosition.Middle, switchList[i], buttonTime);
                        }

                        return;
                    }
                    else
                    {
                        if (switchList[i] == bottomTexture)
                        {
                            world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                            frontSide.BottomTexture = switchList[i ^ 1];

                            if (useAgain)
                            {
                                StartButton(line, ButtonPosition.Bottom, switchList[i], buttonTime);
                            }

                            return;
                        }
                    }
                }
            }
        }

        private void StartButton(LineDef line, ButtonPosition w, int texture, int time)
        {
            // See if button is already pressed.
            for (var i = 0; i < maxButtonCount; i++)
            {
                if (buttonList[i].Timer != 0 && buttonList[i].Line == line)
                {
                    return;
                }
            }

            for (var i = 0; i < maxButtonCount; i++)
            {
                if (buttonList[i].Timer == 0)
                {
                    buttonList[i].Line = line;
                    buttonList[i].Position = w;
                    buttonList[i].Texture = texture;
                    buttonList[i].Timer = time;
                    buttonList[i].SoundOrigin = line.SoundOrigin;
                    return;
                }
            }

            throw new Exception("No button slots left!");
        }

        /// <summary>
        /// Animate planes, scroll walls, etc.
        /// </summary>
        public void Update()
        {
            // Level timer.
            if (levelTimer)
            {
                levelTimeCount--;
                if (levelTimeCount == 0)
                {
                    world.G_ExitLevel();
                }
            }

            // Animate flats and textures globally.
            var animations = world.Map.Animation.Animations;
            for (var k = 0; k < animations.Length; k++)
            {
                var anim = animations[k];
                for (var i = anim.BasePic; i < anim.BasePic + anim.NumPics; i++)
                {
                    var pic = anim.BasePic + ((world.levelTime / anim.Speed + i) % anim.NumPics);
                    if (anim.IsTexture)
                    {
                        textureTranslation[i] = pic;
                    }
                    else
                    {
                        flatTranslation[i] = pic;
                    }
                }
            }

            // Animate line specials.
            foreach (var line in scrollLines)
            {
                line.Side0.TextureOffset += Fixed.One;
            }

            // Do buttons.
            for (var i = 0; i < maxButtonCount; i++)
            {
                if (buttonList[i].Timer > 0)
                {
                    buttonList[i].Timer--;

                    if (buttonList[i].Timer == 0)
                    {
                        switch (buttonList[i].Position)
                        {
                            case ButtonPosition.Top:
                                buttonList[i].Line.Side0.TopTexture = buttonList[i].Texture;
                                break;

                            case ButtonPosition.Middle:
                                buttonList[i].Line.Side0.MiddleTexture = buttonList[i].Texture;
                                break;

                            case ButtonPosition.Bottom:
                                buttonList[i].Line.Side0.BottomTexture = buttonList[i].Texture;
                                break;
                        }

                        world.StartSound(buttonList[i].SoundOrigin, Sfx.SWTCHN, SfxType.Misc, 50);
                        buttonList[i].Clear();
                    }
                }
            }
        }

        public int[] TextureTranslation => textureTranslation;
        public int[] FlatTranslation => flatTranslation;
    }
}
