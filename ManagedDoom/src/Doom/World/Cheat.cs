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
                Tuple.Create("idfa", new Action(FullAmmo)),
                Tuple.Create("idkfa", new Action(FullAmmoAndKeys)),
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


        private void FullAmmo()
        {
            var player = world.Options.Players[world.Options.ConsolePlayer];
            for (var i = 0; i < (int)WeaponType.Count; i++)
            {
                player.WeaponOwned[i] = true;
            }
            player.Backpack = true;
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                player.MaxAmmo[i] = 2 * DoomInfo.AmmoInfos.Max[i];
                player.Ammo[i] = 2 * DoomInfo.AmmoInfos.Max[i];
            }
            player.SendMessage(DoomInfo.Strings.STSTR_FAADDED);
        }

        private void FullAmmoAndKeys()
        {
            var player = world.Options.Players[world.Options.ConsolePlayer];
            for (var i = 0; i < (int)WeaponType.Count; i++)
            {
                player.WeaponOwned[i] = true;
            }
            player.Backpack = true;
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                player.MaxAmmo[i] = 2 * DoomInfo.AmmoInfos.Max[i];
                player.Ammo[i] = 2 * DoomInfo.AmmoInfos.Max[i];
            }
            for (var i = 0; i < (int)CardType.Count; i++)
            {
                player.Cards[i] = true;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_KFAADDED);
        }

        private void GodMode()
        {
            var player = world.Options.Players[world.Options.ConsolePlayer];
            if ((player.Cheats & CheatFlags.GodMode) != 0)
            {
                player.Cheats &= ~CheatFlags.GodMode;
                player.SendMessage(DoomInfo.Strings.STSTR_DQDOFF);
            }
            else
            {
                player.Cheats |= CheatFlags.GodMode;
                player.SendMessage(DoomInfo.Strings.STSTR_DQDON);
            }
        }

        private void NoClip()
        {
            var player = world.Options.Players[world.Options.ConsolePlayer];
            if ((player.Cheats & CheatFlags.NoClip) != 0)
            {
                player.Cheats &= ~CheatFlags.NoClip;
                player.SendMessage(DoomInfo.Strings.STSTR_NCOFF);
            }
            else
            {
                player.Cheats |= CheatFlags.NoClip;
                player.SendMessage(DoomInfo.Strings.STSTR_NCON);
            }
        }

        private void FullMap()
        {
            world.AutoMap.ToggleCheat();
        }
    }
}
