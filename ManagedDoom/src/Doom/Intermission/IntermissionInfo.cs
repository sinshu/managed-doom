using System;

namespace ManagedDoom
{
    public class IntermissionInfo
    {
        // episode # (0-2)
        public int Epsd;

        // if true, splash the secret level
        public bool DidSecret;

        // previous and next levels, origin 0
        public int Last;
        public int Next;

        public int maxKills;
        public int maxItems;
        public int maxSecret;
        public int maxFrags;

        // the par time
        public int ParTime;

        // index of this player in game
        public int PNum;

        public PlayerIntermissionInfo[] Plyr;

        public IntermissionInfo()
        {
            Plyr = new PlayerIntermissionInfo[Player.MaxPlayerCount];
            for (var i = 0; i < Plyr.Length; i++)
            {
                Plyr[i] = new PlayerIntermissionInfo();
            }
        }

        public int MaxKills => Math.Max(maxKills, 1);
        public int MaxItems => Math.Max(maxItems, 1);
        public int MaxSecret => Math.Max(maxSecret, 1);
        public int MaxFrags => Math.Max(maxFrags, 1);
    }
}
