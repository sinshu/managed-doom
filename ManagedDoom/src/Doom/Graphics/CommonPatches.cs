using System;

namespace ManagedDoom
{
    public sealed class CommonPatches
    {
        // background (map of levels).
        private Patch background;

        // You Are Here graphic
        private Patch youAreHere1;
        private Patch youAreHere2;

        // splat
        private Patch splat;

        // %, : graphics
        private Patch percent;
        private Patch colon;

        // 0-9 graphic
        private Patch[] numbers;

        // minus sign
        private Patch wiminus;

        // "Finished!" graphics
        private Patch finished;

        // "Entering" graphic
        private Patch entering;

        // "secret"
        private Patch sp_secret;

        // "Kills", "Scrt", "Items", "Frags"
        private Patch kills;
        private Patch secret;
        private Patch items;
        private Patch frags;

        // Time sucks.
        private Patch time;
        private Patch par;
        private Patch sucks;

        // "killers", "victims"
        private Patch killers;
        private Patch victims;

        // "Total", your face, your dead face
        private Patch total;
        private Patch star;
        private Patch bstar;

        // "red P[1..MAXPLAYERS]"
        private Patch[] p;

        // "gray P[1..MAXPLAYERS]"
        private Patch[] bp;

        // Name graphics of each level (centered)
        private Patch[] lnames;

        public CommonPatches(Wad wad)
        {
            background = Patch.FromWad("INTERPIC", wad);

            wiminus = Patch.FromWad("WIMINUS", wad);

            numbers = new Patch[10];
            for (var i = 0; i < 10; i++)
            {
                numbers[i] = Patch.FromWad("WINUM" + i, wad);
            }

            percent = Patch.FromWad("WIPCNT", wad);

            finished = Patch.FromWad("WIF", wad);

            entering = Patch.FromWad("WIENTER", wad);

            kills = Patch.FromWad("WIOSTK", wad);

            secret = Patch.FromWad("WIOSTS", wad);

            sp_secret = Patch.FromWad("WISCRT2", wad);
        }

        public Patch Background => background;
        public Patch[] Numbers => numbers;
    }
}
