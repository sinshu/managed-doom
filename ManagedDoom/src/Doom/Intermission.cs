using System;

namespace ManagedDoom
{
    public sealed class Intermission
    {
        // used to accelerate or skip a stage
        private int acceleratestage;

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

        private int[] cnt_kills;
        private int[] cnt_items;
        private int[] cnt_secret;
        private int cnt_time;
        private int cnt_par;
        private int cnt_pause;

        private int sp_state;

        public Intermission()
        {
            cnt_kills = new int[Player.MaxPlayerCount];
            cnt_items = new int[Player.MaxPlayerCount];
            cnt_secret = new int[Player.MaxPlayerCount];

            InitStats();
        }

        private void InitStats()
        {
            state = IntermissionState.StatCount;
            acceleratestage = 0;
            sp_state = 1;
            cnt_kills[0] = cnt_items[0] = cnt_secret[0] = -1;
            cnt_time = cnt_par = -1;
            cnt_pause = GameConstants.TicRate;

            // WI_initAnimatedBack();
        }
    }
}
