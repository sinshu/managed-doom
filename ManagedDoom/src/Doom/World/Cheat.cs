using System;
using System.Runtime.CompilerServices;

namespace ManagedDoom
{
    public sealed class Cheat
    {
        private World world;

        private Tuple<string, Action>[] list;

        private char[] buffer;
        private int p;

        public Cheat(World world)
        {
            this.world = world;

            list = new Tuple<string, Action>[]
            {
                Tuple.Create("iddqd", new Action(GodMode)),
                Tuple.Create("idclip", new Action(NoClip)),
                Tuple.Create("iddt", new Action(FullMap))
            };

            var max = 0;
            foreach (var tuple in list)
            {
                if (tuple.Item1.Length > max)
                {
                    max = tuple.Item1.Length;
                }
            }
            buffer = new char[max];

            p = 0;
        }

        public bool DoEvent(DoomEvent e)
        {
            if (e.Type == EventType.KeyDown)
            {
                buffer[p] = e.Key.GetChar();

                p = (p + 1) % buffer.Length;

                CheckBuffer();
            }

            return true;
        }

        private void CheckBuffer()
        {
            for (var i = 0; i < list.Length; i++)
            {
                var code = list[i].Item1;
                var q = p;
                int j;
                for (j = 0; j < code.Length; j++)
                {
                    q--;
                    if (q == -1)
                    {
                        q = buffer.Length - 1;
                    }
                    if (buffer[q] != code[code.Length - j - 1])
                    {
                        break;
                    }
                }
                if (j == code.Length)
                {
                    list[i].Item2();
                }
            }
        }


        private void GodMode()
        {
            var player = world.Players[world.Options.ConsolePlayer];
            player.Cheats |= CheatFlags.GodMode;
        }

        private void NoClip()
        {
            var player = world.Players[world.Options.ConsolePlayer];
            if ((player.Cheats & CheatFlags.NoClip) != 0)
            {
                player.Cheats &= ~CheatFlags.NoClip;
            }
            else
            {
                player.Cheats |= CheatFlags.NoClip;
            }
        }

        private void FullMap()
        {
            world.AutoMap.ToggleCheat();
        }
    }
}
