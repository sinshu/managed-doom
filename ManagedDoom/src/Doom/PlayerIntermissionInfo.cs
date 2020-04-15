using System;

namespace ManagedDoom
{
    public class PlayerIntermissionInfo
    {
        // whether the player is in game
        public bool InGame;
    
        // Player stats, kills, collected items etc.
        public int Skills;
        public int SItems;
        public int SSecret;
        public int STime;
        public int[] Frags;

        // current score on entry, modified on return
        public int Score;

        public PlayerIntermissionInfo()
        {
            Frags = new int[Player.MaxPlayerCount];
        }
    }
}
