using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public class PlayerScores
    {
        // Whether the player is in game.
        private bool inGame;
    
        // Player stats, kills, collected items etc.
        private int killCount;
        private int itemCount;
        private int secretCount;
        private int time;
        private int[] frags;

        public PlayerScores()
        {
            frags = new int[Player.MaxPlayerCount];
        }

        public bool InGame
        {
            get => inGame;
            set => inGame = value;
        }

        public int KillCount
        {
            get => killCount;
            set => killCount = value;
        }

        public int ItemCount
        {
            get => itemCount;
            set => itemCount = value;
        }

        public int SecretCount
        {
            get => secretCount;
            set => secretCount = value;
        }

        public int Time
        {
            get => time;
            set => time = value;
        }

        public int[] Frags
        {
            get => frags;
        }
    }
}
