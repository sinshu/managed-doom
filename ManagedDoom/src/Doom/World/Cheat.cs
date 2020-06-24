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
                Tuple.Create("idspispopd", new Action(NoClip)),
                Tuple.Create("iddt", new Action(FullMap)),
                Tuple.Create("idbehold", new Action(ShowPowerUpList)),
                Tuple.Create("idbeholdv", new Action(ToggleInvulnerability)),
                Tuple.Create("idbeholds", new Action(ToggleStrength)),
                Tuple.Create("idbeholdi", new Action(ToggleInvisibility)),
                Tuple.Create("idbeholdr", new Action(ToggleIronFeet)),
                Tuple.Create("idbeholda", new Action(ToggleAllMap)),
                Tuple.Create("idbeholdl", new Action(ToggleInfrared)),
                Tuple.Create("idchoppers", new Action(GiveChainsaw)),
                Tuple.Create("tntem", new Action(KillMonsters)),
                Tuple.Create("killem", new Action(KillMonsters)),
                Tuple.Create("fhhall", new Action(KillMonsters))
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
            var player = world.ConsolePlayer;
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
            var player = world.ConsolePlayer;
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
            var player = world.ConsolePlayer;
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
            var player = world.ConsolePlayer;
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

        private void ShowPowerUpList()
        {
            var player = world.ConsolePlayer;
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLD);
        }

        private void ToggleInvulnerability()
        {
            var player = world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Invulnerability] > 0)
            {
                player.Powers[(int)PowerType.Invulnerability] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.Invulnerability] = DoomInfo.PowerDuration.Invulnerability;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleStrength()
        {
            var player = world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Strength] != 0)
            {
                player.Powers[(int)PowerType.Strength] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.Strength] = 1;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleInvisibility()
        {
            var player = world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Invisibility] > 0)
            {
                player.Powers[(int)PowerType.Invisibility] = 0;
                player.Mobj.Flags &= ~MobjFlags.Shadow;
            }
            else
            {
                player.Powers[(int)PowerType.Invisibility] = DoomInfo.PowerDuration.Invisibility;
                player.Mobj.Flags |= MobjFlags.Shadow;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleIronFeet()
        {
            var player = world.ConsolePlayer;
            if (player.Powers[(int)PowerType.IronFeet] > 0)
            {
                player.Powers[(int)PowerType.IronFeet] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.IronFeet] = DoomInfo.PowerDuration.IronFeet;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleAllMap()
        {
            var player = world.ConsolePlayer;
            if (player.Powers[(int)PowerType.AllMap] != 0)
            {
                player.Powers[(int)PowerType.AllMap] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.AllMap] = 1;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleInfrared()
        {
            var player = world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Infrared] > 0)
            {
                player.Powers[(int)PowerType.Infrared] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.Infrared] = DoomInfo.PowerDuration.Infrared;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void GiveChainsaw()
        {
            var player = world.ConsolePlayer;
            player.WeaponOwned[(int)WeaponType.Chainsaw] = true;
            player.SendMessage(DoomInfo.Strings.STSTR_CHOPPERS);
        }

        private void KillMonsters()
        {
            var player = world.ConsolePlayer.Mobj;
            foreach (var thinker in world.Thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null &&
                    mobj.Player == null &&
                    ((mobj.Flags & MobjFlags.CountKill) != 0 || mobj.Type == MobjType.Skull))
                {
                    world.ThingInteraction.DamageMobj(mobj, null, player, 10000);
                }
            }
        }
    }
}
