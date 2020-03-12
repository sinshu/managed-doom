using System;
using System.IO;

namespace ManagedDoom
{
    public sealed class Demo
    {
        private int p;
        private byte[] data;

        private GameOptions options;
        private Player[] players;

        public Demo(Resources resources, byte[] data)
        {
            p = 0;

            if (data[p++] != 109)
            {
                throw new Exception("Demo is from a different game version!");
            }

            this.data = data;

            options = new GameOptions();
            options.GameSkill = (Skill)data[p++];
            options.Episode = data[p++];
            options.Map = data[p++];
            options.Deathmatch = data[p++];
            options.RespawnMonsters = data[p++] != 0;
            options.FastMonsters = data[p++] != 0;
            options.NoMonsters = data[p++] != 0;
            options.ConsolePlayer = data[p++];

            players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player();
                players[i].PlayerState = PlayerState.Reborn;
            }
            players[0].InGame = data[p++] != 0;
            players[1].InGame = data[p++] != 0;
            players[2].InGame = data[p++] != 0;
            players[3].InGame = data[p++] != 0;
        }

        public Demo(Resources resources, string fileName)
            : this(resources, File.ReadAllBytes(fileName))
        {
        }

        public bool ReadCmd()
        {
            if (data[p] == 0x80)
            {
                return false;
            }

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame)
                {
                    var cmd = players[i].Cmd;
                    cmd.ForwardMove = (sbyte)data[p++];
                    cmd.SideMove = (sbyte)data[p++];
                    cmd.AngleTurn = (short)(data[p++] << 8);
                    cmd.Buttons = data[p++];
                }
            }

            return true;
        }

        public GameOptions Options => options;
        public Player[] Players => players;
    }
}
