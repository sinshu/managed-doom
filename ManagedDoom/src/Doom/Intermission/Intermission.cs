using System;

namespace ManagedDoom
{
    public sealed class Intermission
    {
        private GameOptions options;

        // used to accelerate or skip a stage
        private bool acceleratestage;

        // wbs->pnum
        private int me;

        // specifies current state
        public IntermissionState state;

        // contains information passed into intermission
        private IntermissionInfo wbs;

        private PlayerIntermissionInfo[] plrs;

        // used for general timing
        private int cnt;

        // used for timing of background animation
        private int bcnt;

        // signals to refresh everything for one frame
        private int firstrefresh;

        public int[] cnt_kills;
        public int[] cnt_items;
        public int[] cnt_secret;
        public int[] cnt_frags;
        public int cnt_time;
        public int cnt_par;
        public int cnt_pause;

        private int sp_state;

        private int ng_state;
        private bool dofrags;

        private int dm_state;
        private int[][] dm_frags;
        private int[] dm_totals;

        private bool snl_pointeron;

        private Player[] players;
        private DoomRandom random;

        private Animation[] animations;

        public Intermission(Player[] players, IntermissionInfo wbs, GameOptions options)
        {
            this.players = players;
            this.wbs = wbs;
            this.options = options;

            plrs = wbs.Plyr;

            cnt_kills = new int[Player.MaxPlayerCount];
            cnt_items = new int[Player.MaxPlayerCount];
            cnt_secret = new int[Player.MaxPlayerCount];
            cnt_frags = new int[Player.MaxPlayerCount];

            dm_frags = new int[Player.MaxPlayerCount][];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                dm_frags[i] = new int[Player.MaxPlayerCount];
            }
            dm_totals = new int[Player.MaxPlayerCount];

            animations = new Animation[AnimationInfo.Episodes[wbs.Epsd].Count];
            for (var i = 0; i < animations.Length; i++)
            {
                animations[i] = new Animation(this, AnimationInfo.Episodes[wbs.Epsd][i], i);
            }

            random = new DoomRandom();

            if (options.Deathmatch != 0)
            {
                WI_initDeathmatchStats();
            }
            else if (options.NetGame)
            {
                WI_InitNetgameStats();
            }
            else
            {
                InitStats();
            }
        }

        private void InitStats()
        {
            state = IntermissionState.StatCount;
            acceleratestage = false;
            sp_state = 1;
            cnt_kills[0] = cnt_items[0] = cnt_secret[0] = -1;
            cnt_time = cnt_par = -1;
            cnt_pause = GameConstants.TicRate;

            WI_initAnimatedBack();
        }

        private void WI_InitNetgameStats()
        {
            state = IntermissionState.StatCount;
            acceleratestage = false;
            ng_state = 1;
            cnt_pause = GameConstants.TicRate;

            var frags = 0;
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!players[i].InGame)
                {
                    continue;
                }

                cnt_kills[i] = cnt_items[i] = cnt_secret[i] = cnt_frags[i] = 0;

                frags += WI_fragSum(i);
            }

            dofrags = frags > 0;

            WI_initAnimatedBack();
        }

        private void WI_initDeathmatchStats()
        {
            state = IntermissionState.StatCount;
            acceleratestage = false;
            dm_state = 1;

            cnt_pause = GameConstants.TicRate;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame)
                {
                    for (var j = 0; j < Player.MaxPlayerCount; j++)
                    {
                        if (players[j].InGame)
                        {
                            dm_frags[i][j] = 0;
                        }
                    }

                    dm_totals[i] = 0;
                }
            }

            WI_initAnimatedBack();
        }












        private bool end = false;

        public bool Update()
        {
            // counter for general background animation
            bcnt++;

            WI_checkForAccelerate();


            switch (state)
            {
                case IntermissionState.StatCount:
                    if (options.Deathmatch != 0)
                    {
                        WI_updateDeathmatchStats();
                    }
                    else if (options.NetGame)
                    {
                        WI_UpdateNetgameStats();
                    }
                    else
                    {
                        UpdateStats();
                    }
                    break;

                case IntermissionState.ShowNextLoc:
                    WI_updateShowNextLoc();
                    break;

                case IntermissionState.NoState:
                    WI_updateNoState();
                    break;
            }

            return end;
        }

        private void UpdateStats()
        {
            WI_updateAnimatedBack();

            if (acceleratestage && sp_state != 10)
            {
                acceleratestage = false;
                cnt_kills[0] = (plrs[me].Skills * 100) / wbs.MaxKills;
                cnt_items[0] = (plrs[me].SItems * 100) / wbs.MaxItems;
                cnt_secret[0] = (plrs[me].SSecret * 100) / wbs.MaxSecret;
                cnt_time = plrs[me].STime / GameConstants.TicRate;
                cnt_par = wbs.ParTime / GameConstants.TicRate;
                StartSound(Sfx.BAREXP);
                sp_state = 10;
            }

            if (sp_state == 2)
            {
                cnt_kills[0] += 2;

                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                if (cnt_kills[0] >= (plrs[me].Skills * 100) / wbs.MaxKills)
                {
                    cnt_kills[0] = (plrs[me].Skills * 100) / wbs.MaxKills;
                    StartSound(Sfx.BAREXP);
                    sp_state++;
                }
            }
            else if (sp_state == 4)
            {
                cnt_items[0] += 2;

                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                if (cnt_items[0] >= (plrs[me].SItems * 100) / wbs.MaxItems)
                {
                    cnt_items[0] = (plrs[me].SItems * 100) / wbs.MaxItems;
                    StartSound(Sfx.BAREXP);
                    sp_state++;
                }
            }
            else if (sp_state == 6)
            {
                cnt_secret[0] += 2;

                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                if (cnt_secret[0] >= (plrs[me].SSecret * 100) / wbs.MaxSecret)
                {
                    cnt_secret[0] = (plrs[me].SSecret * 100) / wbs.MaxSecret;
                    StartSound(Sfx.BAREXP);
                    sp_state++;
                }
            }

            else if (sp_state == 8)
            {
                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                cnt_time += 3;

                if (cnt_time >= plrs[me].STime / GameConstants.TicRate)
                {
                    cnt_time = plrs[me].STime / GameConstants.TicRate;
                }

                cnt_par += 3;

                if (cnt_par >= wbs.ParTime / GameConstants.TicRate)
                {
                    cnt_par = wbs.ParTime / GameConstants.TicRate;

                    if (cnt_time >= plrs[me].STime / GameConstants.TicRate)
                    {
                        StartSound(Sfx.BAREXP);
                        sp_state++;
                    }
                }
            }
            else if (sp_state == 10)
            {
                if (acceleratestage)
                {
                    StartSound(Sfx.SGCOCK);

                    if (options.GameMode == GameMode.Commercial)
                    {
                        WI_initNoState();
                    }
                    else
                    {
                        WI_initShowNextLoc();
                    }
                }
            }
            else if ((sp_state & 1) != 0)
            {
                if (--cnt_pause == 0)
                {
                    sp_state++;
                    cnt_pause = GameConstants.TicRate;
                }
            }
        }




        private void WI_UpdateNetgameStats()
        {
            bool stillticking;

            WI_updateAnimatedBack();

            if (acceleratestage && ng_state != 10)
            {
                acceleratestage = false;

                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (!players[i].InGame)
                    {
                        continue;
                    }

                    cnt_kills[i] = (plrs[i].Skills * 100) / wbs.MaxKills;
                    cnt_items[i] = (plrs[i].SItems * 100) / wbs.MaxItems;
                    cnt_secret[i] = (plrs[i].SSecret * 100) / wbs.MaxSecret;
                }

                StartSound(Sfx.BAREXP);

                ng_state = 10;
            }

            if (ng_state == 2)
            {
                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                stillticking = false;

                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (!players[i].InGame)
                    {
                        continue;
                    }

                    cnt_kills[i] += 2;

                    if (cnt_kills[i] >= (plrs[i].Skills * 100) / wbs.MaxKills)
                    {
                        cnt_kills[i] = (plrs[i].Skills * 100) / wbs.MaxKills;
                    }
                    else
                    {
                        stillticking = true;
                    }
                }

                if (!stillticking)
                {
                    StartSound(Sfx.BAREXP);
                    ng_state++;
                }
            }
            else if (ng_state == 4)
            {
                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                stillticking = false;

                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (!players[i].InGame)
                    {
                        continue;
                    }

                    cnt_items[i] += 2;
                    if (cnt_items[i] >= (plrs[i].SItems * 100) / wbs.MaxItems)
                    {
                        cnt_items[i] = (plrs[i].SItems * 100) / wbs.MaxItems;
                    }
                    else
                    {
                        stillticking = true;
                    }
                }

                if (!stillticking)
                {
                    StartSound(Sfx.BAREXP);
                    ng_state++;
                }
            }
            else if (ng_state == 6)
            {
                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                stillticking = false;

                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (!players[i].InGame)
                    {
                        continue;
                    }

                    cnt_secret[i] += 2;

                    if (cnt_secret[i] >= (plrs[i].SSecret * 100) / wbs.MaxSecret)
                    {
                        cnt_secret[i] = (plrs[i].SSecret * 100) / wbs.MaxSecret;
                    }
                    else
                    {
                        stillticking = true;
                    }
                }

                if (!stillticking)
                {
                    StartSound(Sfx.BAREXP);
                    if (dofrags)
                    {
                        ng_state++;
                    }
                    else
                    {
                        ng_state += 3;
                    }
                }
            }
            else if (ng_state == 8)
            {
                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                stillticking = false;

                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (!players[i].InGame)
                    {
                        continue;
                    }

                    cnt_frags[i] += 1;

                    int fsum;
                    if (cnt_frags[i] >= (fsum = WI_fragSum(i)))
                    {
                        cnt_frags[i] = fsum;
                    }
                    else
                    {
                        stillticking = true;
                    }
                }

                if (!stillticking)
                {
                    StartSound(Sfx.PLDETH);
                    ng_state++;
                }
            }
            else if (ng_state == 10)
            {
                if (acceleratestage)
                {
                    StartSound(Sfx.SGCOCK);

                    if (options.GameMode == GameMode.Commercial)
                    {
                        WI_initNoState();
                    }
                    else
                    {
                        WI_initShowNextLoc();
                    }
                }
            }
            else if ((ng_state & 1) != 0)
            {
                if (--cnt_pause == 0)
                {
                    ng_state++;
                    cnt_pause = GameConstants.TicRate;
                }
            }
        }



        private void WI_updateDeathmatchStats()
        {
            bool stillticking;

            WI_updateAnimatedBack();

            if (acceleratestage && dm_state != 4)
            {
                acceleratestage = false;

                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (players[i].InGame)
                    {
                        for (var j = 0; j < Player.MaxPlayerCount; j++)
                        {
                            if (players[j].InGame)
                            {
                                dm_frags[i][j] = plrs[i].Frags[j];
                            }
                        }

                        dm_totals[i] = WI_fragSum(i);
                    }
                }

                StartSound(Sfx.BAREXP);

                dm_state = 4;
            }

            if (dm_state == 2)
            {
                if ((bcnt & 3) == 0)
                {
                    StartSound(Sfx.PISTOL);
                }

                stillticking = false;

                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (players[i].InGame)
                    {
                        for (var j = 0; j < Player.MaxPlayerCount; j++)
                        {
                            if (players[j].InGame && dm_frags[i][j] != plrs[i].Frags[j])
                            {
                                if (plrs[i].Frags[j] < 0)
                                {
                                    dm_frags[i][j]--;
                                }
                                else
                                {
                                    dm_frags[i][j]++;
                                }

                                if (dm_frags[i][j] > 99)
                                {
                                    dm_frags[i][j] = 99;
                                }

                                if (dm_frags[i][j] < -99)
                                {
                                    dm_frags[i][j] = -99;
                                }

                                stillticking = true;
                            }
                        }

                        dm_totals[i] = WI_fragSum(i);

                        if (dm_totals[i] > 99)
                        {
                            dm_totals[i] = 99;
                        }

                        if (dm_totals[i] < -99)
                        {
                            dm_totals[i] = -99;
                        }
                    }

                }

                if (!stillticking)
                {
                    StartSound(Sfx.BAREXP);
                    dm_state++;
                }

            }
            else if (dm_state == 4)
            {
                if (acceleratestage)
                {
                    StartSound(Sfx.SLOP);

                    if (options.GameMode == GameMode.Commercial)
                    {
                        WI_initNoState();
                    }
                    else
                    {
                        WI_initShowNextLoc();
                    }
                }
            }
            else if ((dm_state & 1) != 0)
            {
                if (--cnt_pause == 0)
                {
                    dm_state++;
                    cnt_pause = GameConstants.TicRate;
                }
            }
        }

        private int WI_fragSum(int playernum)
        {
            var frags = 0;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame && i != playernum)
                {
                    frags += plrs[playernum].Frags[i];
                }
            }

            frags -= plrs[playernum].Frags[playernum];

            return frags;
        }












        private void WI_updateShowNextLoc()
        {
            WI_updateAnimatedBack();

            if (--cnt == 0 || acceleratestage)
            {
                WI_initNoState();
            }
            else
            {
                snl_pointeron = (cnt & 31) < 20;
            }
        }

        private void WI_updateNoState()
        {

            WI_updateAnimatedBack();

            if (--cnt == 0)
            {
                //WI_End();
                //G_WorldDone();
                end = true;
            }
        }

        private void WI_updateAnimatedBack()
        {
            if (options.GameMode == GameMode.Commercial)
            {
                return;
            }

            if (wbs.Epsd > 2)
            {
                return;
            }

            foreach (var a in animations)
            {
                a.Update(bcnt);
            }
        }




        private void WI_initNoState()
        {
            state = IntermissionState.NoState;
            acceleratestage = false;
            cnt = 10;
        }

        private static readonly int SHOWNEXTLOCDELAY = 4;

        private void WI_initShowNextLoc()
        {
            state = IntermissionState.ShowNextLoc;
            acceleratestage = false;
            cnt = SHOWNEXTLOCDELAY * GameConstants.TicRate;

            WI_initAnimatedBack();
        }


        private void WI_checkForAccelerate()
        {
            // check for button presses to skip delays
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                var player = players[i];
                if (player.InGame)
                {
                    if ((player.Cmd.Buttons & TicCmdButtons.Attack) != 0)
                    {
                        if (!player.AttackDown)
                        {
                            acceleratestage = true;
                        }
                        player.AttackDown = true;
                    }
                    else
                    {
                        player.AttackDown = false;
                    }

                    if ((player.Cmd.Buttons & TicCmdButtons.Use) != 0)
                    {
                        if (!player.UseDown)
                        {
                            acceleratestage = true;
                        }
                        player.UseDown = true;
                    }
                    else
                    {
                        player.UseDown = false;
                    }
                }
            }
        }

        private void WI_initAnimatedBack()
        {
            if (options.GameMode == GameMode.Commercial)
            {
                return;
            }

            if (wbs.Epsd > 2)
            {
                return;
            }

            foreach (var animation in animations)
            {
                animation.Reset(bcnt);
            }
        }

        private void StartSound(Sfx sfx)
        {
            if (audio != null)
            {
                audio.StartSound(null, sfx);
            }
        }



        public IntermissionInfo Wbs => wbs;
        public GameOptions Options => options;
        public Player[] Players => players;
        public int[][] DM_Frags => dm_frags;
        public int[] DM_Totals => dm_totals;
        public bool DoFrags => dofrags;
        public DoomRandom Random => random;
        public Animation[] Animations => animations;
        public bool Snl_PointerOn => snl_pointeron;



        private SfmlAudio audio;

        public SfmlAudio Audio
        {
            get => audio;
            set => audio = value;
        }
    }
}
