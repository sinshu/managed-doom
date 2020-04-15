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

        public int MaxKills;
        public int MaxItems;
        public int MaxSecret;
        public int MaxFrags;

        // the par time
        public int ParTime;

        // index of this player in game
        public int PNum;

        public PlayerIntermissionInfo[] Plyr;

        public IntermissionInfo()
        {
            Plyr = new PlayerIntermissionInfo[Player.MaxPlayerCount];
        }
    }
}
