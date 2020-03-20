using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        public static readonly string[] SpriteNames =
        {
            "TROO", "SHTG", "PUNG", "PISG", "PISF", "SHTF", "SHT2", "CHGG", "CHGF", "MISG",
            "MISF", "SAWG", "PLSG", "PLSF", "BFGG", "BFGF", "BLUD", "PUFF", "BAL1", "BAL2",
            "PLSS", "PLSE", "MISL", "BFS1", "BFE1", "BFE2", "TFOG", "IFOG", "PLAY", "POSS",
            "SPOS", "VILE", "FIRE", "FATB", "FBXP", "SKEL", "MANF", "FATT", "CPOS", "SARG",
            "HEAD", "BAL7", "BOSS", "BOS2", "SKUL", "SPID", "BSPI", "APLS", "APBX", "CYBR",
            "PAIN", "SSWV", "KEEN", "BBRN", "BOSF", "ARM1", "ARM2", "BAR1", "BEXP", "FCAN",
            "BON1", "BON2", "BKEY", "RKEY", "YKEY", "BSKU", "RSKU", "YSKU", "STIM", "MEDI",
            "SOUL", "PINV", "PSTR", "PINS", "MEGA", "SUIT", "PMAP", "PVIS", "CLIP", "AMMO",
            "ROCK", "BROK", "CELL", "CELP", "SHEL", "SBOX", "BPAK", "BFUG", "MGUN", "CSAW",
            "LAUN", "PLAS", "SHOT", "SGN2", "COLU", "SMT2", "GOR1", "POL2", "POL5", "POL4",
            "POL3", "POL1", "POL6", "GOR2", "GOR3", "GOR4", "GOR5", "SMIT", "COL1", "COL2",
            "COL3", "COL4", "CAND", "CBRA", "COL6", "TRE1", "TRE2", "ELEC", "CEYE", "FSKU",
            "COL5", "TBLU", "TGRN", "TRED", "SMBT", "SMGT", "SMRT", "HDB1", "HDB2", "HDB3",
            "HDB4", "HDB5", "HDB6", "POB1", "POB2", "BRS1", "TLMP", "TLP2"
        };

        public static readonly WeaponInfo[] WeaponInfos = new WeaponInfo[]
        {
            // fist
            new WeaponInfo(
                AmmoType.NoAmmo,
                State.Punchup,
                State.Punchdown,
                State.Punch,
                State.Punch1,
                State.Null),

            // pistol
            new WeaponInfo(
                AmmoType.Clip,
                State.Pistolup,
                State.Pistoldown,
                State.Pistol,
                State.Pistol1,
                State.Pistolflash),

            // shotgun
            new WeaponInfo(
                AmmoType.Shell,
                State.Sgunup,
                State.Sgundown,
                State.Sgun,
                State.Sgun1,
                State.Sgunflash1),

            // chaingun
            new WeaponInfo(
                AmmoType.Clip,
                State.Chainup,
                State.Chaindown,
                State.Chain,
                State.Chain1,
                State.Chainflash1),

            // missile launcher
            new WeaponInfo(
                AmmoType.Missile,
                State.Missileup,
                State.Missiledown,
                State.Missile,
                State.Missile1,
                State.Missileflash1),

            // plasma rifle
            new WeaponInfo(
                AmmoType.Cell,
                State.Plasmaup,
                State.Plasmadown,
                State.Plasma,
                State.Plasma1,
                State.Plasmaflash1),

            // bfg 9000
            new WeaponInfo(
                AmmoType.Cell,
                State.Bfgup,
                State.Bfgdown,
                State.Bfg,
                State.Bfg1,
                State.Bfgflash1),

            // chainsaw
            new WeaponInfo(
                AmmoType.Cell,
                State.Sawup,
                State.Sawdown,
                State.Saw,
                State.Saw1,
                State.Null),

            // // super shotgun
            new WeaponInfo(
                AmmoType.Shell,
                State.Dsgunup,
                State.Dsgundown,
                State.Dsgun,
                State.Dsgun1,
                State.Dsgunflash1)
        };
    }
}
