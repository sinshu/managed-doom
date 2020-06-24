using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ManagedDoom
{
    public sealed class Cheat
    {
        private static Tuple<string, Action<Cheat>>[] list = new Tuple<string, Action<Cheat>>[]
        {
            Tuple.Create("idfa", (Action<Cheat>)(cheat => cheat.FullAmmo())),
            Tuple.Create("idkfa", (Action<Cheat>)(cheat => cheat.FullAmmoAndKeys())),
            Tuple.Create("iddqd", (Action<Cheat>)(cheat => cheat.GodMode())),
            Tuple.Create("idclip", (Action<Cheat>)(cheat => cheat.NoClip())),
            Tuple.Create("idspispopd", (Action<Cheat>)(cheat => cheat.NoClip())),
            Tuple.Create("iddt", (Action<Cheat>)(cheat => cheat.FullMap())),
            Tuple.Create("idbehold", (Action<Cheat>)(cheat => cheat.ShowPowerUpList())),
            Tuple.Create("idbeholdv", (Action<Cheat>)(cheat => cheat.ToggleInvulnerability())),
            Tuple.Create("idbeholds", (Action<Cheat>)(cheat => cheat.ToggleStrength())),
            Tuple.Create("idbeholdi", (Action<Cheat>)(cheat => cheat.ToggleInvisibility())),
            Tuple.Create("idbeholdr", (Action<Cheat>)(cheat => cheat.ToggleIronFeet())),
            Tuple.Create("idbeholda", (Action<Cheat>)(cheat => cheat.ToggleAllMap())),
            Tuple.Create("idbeholdl", (Action<Cheat>)(cheat => cheat.ToggleInfrared())),
            Tuple.Create("idchoppers", (Action<Cheat>)(cheat => cheat.GiveChainsaw())),
            Tuple.Create("tntem", (Action<Cheat>)(cheat => cheat.KillMonsters())),
            Tuple.Create("killem", (Action<Cheat>)(cheat => cheat.KillMonsters())),
            Tuple.Create("fhhall", (Action<Cheat>)(cheat => cheat.KillMonsters()))
        };

        private static readonly int maxLength = list.Max(tuple => tuple.Item1.Length);

        private World world;

        private char[] buffer;
        private int p;

        public Cheat(World world)
        {
            this.world = world;

            buffer = new char[maxLength];
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
                    list[i].Item2(this);
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
            var player = world.ConsolePlayer;
            var count = 0;
            foreach (var thinker in world.Thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null &&
                    mobj.Player == null &&
                    ((mobj.Flags & MobjFlags.CountKill) != 0 || mobj.Type == MobjType.Skull))
                {
                    world.ThingInteraction.DamageMobj(mobj, null, player.Mobj, 10000);
                    count++;
                }
            }
            player.SendMessage(count + " monsters killed");
        }
    }
}
