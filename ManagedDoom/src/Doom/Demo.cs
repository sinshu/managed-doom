using System;
using System.IO;

namespace ManagedDoom
{
    public sealed class Demo
    {
        private int p;
        private byte[] data;

        private GameOptions options;

        public Demo(byte[] data)
        {
            p = 0;

            if (data[p++] != 109)
            {
                throw new Exception("Demo is from a different game version!");
            }

            this.data = data;

            options = new GameOptions();
            options.Skill = (GameSkill)data[p++];
            options.Episode = data[p++];
            options.Map = data[p++];
            options.Deathmatch = data[p++];
            options.RespawnMonsters = data[p++] != 0;
            options.FastMonsters = data[p++] != 0;
            options.NoMonsters = data[p++] != 0;
            options.ConsolePlayer = data[p++];

            options.PlayerInGame[0] = data[p++] != 0;
            options.PlayerInGame[1] = data[p++] != 0;
            options.PlayerInGame[2] = data[p++] != 0;
            options.PlayerInGame[3] = data[p++] != 0;
        }

        public Demo(string fileName)
            : this(File.ReadAllBytes(fileName))
        {
        }

        public bool ReadCmd(TicCmd[] cmds)
        {
            if (data[p] == 0x80)
            {
                return false;
            }

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (options.PlayerInGame[i])
                {
                    var cmd = cmds[i];
                    cmd.ForwardMove = (sbyte)data[p++];
                    cmd.SideMove = (sbyte)data[p++];
                    cmd.AngleTurn = (short)(data[p++] << 8);
                    cmd.Buttons = data[p++];
                }
            }

            //Console.Write("*");

            return true;
        }

        public GameOptions Options => options;
    }
}
