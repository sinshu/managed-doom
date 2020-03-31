using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public class Specials
    {
        private static readonly int MAXSWITCHES = 50;
        private static readonly int MAXBUTTONS = 16;
        private static readonly int BUTTONTIME = 35;

        private static readonly Tuple<DoomString, DoomString>[] switchTextureList = new Tuple<DoomString, DoomString>[]
        {
            Tuple.Create(new DoomString("SW1BRCOM"), new DoomString("SW2BRCOM")),
            Tuple.Create(new DoomString("SW1BRN1"), new DoomString("SW2BRN1")),
            Tuple.Create(new DoomString("SW1BRN2"), new DoomString("SW2BRN2")),
            Tuple.Create(new DoomString("SW1BRNGN"), new DoomString("SW2BRNGN")),
            Tuple.Create(new DoomString("SW1BROWN"), new DoomString("SW2BROWN")),
            Tuple.Create(new DoomString("SW1COMM"), new DoomString("SW2COMM")),
            Tuple.Create(new DoomString("SW1COMP"), new DoomString("SW2COMP")),
            Tuple.Create(new DoomString("SW1DIRT"), new DoomString("SW2DIRT")),
            Tuple.Create(new DoomString("SW1EXIT"), new DoomString("SW2EXIT")),
            Tuple.Create(new DoomString("SW1GRAY"), new DoomString("SW2GRAY")),
            Tuple.Create(new DoomString("SW1GRAY1"), new DoomString("SW2GRAY1")),
            Tuple.Create(new DoomString("SW1METAL"), new DoomString("SW2METAL")),
            Tuple.Create(new DoomString("SW1PIPE"), new DoomString("SW2PIPE")),
            Tuple.Create(new DoomString("SW1SLAD"), new DoomString("SW2SLAD")),
            Tuple.Create(new DoomString("SW1STARG"), new DoomString("SW2STARG")),
            Tuple.Create(new DoomString("SW1STON1"), new DoomString("SW2STON1")),
            Tuple.Create(new DoomString("SW1STON2"), new DoomString("SW2STON2")),
            Tuple.Create(new DoomString("SW1STONE"), new DoomString("SW2STONE")),
            Tuple.Create(new DoomString("SW1STRTN"), new DoomString("SW2STRTN")),
            Tuple.Create(new DoomString("SW1BLUE"), new DoomString("SW2BLUE")),
            Tuple.Create(new DoomString("SW1CMT"), new DoomString("SW2CMT")),
            Tuple.Create(new DoomString("SW1GARG"), new DoomString("SW2GARG")),
            Tuple.Create(new DoomString("SW1GSTON"), new DoomString("SW2GSTON")),
            Tuple.Create(new DoomString("SW1HOT"), new DoomString("SW2HOT")),
            Tuple.Create(new DoomString("SW1LION"), new DoomString("SW2LION")),
            Tuple.Create(new DoomString("SW1SATYR"), new DoomString("SW2SATYR")),
            Tuple.Create(new DoomString("SW1SKIN"), new DoomString("SW2SKIN")),
            Tuple.Create(new DoomString("SW1VINE"), new DoomString("SW2VINE")),
            Tuple.Create(new DoomString("SW1WOOD"), new DoomString("SW2WOOD")),
            Tuple.Create(new DoomString("SW1PANEL"), new DoomString("SW2PANEL")),
            Tuple.Create(new DoomString("SW1ROCK"), new DoomString("SW2ROCK")),
            Tuple.Create(new DoomString("SW1MET2"), new DoomString("SW2MET2")),
            Tuple.Create(new DoomString("SW1WDMET"), new DoomString("SW2WDMET")),
            Tuple.Create(new DoomString("SW1BRIK"), new DoomString("SW2BRIK")),
            Tuple.Create(new DoomString("SW1MOD1"), new DoomString("SW2MOD1")),
            Tuple.Create(new DoomString("SW1ZIM"), new DoomString("SW2ZIM")),
            Tuple.Create(new DoomString("SW1STON6"), new DoomString("SW2STON6")),
            Tuple.Create(new DoomString("SW1TEK"), new DoomString("SW2TEK")),
            Tuple.Create(new DoomString("SW1MARB"), new DoomString("SW2MARB")),
            Tuple.Create(new DoomString("SW1SKULL"), new DoomString("SW2SKULL"))
        };

        private World world;

        private Button[] buttonList;

        private int[] switchList;

        public Specials(World world)
        {
            this.world = world;

            buttonList = new Button[MAXBUTTONS];
            for (var i = 0; i < buttonList.Length; i++)
            {
                buttonList[i] = new Button();
            }

            InitSwitchList();
        }

        private void InitSwitchList()
        {
            var textures = world.Map.Textures;
            var list = new List<int>();
            foreach (var tuple in switchTextureList)
            {
                var texNum1 = textures.GetNumber(tuple.Item1);
                var texNum2 = textures.GetNumber(tuple.Item2);
                if (texNum1 != -1 && texNum2 != -1)
                {
                    list.Add(texNum1);
                    list.Add(texNum2);
                }
            }
            switchList = list.ToArray();
        }

        public void ChangeSwitchTexture(LineDef line, bool useAgain)
        {
            if (!useAgain)
            {
                line.Special = 0;
            }

            var texTop = line.Side0.TopTexture;
            var texMid = line.Side0.MiddleTexture;
            var texBot = line.Side0.BottomTexture;

            var sound = Sfx.SWTCHN;

            // EXIT SWITCH?
            if ((int)line.Special == 11)
            {
                sound = Sfx.SWTCHX;
            }

            for (var i = 0; i < switchList.Length; i++)
            {
                if (switchList[i] == texTop)
                {
                    world.StartSound(line.SoundOrigin, sound);
                    line.Side0.TopTexture = switchList[i ^ 1];

                    if (useAgain)
                    {
                        StartButton(line, ButtonWhere.Top, switchList[i], BUTTONTIME);
                    }

                    return;
                }
                else
                {
                    if (switchList[i] == texMid)
                    {
                        world.StartSound(line.SoundOrigin, sound);
                        line.Side0.MiddleTexture = switchList[i ^ 1];

                        if (useAgain)
                        {
                            StartButton(line, ButtonWhere.Middle, switchList[i], BUTTONTIME);
                        }

                        return;
                    }
                    else
                    {
                        if (switchList[i] == texBot)
                        {
                            world.StartSound(line.SoundOrigin, sound);
                            line.Side0.BottomTexture = switchList[i ^ 1];

                            if (useAgain)
                            {
                                StartButton(line, ButtonWhere.Bottom, switchList[i], BUTTONTIME);
                            }

                            return;
                        }
                    }
                }
            }
        }

        private void StartButton(LineDef line, ButtonWhere w, int texture, int time)
        {
            // See if button is already pressed
            for (var i = 0; i < MAXBUTTONS; i++)
            {
                if (buttonList[i].Timer != 0 && buttonList[i].Line == line)
                {
                    return;
                }
            }

            for (var i = 0; i < MAXBUTTONS; i++)
            {
                if (buttonList[i].Timer == 0)
                {
                    buttonList[i].Line = line;
                    buttonList[i].Where = w;
                    buttonList[i].Texture = texture;
                    buttonList[i].Timer = time;
                    buttonList[i].SoundOrigin = line.SoundOrigin;
                    return;
                }
            }

            throw new Exception("P_StartButton: no button slots left!");
        }



        //
        // P_UpdateSpecials
        // Animate planes, scroll walls, etc.
        //
        private bool levelTimer;
        private int levelTimeCount;

        public void Update()
        {
            /*
            anim_t* anim;
            int pic;
            int i;
            line_t* line;
            */

            /*
            //	LEVEL TIMER
            if (levelTimer == true)
            {
                levelTimeCount--;
                if (!levelTimeCount)
                    G_ExitLevel();
            }
            */

            /*
            //	ANIMATE FLATS AND TEXTURES GLOBALLY
            for (anim = anims; anim < lastanim; anim++)
            {
                for (i = anim->basepic; i < anim->basepic + anim->numpics; i++)
                {
                    pic = anim->basepic + ((leveltime / anim->speed + i) % anim->numpics);
                    if (anim->istexture)
                        texturetranslation[i] = pic;
                    else
                        flattranslation[i] = pic;
                }
            }
            */

            /*
            //	ANIMATE LINE SPECIALS
            for (i = 0; i < numlinespecials; i++)
            {
                line = linespeciallist[i];
                switch (line->special)
                {
                    case 48:
                        // EFFECT FIRSTCOL SCROLL +
                        sides[line->sidenum[0]].textureoffset += FRACUNIT;
                        break;
                }
            }
            */

            //	DO BUTTONS
            for (var i = 0; i < MAXBUTTONS; i++)
            {
                if (buttonList[i].Timer > 0)
                {
                    buttonList[i].Timer--;
                    if (buttonList[i].Timer == 0)
                    {
                        switch (buttonList[i].Where)
                        {
                            case ButtonWhere.Top:
                                buttonList[i].Line.Side0.TopTexture = buttonList[i].Texture;
                                break;

                            case ButtonWhere.Middle:
                                buttonList[i].Line.Side0.MiddleTexture = buttonList[i].Texture;
                                break;

                            case ButtonWhere.Bottom:
                                buttonList[i].Line.Side0.BottomTexture = buttonList[i].Texture;
                                break;
                        }
                        world.StartSound(buttonList[i].SoundOrigin, Sfx.SWTCHN);
                        buttonList[i].Clear();
                    }
                }
            }
        }
    }
}
