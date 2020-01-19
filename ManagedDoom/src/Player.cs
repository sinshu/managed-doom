using System;

namespace ManagedDoom
{
    public sealed class Player
    {
        public const int MaxPlayerCount = 4;

        public Mobj Mobj;
        public PlayerState playerState;
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
        public int AttackDown;
        public int UseDown;

        // Bit flags, for cheats and debug.
        // See cheat_t, above.
        public int Cheats;

        // Refired shots are less accurate.
        public int Refire;

        // For intermission stats.
        public int KillCount;
        public int ItemCount;
        public int SecretCount;

        // Hint messages.
        string Message;

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
        public PlayerSprite[] PSprites;

        // True if secret level has been done.
        public bool DidSecret;

        public Player()
        {
        }
    }
}
