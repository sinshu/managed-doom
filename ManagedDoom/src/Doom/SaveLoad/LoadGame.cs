using System;

namespace ManagedDoom
{
    public sealed class LoadGame
    {
        private static readonly int descriptionSize = 24;
        private static readonly int versionSize = 16;

        private byte[] data;
        private int save_p;

        private GameOptions options;
        private Player[] players;

        public LoadGame(byte[] data)
        {
            this.data = data;

            var description = ReadDescription();
            var version = ReadVersion();

            options = new GameOptions();
            options.Skill = (GameSkill)data[save_p++];
            options.Episode = data[save_p++];
            options.Map = data[save_p++];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                options.PlayerInGame[i] = data[save_p++] != 0;
            }

            players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player(i);
                players[i].InGame = options.PlayerInGame[i];
            }

            var a = data[save_p++];
            var b = data[save_p++];
            var c = data[save_p++];
            var levelTime = (a << 16) + (b << 8) + c;

            UnArchivePlayers();

            Console.WriteLine("END");
        }

        private void PADSAVEP()
        {
            save_p += (4 - (save_p & 3)) & 3;
        }

        private string ReadDescription()
        {
            var value = DoomInterop.ToString(data, save_p, descriptionSize);
            save_p += descriptionSize;
            return value;
        }

        private string ReadVersion()
        {
            var value = DoomInterop.ToString(data, save_p, versionSize);
            save_p += versionSize;
            return value;
        }

        private static int UnArchivePlayer(Player player, byte[] data, int p)
        {
            player.Clear();

            player.ViewZ = new Fixed(BitConverter.ToInt32(data, p + 16));
            player.ViewHeight = new Fixed(BitConverter.ToInt32(data, p + 20));
            player.DeltaViewHeight = new Fixed(BitConverter.ToInt32(data, p + 24));
            player.Bob = new Fixed(BitConverter.ToInt32(data, p + 28));
            player.Health = BitConverter.ToInt32(data, p + 32);
            player.ArmorPoints = BitConverter.ToInt32(data, p + 36);
            player.ArmorType = BitConverter.ToInt32(data, p + 40);
            for (var i = 0; i < (int)PowerType.Count; i++)
            {
                player.Powers[i] = BitConverter.ToInt32(data, p + 44 + 4 * i);
            }
            for (var i = 0; i < (int)PowerType.Count; i++)
            {
                player.Cards[i] = BitConverter.ToInt32(data, p + 68 + 4 * i) != 0;
            }
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                player.Frags[i] = BitConverter.ToInt32(data, p + 96 + 4 * i);
            }
            player.ReadyWeapon = (WeaponType)BitConverter.ToInt32(data, p + 112);
            player.PendingWeapon = (WeaponType)BitConverter.ToInt32(data, p + 116);
            for (var i = 0; i < (int)WeaponType.Count; i++)
            {
                player.WeaponOwned[i] = BitConverter.ToInt32(data, p + 120 + 4 * i) != 0;
            }
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                player.Ammo[i] = BitConverter.ToInt32(data, p + 156 + 4 * i);
            }
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                player.MaxAmmo[i] = BitConverter.ToInt32(data, p + 172 + 4 * i);
            }
            player.AttackDown = BitConverter.ToInt32(data, p + 188) != 0;
            player.UseDown = BitConverter.ToInt32(data, p + 192) != 0;
            player.Cheats = (CheatFlags)BitConverter.ToInt32(data, p + 196);
            player.Refire = BitConverter.ToInt32(data, p + 200);
            player.KillCount = BitConverter.ToInt32(data, p + 204);
            player.ItemCount = BitConverter.ToInt32(data, p + 208);
            player.SecretCount = BitConverter.ToInt32(data, p + 212);
            player.DamageCount = BitConverter.ToInt32(data, p + 220);
            player.BonusCount = BitConverter.ToInt32(data, p + 224);
            player.ExtraLight = BitConverter.ToInt32(data, p + 232);
            player.FixedColorMap = BitConverter.ToInt32(data, p + 236);
            player.ColorMap = BitConverter.ToInt32(data, p + 240);
            for (var i = 0; i < (int)PlayerSprite.Count; i++)
            {
                player.PlayerSprites[i].State = DoomInfo.States[BitConverter.ToInt32(data, p + 244 + 16 * i)];
                player.PlayerSprites[i].Tics = BitConverter.ToInt32(data, p + 244 + 16 * i + 4);
                player.PlayerSprites[i].Sx = new Fixed(BitConverter.ToInt32(data, p + 244 + 16 * i + 8));
                player.PlayerSprites[i].Sy = new Fixed(BitConverter.ToInt32(data, p + 244 + 16 * i + 12));
            }
            player.DidSecret = BitConverter.ToInt32(data, p + 276) != 0;

            return p + 280;
        }

        private void UnArchivePlayers()
        {
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!options.PlayerInGame[i])
                {
                    continue;
                }

                PADSAVEP();

                save_p = UnArchivePlayer(players[i], data, save_p);
            }
        }
    }
}
