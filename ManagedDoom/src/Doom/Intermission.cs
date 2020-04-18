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
        private IntermissionState state;

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
        public int cnt_time;
        public int cnt_par;
        public int cnt_pause;

        private int sp_state;

        public Intermission(IntermissionInfo wbs, GameOptions options)
        {
            this.wbs = wbs;
            this.options = options;

            plrs = wbs.Plyr;

            cnt_kills = new int[Player.MaxPlayerCount];
            cnt_items = new int[Player.MaxPlayerCount];
            cnt_secret = new int[Player.MaxPlayerCount];

            InitStats();
        }

        private void InitStats()
        {
            state = IntermissionState.StatCount;
            acceleratestage = false;
            sp_state = 1;
            cnt_kills[0] = cnt_items[0] = cnt_secret[0] = -1;
            cnt_time = cnt_par = -1;
            cnt_pause = GameConstants.TicRate;

            // WI_initAnimatedBack();
        }

        public void Update()
        {
            // counter for general background animation
            bcnt++;

            UpdateStats();
        }

        private void UpdateStats()
        {
            //WI_updateAnimatedBack();

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

            //WI_initAnimatedBack();
        }




        private void StartSound(Sfx sfx)
        {
            if (audio != null)
            {
                audio.StartSound(null, sfx);
            }
        }



        public IntermissionInfo Wbs => wbs;









        private SfmlAudio audio;

        public SfmlAudio Audio
        {
            get => audio;
            set => audio = value;
        }
    }
}
