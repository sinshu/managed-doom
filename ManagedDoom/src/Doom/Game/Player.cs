using System;

namespace ManagedDoom
{
    public sealed class Player
    {
        public const int MaxPlayerCount = 4;

        public const int MAXHEALTH = 100;
        public static readonly Fixed VIEWHEIGHT = Fixed.FromInt(41);

        public int Number;
        public bool InGame;

        public Mobj Mobj;
        public PlayerState PlayerState;
        public TicCmd Cmd;

        // Determine POV,
        //  including viewpoint bobbing during movement.
        // Focal origin above r.z
        public Fixed ViewZ;

        // Base height above floor for viewz.
        public Fixed ViewHeight;

        // Bob/squat speed.
        public Fixed DeltaViewHeight;

        // bounded/scaled total momentum.
        public Fixed Bob;

        // This is only used between levels,
        // mo->health is used during levels.
        public int Health;
        public int ArmorPoints;

        // Armor type is 0-2.
        public int ArmorType;

        // Power ups. invinc and invis are tic counters.
        public int[] Powers;
        public bool[] Cards;
        public bool Backpack;

        // Frags, kills of other players.
        public int[] Frags;

        public WeaponType ReadyWeapon;

        // Is wp_nochange if not changing.
        public WeaponType PendingWeapon;

        public bool[] WeaponOwned;
        public int[] Ammo;
        public int[] MaxAmmo;

        // True if button down last tic.
        public bool AttackDown;
        public bool UseDown;

        // Bit flags, for cheats and debug.
        // See cheat_t, above.
        public CheatFlags Cheats;

        // Refired shots are less accurate.
        public int Refire;

        // For intermission stats.
        public int KillCount;
        public int ItemCount;
        public int SecretCount;

        // Hint messages.
        public string Message;
        public int MessageTime;

        // For screen flashing (red or bright).
        public int DamageCount;
        public int BonusCount;

        // Who did damage (NULL for floors/ceilings).
        public Mobj Attacker;

        // So gun flashes light up areas.
        public int ExtraLight;

        // Current PLAYPAL, ???
        //  can be set to REDCOLORMAP for pain, etc.
        public int FixedColorMap;

        // Player skin colorshift,
        //  0-3 for which color to draw player.
        public int ColorMap;

        // Overlay view sprites (gun, etc).
        public PlayerSpriteDef[] PlayerSprites;

        // True if secret level has been done.
        public bool DidSecret;

        public Player(int number)
        {
            Number = number;

            Cmd = new TicCmd();

            Powers = new int[(int)PowerType.Count];
            Cards = new bool[(int)CardType.Count];

            Frags = new int[MaxPlayerCount];

            WeaponOwned = new bool[(int)WeaponType.Count];
            Ammo = new int[(int)WeaponType.Count];
            MaxAmmo = new int[(int)WeaponType.Count];

            PlayerSprites = new PlayerSpriteDef[(int)PlayerSprite.Count];
            for (var i = 0; i < PlayerSprites.Length; i++)
            {
                PlayerSprites[i] = new PlayerSpriteDef();
            }
        }

        public void Reborn()
        {
            Mobj = null;
            PlayerState = PlayerState.Live;
            Cmd.Clear();

            ViewZ = Fixed.Zero;
            ViewHeight = Fixed.Zero;
            DeltaViewHeight = Fixed.Zero;
            Bob = Fixed.Zero;

            Health = MAXHEALTH;
            ArmorPoints = 0;
            ArmorType = 0;

            Array.Clear(Powers, 0, Powers.Length);
            Array.Clear(Cards, 0, Cards.Length);
            Backpack = false;

            ReadyWeapon = WeaponType.Pistol;
            PendingWeapon = WeaponType.Pistol;

            Array.Clear(WeaponOwned, 0, WeaponOwned.Length);
            Array.Clear(Ammo, 0, Ammo.Length);
            Array.Clear(MaxAmmo, 0, MaxAmmo.Length);

            WeaponOwned[(int)WeaponType.Fist] = true;
            WeaponOwned[(int)WeaponType.Pistol] = true;
            Ammo[(int)AmmoType.Clip] = 50;
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                MaxAmmo[i] = DoomInfo.AmmoInfos.Max[i];
            }

            // don't do anything immediately
            UseDown = true;
            AttackDown = true;

            Cheats = 0;

            Refire = 0;

            Message = null;
            MessageTime = 0;

            DamageCount = 0;
            BonusCount = 0;

            Attacker = null;

            ExtraLight = 0;

            FixedColorMap = 0;

            ColorMap = 0;

            foreach (var psp in PlayerSprites)
            {
                psp.Clear();
            }

            DidSecret = false;
        }

        public void SendMessage(string message)
        {
            Message = message;
            MessageTime = 4 * GameConstants.TicRate;
        }
    }
}
