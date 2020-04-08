using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public class Specials
    {
        private static readonly int MAXBUTTONS = 16;
        private static readonly int BUTTONTIME = 35;

        private World world;

        private Button[] buttonList;

        public Specials(World world)
        {
            this.world = world;

            buttonList = new Button[MAXBUTTONS];
            for (var i = 0; i < buttonList.Length; i++)
            {
                buttonList[i] = new Button();
            }
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

            var switchList = world.Map.Textures.SwitchList;

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
