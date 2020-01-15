using System;

namespace ManagedDoom
{
    public static class Info
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

        public static readonly StateDef[] States =
        {
            new StateDef(Sprite.TROO, 0, -1, null, null, State.Null, 0, 0), // State.Null
            new StateDef(Sprite.SHTG, 4, 0, PlayerActions.Light0, null, State.Null, 0, 0), // State.Lightdone
            new StateDef(Sprite.PUNG, 0, 1, PlayerActions.WeaponReady, null, State.Punch, 0, 0), // State.Punch
            new StateDef(Sprite.PUNG, 0, 1, PlayerActions.Lower, null, State.Punchdown, 0, 0), // State.Punchdown
            new StateDef(Sprite.PUNG, 0, 1, PlayerActions.Raise, null, State.Punchup, 0, 0), // State.Punchup
            new StateDef(Sprite.PUNG, 1, 4, null, null, State.Punch2, 0, 0), // State.Punch1
            new StateDef(Sprite.PUNG, 2, 4, PlayerActions.Punch, null, State.Punch3, 0, 0), // State.Punch2
            new StateDef(Sprite.PUNG, 3, 5, null, null, State.Punch4, 0, 0), // State.Punch3
            new StateDef(Sprite.PUNG, 2, 4, null, null, State.Punch5, 0, 0), // State.Punch4
            new StateDef(Sprite.PUNG, 1, 5, PlayerActions.ReFire, null, State.Punch, 0, 0), // State.Punch5
            new StateDef(Sprite.PISG, 0, 1, PlayerActions.WeaponReady, null, State.Pistol, 0, 0), // State.Pistol
            new StateDef(Sprite.PISG, 0, 1, PlayerActions.Lower, null, State.Pistoldown, 0, 0), // State.Pistoldown
            new StateDef(Sprite.PISG, 0, 1, PlayerActions.Raise, null, State.Pistolup, 0, 0), // State.Pistolup
            new StateDef(Sprite.PISG, 0, 4, null, null, State.Pistol2, 0, 0), // State.Pistol1
            new StateDef(Sprite.PISG, 1, 6, PlayerActions.FirePistol, null, State.Pistol3, 0, 0), // State.Pistol2
            new StateDef(Sprite.PISG, 2, 4, null, null, State.Pistol4, 0, 0), // State.Pistol3
            new StateDef(Sprite.PISG, 1, 5, PlayerActions.ReFire, null, State.Pistol, 0, 0), // State.Pistol4
            new StateDef(Sprite.PISF, 32768, 7, PlayerActions.Light1, null, State.Lightdone, 0, 0), // State.Pistolflash
            new StateDef(Sprite.SHTG, 0, 1, PlayerActions.WeaponReady, null, State.Sgun, 0, 0), // State.Sgun
            new StateDef(Sprite.SHTG, 0, 1, PlayerActions.Lower, null, State.Sgundown, 0, 0), // State.Sgundown
            new StateDef(Sprite.SHTG, 0, 1, PlayerActions.Raise, null, State.Sgunup, 0, 0), // State.Sgunup
            new StateDef(Sprite.SHTG, 0, 3, null, null, State.Sgun2, 0, 0), // State.Sgun1
            new StateDef(Sprite.SHTG, 0, 7, PlayerActions.FireShotgun, null, State.Sgun3, 0, 0), // State.Sgun2
            new StateDef(Sprite.SHTG, 1, 5, null, null, State.Sgun4, 0, 0), // State.Sgun3
            new StateDef(Sprite.SHTG, 2, 5, null, null, State.Sgun5, 0, 0), // State.Sgun4
            new StateDef(Sprite.SHTG, 3, 4, null, null, State.Sgun6, 0, 0), // State.Sgun5
            new StateDef(Sprite.SHTG, 2, 5, null, null, State.Sgun7, 0, 0), // State.Sgun6
            new StateDef(Sprite.SHTG, 1, 5, null, null, State.Sgun8, 0, 0), // State.Sgun7
            new StateDef(Sprite.SHTG, 0, 3, null, null, State.Sgun9, 0, 0), // State.Sgun8
            new StateDef(Sprite.SHTG, 0, 7, PlayerActions.ReFire, null, State.Sgun, 0, 0), // State.Sgun9
            new StateDef(Sprite.SHTF, 32768, 4, PlayerActions.Light1, null, State.Sgunflash2, 0, 0), // State.Sgunflash1
            new StateDef(Sprite.SHTF, 32769, 3, PlayerActions.Light2, null, State.Lightdone, 0, 0), // State.Sgunflash2
            new StateDef(Sprite.SHT2, 0, 1, PlayerActions.WeaponReady, null, State.Dsgun, 0, 0), // State.Dsgun
            new StateDef(Sprite.SHT2, 0, 1, PlayerActions.Lower, null, State.Dsgundown, 0, 0), // State.Dsgundown
            new StateDef(Sprite.SHT2, 0, 1, PlayerActions.Raise, null, State.Dsgunup, 0, 0), // State.Dsgunup
            new StateDef(Sprite.SHT2, 0, 3, null, null, State.Dsgun2, 0, 0), // State.Dsgun1
            new StateDef(Sprite.SHT2, 0, 7, PlayerActions.FireShotgun2, null, State.Dsgun3, 0, 0), // State.Dsgun2
            new StateDef(Sprite.SHT2, 1, 7, null, null, State.Dsgun4, 0, 0), // State.Dsgun3
            new StateDef(Sprite.SHT2, 2, 7, PlayerActions.CheckReload, null, State.Dsgun5, 0, 0), // State.Dsgun4
            new StateDef(Sprite.SHT2, 3, 7, PlayerActions.OpenShotgun2, null, State.Dsgun6, 0, 0), // State.Dsgun5
            new StateDef(Sprite.SHT2, 4, 7, null, null, State.Dsgun7, 0, 0), // State.Dsgun6
            new StateDef(Sprite.SHT2, 5, 7, PlayerActions.LoadShotgun2, null, State.Dsgun8, 0, 0), // State.Dsgun7
            new StateDef(Sprite.SHT2, 6, 6, null, null, State.Dsgun9, 0, 0), // State.Dsgun8
            new StateDef(Sprite.SHT2, 7, 6, PlayerActions.CloseShotgun2, null, State.Dsgun10, 0, 0), // State.Dsgun9
            new StateDef(Sprite.SHT2, 0, 5, PlayerActions.ReFire, null, State.Dsgun, 0, 0), // State.Dsgun10
            new StateDef(Sprite.SHT2, 1, 7, null, null, State.Dsnr2, 0, 0), // State.Dsnr1
            new StateDef(Sprite.SHT2, 0, 3, null, null, State.Dsgundown, 0, 0), // State.Dsnr2
            new StateDef(Sprite.SHT2, 32776, 5, PlayerActions.Light1, null, State.Dsgunflash2, 0, 0), // State.Dsgunflash1
            new StateDef(Sprite.SHT2, 32777, 4, PlayerActions.Light2, null, State.Lightdone, 0, 0), // State.Dsgunflash2
            new StateDef(Sprite.CHGG, 0, 1, PlayerActions.WeaponReady, null, State.Chain, 0, 0), // State.Chain
            new StateDef(Sprite.CHGG, 0, 1, PlayerActions.Lower, null, State.Chaindown, 0, 0), // State.Chaindown
            new StateDef(Sprite.CHGG, 0, 1, PlayerActions.Raise, null, State.Chainup, 0, 0), // State.Chainup
            new StateDef(Sprite.CHGG, 0, 4, PlayerActions.FireCGun, null, State.Chain2, 0, 0), // State.Chain1
            new StateDef(Sprite.CHGG, 1, 4, PlayerActions.FireCGun, null, State.Chain3, 0, 0), // State.Chain2
            new StateDef(Sprite.CHGG, 1, 0, PlayerActions.ReFire, null, State.Chain, 0, 0), // State.Chain3
            new StateDef(Sprite.CHGF, 32768, 5, PlayerActions.Light1, null, State.Lightdone, 0, 0), // State.Chainflash1
            new StateDef(Sprite.CHGF, 32769, 5, PlayerActions.Light2, null, State.Lightdone, 0, 0), // State.Chainflash2
            new StateDef(Sprite.MISG, 0, 1, PlayerActions.WeaponReady, null, State.Missile, 0, 0), // State.Missile
            new StateDef(Sprite.MISG, 0, 1, PlayerActions.Lower, null, State.Missiledown, 0, 0), // State.Missiledown
            new StateDef(Sprite.MISG, 0, 1, PlayerActions.Raise, null, State.Missileup, 0, 0), // State.Missileup
            new StateDef(Sprite.MISG, 1, 8, PlayerActions.GunFlash, null, State.Missile2, 0, 0), // State.Missile1
            new StateDef(Sprite.MISG, 1, 12, PlayerActions.FireMissile, null, State.Missile3, 0, 0), // State.Missile2
            new StateDef(Sprite.MISG, 1, 0, PlayerActions.ReFire, null, State.Missile, 0, 0), // State.Missile3
            new StateDef(Sprite.MISF, 32768, 3, PlayerActions.Light1, null, State.Missileflash2, 0, 0), // State.Missileflash1
            new StateDef(Sprite.MISF, 32769, 4, null, null, State.Missileflash3, 0, 0), // State.Missileflash2
            new StateDef(Sprite.MISF, 32770, 4, PlayerActions.Light2, null, State.Missileflash4, 0, 0), // State.Missileflash3
            new StateDef(Sprite.MISF, 32771, 4, PlayerActions.Light2, null, State.Lightdone, 0, 0), // State.Missileflash4
            new StateDef(Sprite.SAWG, 2, 4, PlayerActions.WeaponReady, null, State.Sawb, 0, 0), // State.Saw
            new StateDef(Sprite.SAWG, 3, 4, PlayerActions.WeaponReady, null, State.Saw, 0, 0), // State.Sawb
            new StateDef(Sprite.SAWG, 2, 1, PlayerActions.Lower, null, State.Sawdown, 0, 0), // State.Sawdown
            new StateDef(Sprite.SAWG, 2, 1, PlayerActions.Raise, null, State.Sawup, 0, 0), // State.Sawup
            new StateDef(Sprite.SAWG, 0, 4, PlayerActions.Saw, null, State.Saw2, 0, 0), // State.Saw1
            new StateDef(Sprite.SAWG, 1, 4, PlayerActions.Saw, null, State.Saw3, 0, 0), // State.Saw2
            new StateDef(Sprite.SAWG, 1, 0, PlayerActions.ReFire, null, State.Saw, 0, 0), // State.Saw3
            new StateDef(Sprite.PLSG, 0, 1, PlayerActions.WeaponReady, null, State.Plasma, 0, 0), // State.Plasma
            new StateDef(Sprite.PLSG, 0, 1, PlayerActions.Lower, null, State.Plasmadown, 0, 0), // State.Plasmadown
            new StateDef(Sprite.PLSG, 0, 1, PlayerActions.Raise, null, State.Plasmaup, 0, 0), // State.Plasmaup
            new StateDef(Sprite.PLSG, 0, 3, PlayerActions.FirePlasma, null, State.Plasma2, 0, 0), // State.Plasma1
            new StateDef(Sprite.PLSG, 1, 20, PlayerActions.ReFire, null, State.Plasma, 0, 0), // State.Plasma2
            new StateDef(Sprite.PLSF, 32768, 4, PlayerActions.Light1, null, State.Lightdone, 0, 0), // State.Plasmaflash1
            new StateDef(Sprite.PLSF, 32769, 4, PlayerActions.Light1, null, State.Lightdone, 0, 0), // State.Plasmaflash2
            new StateDef(Sprite.BFGG, 0, 1, PlayerActions.WeaponReady, null, State.Bfg, 0, 0), // State.Bfg
            new StateDef(Sprite.BFGG, 0, 1, PlayerActions.Lower, null, State.Bfgdown, 0, 0), // State.Bfgdown
            new StateDef(Sprite.BFGG, 0, 1, PlayerActions.Raise, null, State.Bfgup, 0, 0), // State.Bfgup
            new StateDef(Sprite.BFGG, 0, 20, PlayerActions.BFGsound, null, State.Bfg2, 0, 0), // State.Bfg1
            new StateDef(Sprite.BFGG, 1, 10, PlayerActions.GunFlash, null, State.Bfg3, 0, 0), // State.Bfg2
            new StateDef(Sprite.BFGG, 1, 10, PlayerActions.FireBFG, null, State.Bfg4, 0, 0), // State.Bfg3
            new StateDef(Sprite.BFGG, 1, 20, PlayerActions.ReFire, null, State.Bfg, 0, 0), // State.Bfg4
            new StateDef(Sprite.BFGF, 32768, 11, PlayerActions.Light1, null, State.Bfgflash2, 0, 0), // State.Bfgflash1
            new StateDef(Sprite.BFGF, 32769, 6, PlayerActions.Light2, null, State.Lightdone, 0, 0), // State.Bfgflash2
            new StateDef(Sprite.BLUD, 2, 8, null, null, State.Blood2, 0, 0), // State.Blood1
            new StateDef(Sprite.BLUD, 1, 8, null, null, State.Blood3, 0, 0), // State.Blood2
            new StateDef(Sprite.BLUD, 0, 8, null, null, State.Null, 0, 0), // State.Blood3
            new StateDef(Sprite.PUFF, 32768, 4, null, null, State.Puff2, 0, 0), // State.Puff1
            new StateDef(Sprite.PUFF, 1, 4, null, null, State.Puff3, 0, 0), // State.Puff2
            new StateDef(Sprite.PUFF, 2, 4, null, null, State.Puff4, 0, 0), // State.Puff3
            new StateDef(Sprite.PUFF, 3, 4, null, null, State.Null, 0, 0), // State.Puff4
            new StateDef(Sprite.BAL1, 32768, 4, null, null, State.Tball2, 0, 0), // State.Tball1
            new StateDef(Sprite.BAL1, 32769, 4, null, null, State.Tball1, 0, 0), // State.Tball2
            new StateDef(Sprite.BAL1, 32770, 6, null, null, State.Tballx2, 0, 0), // State.Tballx1
            new StateDef(Sprite.BAL1, 32771, 6, null, null, State.Tballx3, 0, 0), // State.Tballx2
            new StateDef(Sprite.BAL1, 32772, 6, null, null, State.Null, 0, 0), // State.Tballx3
            new StateDef(Sprite.BAL2, 32768, 4, null, null, State.Rball2, 0, 0), // State.Rball1
            new StateDef(Sprite.BAL2, 32769, 4, null, null, State.Rball1, 0, 0), // State.Rball2
            new StateDef(Sprite.BAL2, 32770, 6, null, null, State.Rballx2, 0, 0), // State.Rballx1
            new StateDef(Sprite.BAL2, 32771, 6, null, null, State.Rballx3, 0, 0), // State.Rballx2
            new StateDef(Sprite.BAL2, 32772, 6, null, null, State.Null, 0, 0), // State.Rballx3
            new StateDef(Sprite.PLSS, 32768, 6, null, null, State.Plasball2, 0, 0), // State.Plasball
            new StateDef(Sprite.PLSS, 32769, 6, null, null, State.Plasball, 0, 0), // State.Plasball2
            new StateDef(Sprite.PLSE, 32768, 4, null, null, State.Plasexp2, 0, 0), // State.Plasexp
            new StateDef(Sprite.PLSE, 32769, 4, null, null, State.Plasexp3, 0, 0), // State.Plasexp2
            new StateDef(Sprite.PLSE, 32770, 4, null, null, State.Plasexp4, 0, 0), // State.Plasexp3
            new StateDef(Sprite.PLSE, 32771, 4, null, null, State.Plasexp5, 0, 0), // State.Plasexp4
            new StateDef(Sprite.PLSE, 32772, 4, null, null, State.Null, 0, 0), // State.Plasexp5
            new StateDef(Sprite.MISL, 32768, 1, null, null, State.Rocket, 0, 0), // State.Rocket
            new StateDef(Sprite.BFS1, 32768, 4, null, null, State.Bfgshot2, 0, 0), // State.Bfgshot
            new StateDef(Sprite.BFS1, 32769, 4, null, null, State.Bfgshot, 0, 0), // State.Bfgshot2
            new StateDef(Sprite.BFE1, 32768, 8, null, null, State.Bfgland2, 0, 0), // State.Bfgland
            new StateDef(Sprite.BFE1, 32769, 8, null, null, State.Bfgland3, 0, 0), // State.Bfgland2
            new StateDef(Sprite.BFE1, 32770, 8, null, MobjActions.BFGSpray, State.Bfgland4, 0, 0), // State.Bfgland3
            new StateDef(Sprite.BFE1, 32771, 8, null, null, State.Bfgland5, 0, 0), // State.Bfgland4
            new StateDef(Sprite.BFE1, 32772, 8, null, null, State.Bfgland6, 0, 0), // State.Bfgland5
            new StateDef(Sprite.BFE1, 32773, 8, null, null, State.Null, 0, 0), // State.Bfgland6
            new StateDef(Sprite.BFE2, 32768, 8, null, null, State.Bfgexp2, 0, 0), // State.Bfgexp
            new StateDef(Sprite.BFE2, 32769, 8, null, null, State.Bfgexp3, 0, 0), // State.Bfgexp2
            new StateDef(Sprite.BFE2, 32770, 8, null, null, State.Bfgexp4, 0, 0), // State.Bfgexp3
            new StateDef(Sprite.BFE2, 32771, 8, null, null, State.Null, 0, 0), // State.Bfgexp4
            new StateDef(Sprite.MISL, 32769, 8, null, MobjActions.Explode, State.Explode2, 0, 0), // State.Explode1
            new StateDef(Sprite.MISL, 32770, 6, null, null, State.Explode3, 0, 0), // State.Explode2
            new StateDef(Sprite.MISL, 32771, 4, null, null, State.Null, 0, 0), // State.Explode3
            new StateDef(Sprite.TFOG, 32768, 6, null, null, State.Tfog01, 0, 0), // State.Tfog
            new StateDef(Sprite.TFOG, 32769, 6, null, null, State.Tfog02, 0, 0), // State.Tfog01
            new StateDef(Sprite.TFOG, 32768, 6, null, null, State.Tfog2, 0, 0), // State.Tfog02
            new StateDef(Sprite.TFOG, 32769, 6, null, null, State.Tfog3, 0, 0), // State.Tfog2
            new StateDef(Sprite.TFOG, 32770, 6, null, null, State.Tfog4, 0, 0), // State.Tfog3
            new StateDef(Sprite.TFOG, 32771, 6, null, null, State.Tfog5, 0, 0), // State.Tfog4
            new StateDef(Sprite.TFOG, 32772, 6, null, null, State.Tfog6, 0, 0), // State.Tfog5
            new StateDef(Sprite.TFOG, 32773, 6, null, null, State.Tfog7, 0, 0), // State.Tfog6
            new StateDef(Sprite.TFOG, 32774, 6, null, null, State.Tfog8, 0, 0), // State.Tfog7
            new StateDef(Sprite.TFOG, 32775, 6, null, null, State.Tfog9, 0, 0), // State.Tfog8
            new StateDef(Sprite.TFOG, 32776, 6, null, null, State.Tfog10, 0, 0), // State.Tfog9
            new StateDef(Sprite.TFOG, 32777, 6, null, null, State.Null, 0, 0), // State.Tfog10
            new StateDef(Sprite.IFOG, 32768, 6, null, null, State.Ifog01, 0, 0), // State.Ifog
            new StateDef(Sprite.IFOG, 32769, 6, null, null, State.Ifog02, 0, 0), // State.Ifog01
            new StateDef(Sprite.IFOG, 32768, 6, null, null, State.Ifog2, 0, 0), // State.Ifog02
            new StateDef(Sprite.IFOG, 32769, 6, null, null, State.Ifog3, 0, 0), // State.Ifog2
            new StateDef(Sprite.IFOG, 32770, 6, null, null, State.Ifog4, 0, 0), // State.Ifog3
            new StateDef(Sprite.IFOG, 32771, 6, null, null, State.Ifog5, 0, 0), // State.Ifog4
            new StateDef(Sprite.IFOG, 32772, 6, null, null, State.Null, 0, 0), // State.Ifog5
            new StateDef(Sprite.PLAY, 0, -1, null, null, State.Null, 0, 0), // State.Play
            new StateDef(Sprite.PLAY, 0, 4, null, null, State.PlayRun2, 0, 0), // State.PlayRun1
            new StateDef(Sprite.PLAY, 1, 4, null, null, State.PlayRun3, 0, 0), // State.PlayRun2
            new StateDef(Sprite.PLAY, 2, 4, null, null, State.PlayRun4, 0, 0), // State.PlayRun3
            new StateDef(Sprite.PLAY, 3, 4, null, null, State.PlayRun1, 0, 0), // State.PlayRun4
            new StateDef(Sprite.PLAY, 4, 12, null, null, State.Play, 0, 0), // State.PlayAtk1
            new StateDef(Sprite.PLAY, 32773, 6, null, null, State.PlayAtk1, 0, 0), // State.PlayAtk2
            new StateDef(Sprite.PLAY, 6, 4, null, null, State.PlayPain2, 0, 0), // State.PlayPain
            new StateDef(Sprite.PLAY, 6, 4, null, MobjActions.Pain, State.Play, 0, 0), // State.PlayPain2
            new StateDef(Sprite.PLAY, 7, 10, null, null, State.PlayDie2, 0, 0), // State.PlayDie1
            new StateDef(Sprite.PLAY, 8, 10, null, MobjActions.PlayerScream, State.PlayDie3, 0, 0), // State.PlayDie2
            new StateDef(Sprite.PLAY, 9, 10, null, MobjActions.Fall, State.PlayDie4, 0, 0), // State.PlayDie3
            new StateDef(Sprite.PLAY, 10, 10, null, null, State.PlayDie5, 0, 0), // State.PlayDie4
            new StateDef(Sprite.PLAY, 11, 10, null, null, State.PlayDie6, 0, 0), // State.PlayDie5
            new StateDef(Sprite.PLAY, 12, 10, null, null, State.PlayDie7, 0, 0), // State.PlayDie6
            new StateDef(Sprite.PLAY, 13, -1, null, null, State.Null, 0, 0), // State.PlayDie7
            new StateDef(Sprite.PLAY, 14, 5, null, null, State.PlayXdie2, 0, 0), // State.PlayXdie1
            new StateDef(Sprite.PLAY, 15, 5, null, MobjActions.XScream, State.PlayXdie3, 0, 0), // State.PlayXdie2
            new StateDef(Sprite.PLAY, 16, 5, null, MobjActions.Fall, State.PlayXdie4, 0, 0), // State.PlayXdie3
            new StateDef(Sprite.PLAY, 17, 5, null, null, State.PlayXdie5, 0, 0), // State.PlayXdie4
            new StateDef(Sprite.PLAY, 18, 5, null, null, State.PlayXdie6, 0, 0), // State.PlayXdie5
            new StateDef(Sprite.PLAY, 19, 5, null, null, State.PlayXdie7, 0, 0), // State.PlayXdie6
            new StateDef(Sprite.PLAY, 20, 5, null, null, State.PlayXdie8, 0, 0), // State.PlayXdie7
            new StateDef(Sprite.PLAY, 21, 5, null, null, State.PlayXdie9, 0, 0), // State.PlayXdie8
            new StateDef(Sprite.PLAY, 22, -1, null, null, State.Null, 0, 0), // State.PlayXdie9
            new StateDef(Sprite.POSS, 0, 10, null, MobjActions.Look, State.PossStnd2, 0, 0), // State.PossStnd
            new StateDef(Sprite.POSS, 1, 10, null, MobjActions.Look, State.PossStnd, 0, 0), // State.PossStnd2
            new StateDef(Sprite.POSS, 0, 4, null, MobjActions.Chase, State.PossRun2, 0, 0), // State.PossRun1
            new StateDef(Sprite.POSS, 0, 4, null, MobjActions.Chase, State.PossRun3, 0, 0), // State.PossRun2
            new StateDef(Sprite.POSS, 1, 4, null, MobjActions.Chase, State.PossRun4, 0, 0), // State.PossRun3
            new StateDef(Sprite.POSS, 1, 4, null, MobjActions.Chase, State.PossRun5, 0, 0), // State.PossRun4
            new StateDef(Sprite.POSS, 2, 4, null, MobjActions.Chase, State.PossRun6, 0, 0), // State.PossRun5
            new StateDef(Sprite.POSS, 2, 4, null, MobjActions.Chase, State.PossRun7, 0, 0), // State.PossRun6
            new StateDef(Sprite.POSS, 3, 4, null, MobjActions.Chase, State.PossRun8, 0, 0), // State.PossRun7
            new StateDef(Sprite.POSS, 3, 4, null, MobjActions.Chase, State.PossRun1, 0, 0), // State.PossRun8
            new StateDef(Sprite.POSS, 4, 10, null, MobjActions.FaceTarget, State.PossAtk2, 0, 0), // State.PossAtk1
            new StateDef(Sprite.POSS, 5, 8, null, MobjActions.PosAttack, State.PossAtk3, 0, 0), // State.PossAtk2
            new StateDef(Sprite.POSS, 4, 8, null, null, State.PossRun1, 0, 0), // State.PossAtk3
            new StateDef(Sprite.POSS, 6, 3, null, null, State.PossPain2, 0, 0), // State.PossPain
            new StateDef(Sprite.POSS, 6, 3, null, MobjActions.Pain, State.PossRun1, 0, 0), // State.PossPain2
            new StateDef(Sprite.POSS, 7, 5, null, null, State.PossDie2, 0, 0), // State.PossDie1
            new StateDef(Sprite.POSS, 8, 5, null, MobjActions.Scream, State.PossDie3, 0, 0), // State.PossDie2
            new StateDef(Sprite.POSS, 9, 5, null, MobjActions.Fall, State.PossDie4, 0, 0), // State.PossDie3
            new StateDef(Sprite.POSS, 10, 5, null, null, State.PossDie5, 0, 0), // State.PossDie4
            new StateDef(Sprite.POSS, 11, -1, null, null, State.Null, 0, 0), // State.PossDie5
            new StateDef(Sprite.POSS, 12, 5, null, null, State.PossXdie2, 0, 0), // State.PossXdie1
            new StateDef(Sprite.POSS, 13, 5, null, MobjActions.XScream, State.PossXdie3, 0, 0), // State.PossXdie2
            new StateDef(Sprite.POSS, 14, 5, null, MobjActions.Fall, State.PossXdie4, 0, 0), // State.PossXdie3
            new StateDef(Sprite.POSS, 15, 5, null, null, State.PossXdie5, 0, 0), // State.PossXdie4
            new StateDef(Sprite.POSS, 16, 5, null, null, State.PossXdie6, 0, 0), // State.PossXdie5
            new StateDef(Sprite.POSS, 17, 5, null, null, State.PossXdie7, 0, 0), // State.PossXdie6
            new StateDef(Sprite.POSS, 18, 5, null, null, State.PossXdie8, 0, 0), // State.PossXdie7
            new StateDef(Sprite.POSS, 19, 5, null, null, State.PossXdie9, 0, 0), // State.PossXdie8
            new StateDef(Sprite.POSS, 20, -1, null, null, State.Null, 0, 0), // State.PossXdie9
            new StateDef(Sprite.POSS, 10, 5, null, null, State.PossRaise2, 0, 0), // State.PossRaise1
            new StateDef(Sprite.POSS, 9, 5, null, null, State.PossRaise3, 0, 0), // State.PossRaise2
            new StateDef(Sprite.POSS, 8, 5, null, null, State.PossRaise4, 0, 0), // State.PossRaise3
            new StateDef(Sprite.POSS, 7, 5, null, null, State.PossRun1, 0, 0), // State.PossRaise4
            new StateDef(Sprite.SPOS, 0, 10, null, MobjActions.Look, State.SposStnd2, 0, 0), // State.SposStnd
            new StateDef(Sprite.SPOS, 1, 10, null, MobjActions.Look, State.SposStnd, 0, 0), // State.SposStnd2
            new StateDef(Sprite.SPOS, 0, 3, null, MobjActions.Chase, State.SposRun2, 0, 0), // State.SposRun1
            new StateDef(Sprite.SPOS, 0, 3, null, MobjActions.Chase, State.SposRun3, 0, 0), // State.SposRun2
            new StateDef(Sprite.SPOS, 1, 3, null, MobjActions.Chase, State.SposRun4, 0, 0), // State.SposRun3
            new StateDef(Sprite.SPOS, 1, 3, null, MobjActions.Chase, State.SposRun5, 0, 0), // State.SposRun4
            new StateDef(Sprite.SPOS, 2, 3, null, MobjActions.Chase, State.SposRun6, 0, 0), // State.SposRun5
            new StateDef(Sprite.SPOS, 2, 3, null, MobjActions.Chase, State.SposRun7, 0, 0), // State.SposRun6
            new StateDef(Sprite.SPOS, 3, 3, null, MobjActions.Chase, State.SposRun8, 0, 0), // State.SposRun7
            new StateDef(Sprite.SPOS, 3, 3, null, MobjActions.Chase, State.SposRun1, 0, 0), // State.SposRun8
            new StateDef(Sprite.SPOS, 4, 10, null, MobjActions.FaceTarget, State.SposAtk2, 0, 0), // State.SposAtk1
            new StateDef(Sprite.SPOS, 32773, 10, null, MobjActions.SPosAttack, State.SposAtk3, 0, 0), // State.SposAtk2
            new StateDef(Sprite.SPOS, 4, 10, null, null, State.SposRun1, 0, 0), // State.SposAtk3
            new StateDef(Sprite.SPOS, 6, 3, null, null, State.SposPain2, 0, 0), // State.SposPain
            new StateDef(Sprite.SPOS, 6, 3, null, MobjActions.Pain, State.SposRun1, 0, 0), // State.SposPain2
            new StateDef(Sprite.SPOS, 7, 5, null, null, State.SposDie2, 0, 0), // State.SposDie1
            new StateDef(Sprite.SPOS, 8, 5, null, MobjActions.Scream, State.SposDie3, 0, 0), // State.SposDie2
            new StateDef(Sprite.SPOS, 9, 5, null, MobjActions.Fall, State.SposDie4, 0, 0), // State.SposDie3
            new StateDef(Sprite.SPOS, 10, 5, null, null, State.SposDie5, 0, 0), // State.SposDie4
            new StateDef(Sprite.SPOS, 11, -1, null, null, State.Null, 0, 0), // State.SposDie5
            new StateDef(Sprite.SPOS, 12, 5, null, null, State.SposXdie2, 0, 0), // State.SposXdie1
            new StateDef(Sprite.SPOS, 13, 5, null, MobjActions.XScream, State.SposXdie3, 0, 0), // State.SposXdie2
            new StateDef(Sprite.SPOS, 14, 5, null, MobjActions.Fall, State.SposXdie4, 0, 0), // State.SposXdie3
            new StateDef(Sprite.SPOS, 15, 5, null, null, State.SposXdie5, 0, 0), // State.SposXdie4
            new StateDef(Sprite.SPOS, 16, 5, null, null, State.SposXdie6, 0, 0), // State.SposXdie5
            new StateDef(Sprite.SPOS, 17, 5, null, null, State.SposXdie7, 0, 0), // State.SposXdie6
            new StateDef(Sprite.SPOS, 18, 5, null, null, State.SposXdie8, 0, 0), // State.SposXdie7
            new StateDef(Sprite.SPOS, 19, 5, null, null, State.SposXdie9, 0, 0), // State.SposXdie8
            new StateDef(Sprite.SPOS, 20, -1, null, null, State.Null, 0, 0), // State.SposXdie9
            new StateDef(Sprite.SPOS, 11, 5, null, null, State.SposRaise2, 0, 0), // State.SposRaise1
            new StateDef(Sprite.SPOS, 10, 5, null, null, State.SposRaise3, 0, 0), // State.SposRaise2
            new StateDef(Sprite.SPOS, 9, 5, null, null, State.SposRaise4, 0, 0), // State.SposRaise3
            new StateDef(Sprite.SPOS, 8, 5, null, null, State.SposRaise5, 0, 0), // State.SposRaise4
            new StateDef(Sprite.SPOS, 7, 5, null, null, State.SposRun1, 0, 0), // State.SposRaise5
            new StateDef(Sprite.VILE, 0, 10, null, MobjActions.Look, State.VileStnd2, 0, 0), // State.VileStnd
            new StateDef(Sprite.VILE, 1, 10, null, MobjActions.Look, State.VileStnd, 0, 0), // State.VileStnd2
            new StateDef(Sprite.VILE, 0, 2, null, MobjActions.VileChase, State.VileRun2, 0, 0), // State.VileRun1
            new StateDef(Sprite.VILE, 0, 2, null, MobjActions.VileChase, State.VileRun3, 0, 0), // State.VileRun2
            new StateDef(Sprite.VILE, 1, 2, null, MobjActions.VileChase, State.VileRun4, 0, 0), // State.VileRun3
            new StateDef(Sprite.VILE, 1, 2, null, MobjActions.VileChase, State.VileRun5, 0, 0), // State.VileRun4
            new StateDef(Sprite.VILE, 2, 2, null, MobjActions.VileChase, State.VileRun6, 0, 0), // State.VileRun5
            new StateDef(Sprite.VILE, 2, 2, null, MobjActions.VileChase, State.VileRun7, 0, 0), // State.VileRun6
            new StateDef(Sprite.VILE, 3, 2, null, MobjActions.VileChase, State.VileRun8, 0, 0), // State.VileRun7
            new StateDef(Sprite.VILE, 3, 2, null, MobjActions.VileChase, State.VileRun9, 0, 0), // State.VileRun8
            new StateDef(Sprite.VILE, 4, 2, null, MobjActions.VileChase, State.VileRun10, 0, 0), // State.VileRun9
            new StateDef(Sprite.VILE, 4, 2, null, MobjActions.VileChase, State.VileRun11, 0, 0), // State.VileRun10
            new StateDef(Sprite.VILE, 5, 2, null, MobjActions.VileChase, State.VileRun12, 0, 0), // State.VileRun11
            new StateDef(Sprite.VILE, 5, 2, null, MobjActions.VileChase, State.VileRun1, 0, 0), // State.VileRun12
            new StateDef(Sprite.VILE, 32774, 0, null, MobjActions.VileStart, State.VileAtk2, 0, 0), // State.VileAtk1
            new StateDef(Sprite.VILE, 32774, 10, null, MobjActions.FaceTarget, State.VileAtk3, 0, 0), // State.VileAtk2
            new StateDef(Sprite.VILE, 32775, 8, null, MobjActions.VileTarget, State.VileAtk4, 0, 0), // State.VileAtk3
            new StateDef(Sprite.VILE, 32776, 8, null, MobjActions.FaceTarget, State.VileAtk5, 0, 0), // State.VileAtk4
            new StateDef(Sprite.VILE, 32777, 8, null, MobjActions.FaceTarget, State.VileAtk6, 0, 0), // State.VileAtk5
            new StateDef(Sprite.VILE, 32778, 8, null, MobjActions.FaceTarget, State.VileAtk7, 0, 0), // State.VileAtk6
            new StateDef(Sprite.VILE, 32779, 8, null, MobjActions.FaceTarget, State.VileAtk8, 0, 0), // State.VileAtk7
            new StateDef(Sprite.VILE, 32780, 8, null, MobjActions.FaceTarget, State.VileAtk9, 0, 0), // State.VileAtk8
            new StateDef(Sprite.VILE, 32781, 8, null, MobjActions.FaceTarget, State.VileAtk10, 0, 0), // State.VileAtk9
            new StateDef(Sprite.VILE, 32782, 8, null, MobjActions.VileAttack, State.VileAtk11, 0, 0), // State.VileAtk10
            new StateDef(Sprite.VILE, 32783, 20, null, null, State.VileRun1, 0, 0), // State.VileAtk11
            new StateDef(Sprite.VILE, 32794, 10, null, null, State.VileHeal2, 0, 0), // State.VileHeal1
            new StateDef(Sprite.VILE, 32795, 10, null, null, State.VileHeal3, 0, 0), // State.VileHeal2
            new StateDef(Sprite.VILE, 32796, 10, null, null, State.VileRun1, 0, 0), // State.VileHeal3
            new StateDef(Sprite.VILE, 16, 5, null, null, State.VilePain2, 0, 0), // State.VilePain
            new StateDef(Sprite.VILE, 16, 5, null, MobjActions.Pain, State.VileRun1, 0, 0), // State.VilePain2
            new StateDef(Sprite.VILE, 16, 7, null, null, State.VileDie2, 0, 0), // State.VileDie1
            new StateDef(Sprite.VILE, 17, 7, null, MobjActions.Scream, State.VileDie3, 0, 0), // State.VileDie2
            new StateDef(Sprite.VILE, 18, 7, null, MobjActions.Fall, State.VileDie4, 0, 0), // State.VileDie3
            new StateDef(Sprite.VILE, 19, 7, null, null, State.VileDie5, 0, 0), // State.VileDie4
            new StateDef(Sprite.VILE, 20, 7, null, null, State.VileDie6, 0, 0), // State.VileDie5
            new StateDef(Sprite.VILE, 21, 7, null, null, State.VileDie7, 0, 0), // State.VileDie6
            new StateDef(Sprite.VILE, 22, 7, null, null, State.VileDie8, 0, 0), // State.VileDie7
            new StateDef(Sprite.VILE, 23, 5, null, null, State.VileDie9, 0, 0), // State.VileDie8
            new StateDef(Sprite.VILE, 24, 5, null, null, State.VileDie10, 0, 0), // State.VileDie9
            new StateDef(Sprite.VILE, 25, -1, null, null, State.Null, 0, 0), // State.VileDie10
            new StateDef(Sprite.FIRE, 32768, 2, null, MobjActions.StartFire, State.Fire2, 0, 0), // State.Fire1
            new StateDef(Sprite.FIRE, 32769, 2, null, MobjActions.Fire, State.Fire3, 0, 0), // State.Fire2
            new StateDef(Sprite.FIRE, 32768, 2, null, MobjActions.Fire, State.Fire4, 0, 0), // State.Fire3
            new StateDef(Sprite.FIRE, 32769, 2, null, MobjActions.Fire, State.Fire5, 0, 0), // State.Fire4
            new StateDef(Sprite.FIRE, 32770, 2, null, MobjActions.FireCrackle, State.Fire6, 0, 0), // State.Fire5
            new StateDef(Sprite.FIRE, 32769, 2, null, MobjActions.Fire, State.Fire7, 0, 0), // State.Fire6
            new StateDef(Sprite.FIRE, 32770, 2, null, MobjActions.Fire, State.Fire8, 0, 0), // State.Fire7
            new StateDef(Sprite.FIRE, 32769, 2, null, MobjActions.Fire, State.Fire9, 0, 0), // State.Fire8
            new StateDef(Sprite.FIRE, 32770, 2, null, MobjActions.Fire, State.Fire10, 0, 0), // State.Fire9
            new StateDef(Sprite.FIRE, 32771, 2, null, MobjActions.Fire, State.Fire11, 0, 0), // State.Fire10
            new StateDef(Sprite.FIRE, 32770, 2, null, MobjActions.Fire, State.Fire12, 0, 0), // State.Fire11
            new StateDef(Sprite.FIRE, 32771, 2, null, MobjActions.Fire, State.Fire13, 0, 0), // State.Fire12
            new StateDef(Sprite.FIRE, 32770, 2, null, MobjActions.Fire, State.Fire14, 0, 0), // State.Fire13
            new StateDef(Sprite.FIRE, 32771, 2, null, MobjActions.Fire, State.Fire15, 0, 0), // State.Fire14
            new StateDef(Sprite.FIRE, 32772, 2, null, MobjActions.Fire, State.Fire16, 0, 0), // State.Fire15
            new StateDef(Sprite.FIRE, 32771, 2, null, MobjActions.Fire, State.Fire17, 0, 0), // State.Fire16
            new StateDef(Sprite.FIRE, 32772, 2, null, MobjActions.Fire, State.Fire18, 0, 0), // State.Fire17
            new StateDef(Sprite.FIRE, 32771, 2, null, MobjActions.Fire, State.Fire19, 0, 0), // State.Fire18
            new StateDef(Sprite.FIRE, 32772, 2, null, MobjActions.FireCrackle, State.Fire20, 0, 0), // State.Fire19
            new StateDef(Sprite.FIRE, 32773, 2, null, MobjActions.Fire, State.Fire21, 0, 0), // State.Fire20
            new StateDef(Sprite.FIRE, 32772, 2, null, MobjActions.Fire, State.Fire22, 0, 0), // State.Fire21
            new StateDef(Sprite.FIRE, 32773, 2, null, MobjActions.Fire, State.Fire23, 0, 0), // State.Fire22
            new StateDef(Sprite.FIRE, 32772, 2, null, MobjActions.Fire, State.Fire24, 0, 0), // State.Fire23
            new StateDef(Sprite.FIRE, 32773, 2, null, MobjActions.Fire, State.Fire25, 0, 0), // State.Fire24
            new StateDef(Sprite.FIRE, 32774, 2, null, MobjActions.Fire, State.Fire26, 0, 0), // State.Fire25
            new StateDef(Sprite.FIRE, 32775, 2, null, MobjActions.Fire, State.Fire27, 0, 0), // State.Fire26
            new StateDef(Sprite.FIRE, 32774, 2, null, MobjActions.Fire, State.Fire28, 0, 0), // State.Fire27
            new StateDef(Sprite.FIRE, 32775, 2, null, MobjActions.Fire, State.Fire29, 0, 0), // State.Fire28
            new StateDef(Sprite.FIRE, 32774, 2, null, MobjActions.Fire, State.Fire30, 0, 0), // State.Fire29
            new StateDef(Sprite.FIRE, 32775, 2, null, MobjActions.Fire, State.Null, 0, 0), // State.Fire30
            new StateDef(Sprite.PUFF, 1, 4, null, null, State.Smoke2, 0, 0), // State.Smoke1
            new StateDef(Sprite.PUFF, 2, 4, null, null, State.Smoke3, 0, 0), // State.Smoke2
            new StateDef(Sprite.PUFF, 1, 4, null, null, State.Smoke4, 0, 0), // State.Smoke3
            new StateDef(Sprite.PUFF, 2, 4, null, null, State.Smoke5, 0, 0), // State.Smoke4
            new StateDef(Sprite.PUFF, 3, 4, null, null, State.Null, 0, 0), // State.Smoke5
            new StateDef(Sprite.FATB, 32768, 2, null, MobjActions.Tracer, State.Tracer2, 0, 0), // State.Tracer
            new StateDef(Sprite.FATB, 32769, 2, null, MobjActions.Tracer, State.Tracer, 0, 0), // State.Tracer2
            new StateDef(Sprite.FBXP, 32768, 8, null, null, State.Traceexp2, 0, 0), // State.Traceexp1
            new StateDef(Sprite.FBXP, 32769, 6, null, null, State.Traceexp3, 0, 0), // State.Traceexp2
            new StateDef(Sprite.FBXP, 32770, 4, null, null, State.Null, 0, 0), // State.Traceexp3
            new StateDef(Sprite.SKEL, 0, 10, null, MobjActions.Look, State.SkelStnd2, 0, 0), // State.SkelStnd
            new StateDef(Sprite.SKEL, 1, 10, null, MobjActions.Look, State.SkelStnd, 0, 0), // State.SkelStnd2
            new StateDef(Sprite.SKEL, 0, 2, null, MobjActions.Chase, State.SkelRun2, 0, 0), // State.SkelRun1
            new StateDef(Sprite.SKEL, 0, 2, null, MobjActions.Chase, State.SkelRun3, 0, 0), // State.SkelRun2
            new StateDef(Sprite.SKEL, 1, 2, null, MobjActions.Chase, State.SkelRun4, 0, 0), // State.SkelRun3
            new StateDef(Sprite.SKEL, 1, 2, null, MobjActions.Chase, State.SkelRun5, 0, 0), // State.SkelRun4
            new StateDef(Sprite.SKEL, 2, 2, null, MobjActions.Chase, State.SkelRun6, 0, 0), // State.SkelRun5
            new StateDef(Sprite.SKEL, 2, 2, null, MobjActions.Chase, State.SkelRun7, 0, 0), // State.SkelRun6
            new StateDef(Sprite.SKEL, 3, 2, null, MobjActions.Chase, State.SkelRun8, 0, 0), // State.SkelRun7
            new StateDef(Sprite.SKEL, 3, 2, null, MobjActions.Chase, State.SkelRun9, 0, 0), // State.SkelRun8
            new StateDef(Sprite.SKEL, 4, 2, null, MobjActions.Chase, State.SkelRun10, 0, 0), // State.SkelRun9
            new StateDef(Sprite.SKEL, 4, 2, null, MobjActions.Chase, State.SkelRun11, 0, 0), // State.SkelRun10
            new StateDef(Sprite.SKEL, 5, 2, null, MobjActions.Chase, State.SkelRun12, 0, 0), // State.SkelRun11
            new StateDef(Sprite.SKEL, 5, 2, null, MobjActions.Chase, State.SkelRun1, 0, 0), // State.SkelRun12
            new StateDef(Sprite.SKEL, 6, 0, null, MobjActions.FaceTarget, State.SkelFist2, 0, 0), // State.SkelFist1
            new StateDef(Sprite.SKEL, 6, 6, null, MobjActions.SkelWhoosh, State.SkelFist3, 0, 0), // State.SkelFist2
            new StateDef(Sprite.SKEL, 7, 6, null, MobjActions.FaceTarget, State.SkelFist4, 0, 0), // State.SkelFist3
            new StateDef(Sprite.SKEL, 8, 6, null, MobjActions.SkelFist, State.SkelRun1, 0, 0), // State.SkelFist4
            new StateDef(Sprite.SKEL, 32777, 0, null, MobjActions.FaceTarget, State.SkelMiss2, 0, 0), // State.SkelMiss1
            new StateDef(Sprite.SKEL, 32777, 10, null, MobjActions.FaceTarget, State.SkelMiss3, 0, 0), // State.SkelMiss2
            new StateDef(Sprite.SKEL, 10, 10, null, MobjActions.SkelMissile, State.SkelMiss4, 0, 0), // State.SkelMiss3
            new StateDef(Sprite.SKEL, 10, 10, null, MobjActions.FaceTarget, State.SkelRun1, 0, 0), // State.SkelMiss4
            new StateDef(Sprite.SKEL, 11, 5, null, null, State.SkelPain2, 0, 0), // State.SkelPain
            new StateDef(Sprite.SKEL, 11, 5, null, MobjActions.Pain, State.SkelRun1, 0, 0), // State.SkelPain2
            new StateDef(Sprite.SKEL, 11, 7, null, null, State.SkelDie2, 0, 0), // State.SkelDie1
            new StateDef(Sprite.SKEL, 12, 7, null, null, State.SkelDie3, 0, 0), // State.SkelDie2
            new StateDef(Sprite.SKEL, 13, 7, null, MobjActions.Scream, State.SkelDie4, 0, 0), // State.SkelDie3
            new StateDef(Sprite.SKEL, 14, 7, null, MobjActions.Fall, State.SkelDie5, 0, 0), // State.SkelDie4
            new StateDef(Sprite.SKEL, 15, 7, null, null, State.SkelDie6, 0, 0), // State.SkelDie5
            new StateDef(Sprite.SKEL, 16, -1, null, null, State.Null, 0, 0), // State.SkelDie6
            new StateDef(Sprite.SKEL, 16, 5, null, null, State.SkelRaise2, 0, 0), // State.SkelRaise1
            new StateDef(Sprite.SKEL, 15, 5, null, null, State.SkelRaise3, 0, 0), // State.SkelRaise2
            new StateDef(Sprite.SKEL, 14, 5, null, null, State.SkelRaise4, 0, 0), // State.SkelRaise3
            new StateDef(Sprite.SKEL, 13, 5, null, null, State.SkelRaise5, 0, 0), // State.SkelRaise4
            new StateDef(Sprite.SKEL, 12, 5, null, null, State.SkelRaise6, 0, 0), // State.SkelRaise5
            new StateDef(Sprite.SKEL, 11, 5, null, null, State.SkelRun1, 0, 0), // State.SkelRaise6
            new StateDef(Sprite.MANF, 32768, 4, null, null, State.Fatshot2, 0, 0), // State.Fatshot1
            new StateDef(Sprite.MANF, 32769, 4, null, null, State.Fatshot1, 0, 0), // State.Fatshot2
            new StateDef(Sprite.MISL, 32769, 8, null, null, State.Fatshotx2, 0, 0), // State.Fatshotx1
            new StateDef(Sprite.MISL, 32770, 6, null, null, State.Fatshotx3, 0, 0), // State.Fatshotx2
            new StateDef(Sprite.MISL, 32771, 4, null, null, State.Null, 0, 0), // State.Fatshotx3
            new StateDef(Sprite.FATT, 0, 15, null, MobjActions.Look, State.FattStnd2, 0, 0), // State.FattStnd
            new StateDef(Sprite.FATT, 1, 15, null, MobjActions.Look, State.FattStnd, 0, 0), // State.FattStnd2
            new StateDef(Sprite.FATT, 0, 4, null, MobjActions.Chase, State.FattRun2, 0, 0), // State.FattRun1
            new StateDef(Sprite.FATT, 0, 4, null, MobjActions.Chase, State.FattRun3, 0, 0), // State.FattRun2
            new StateDef(Sprite.FATT, 1, 4, null, MobjActions.Chase, State.FattRun4, 0, 0), // State.FattRun3
            new StateDef(Sprite.FATT, 1, 4, null, MobjActions.Chase, State.FattRun5, 0, 0), // State.FattRun4
            new StateDef(Sprite.FATT, 2, 4, null, MobjActions.Chase, State.FattRun6, 0, 0), // State.FattRun5
            new StateDef(Sprite.FATT, 2, 4, null, MobjActions.Chase, State.FattRun7, 0, 0), // State.FattRun6
            new StateDef(Sprite.FATT, 3, 4, null, MobjActions.Chase, State.FattRun8, 0, 0), // State.FattRun7
            new StateDef(Sprite.FATT, 3, 4, null, MobjActions.Chase, State.FattRun9, 0, 0), // State.FattRun8
            new StateDef(Sprite.FATT, 4, 4, null, MobjActions.Chase, State.FattRun10, 0, 0), // State.FattRun9
            new StateDef(Sprite.FATT, 4, 4, null, MobjActions.Chase, State.FattRun11, 0, 0), // State.FattRun10
            new StateDef(Sprite.FATT, 5, 4, null, MobjActions.Chase, State.FattRun12, 0, 0), // State.FattRun11
            new StateDef(Sprite.FATT, 5, 4, null, MobjActions.Chase, State.FattRun1, 0, 0), // State.FattRun12
            new StateDef(Sprite.FATT, 6, 20, null, MobjActions.FatRaise, State.FattAtk2, 0, 0), // State.FattAtk1
            new StateDef(Sprite.FATT, 32775, 10, null, MobjActions.FatAttack1, State.FattAtk3, 0, 0), // State.FattAtk2
            new StateDef(Sprite.FATT, 8, 5, null, MobjActions.FaceTarget, State.FattAtk4, 0, 0), // State.FattAtk3
            new StateDef(Sprite.FATT, 6, 5, null, MobjActions.FaceTarget, State.FattAtk5, 0, 0), // State.FattAtk4
            new StateDef(Sprite.FATT, 32775, 10, null, MobjActions.FatAttack2, State.FattAtk6, 0, 0), // State.FattAtk5
            new StateDef(Sprite.FATT, 8, 5, null, MobjActions.FaceTarget, State.FattAtk7, 0, 0), // State.FattAtk6
            new StateDef(Sprite.FATT, 6, 5, null, MobjActions.FaceTarget, State.FattAtk8, 0, 0), // State.FattAtk7
            new StateDef(Sprite.FATT, 32775, 10, null, MobjActions.FatAttack3, State.FattAtk9, 0, 0), // State.FattAtk8
            new StateDef(Sprite.FATT, 8, 5, null, MobjActions.FaceTarget, State.FattAtk10, 0, 0), // State.FattAtk9
            new StateDef(Sprite.FATT, 6, 5, null, MobjActions.FaceTarget, State.FattRun1, 0, 0), // State.FattAtk10
            new StateDef(Sprite.FATT, 9, 3, null, null, State.FattPain2, 0, 0), // State.FattPain
            new StateDef(Sprite.FATT, 9, 3, null, MobjActions.Pain, State.FattRun1, 0, 0), // State.FattPain2
            new StateDef(Sprite.FATT, 10, 6, null, null, State.FattDie2, 0, 0), // State.FattDie1
            new StateDef(Sprite.FATT, 11, 6, null, MobjActions.Scream, State.FattDie3, 0, 0), // State.FattDie2
            new StateDef(Sprite.FATT, 12, 6, null, MobjActions.Fall, State.FattDie4, 0, 0), // State.FattDie3
            new StateDef(Sprite.FATT, 13, 6, null, null, State.FattDie5, 0, 0), // State.FattDie4
            new StateDef(Sprite.FATT, 14, 6, null, null, State.FattDie6, 0, 0), // State.FattDie5
            new StateDef(Sprite.FATT, 15, 6, null, null, State.FattDie7, 0, 0), // State.FattDie6
            new StateDef(Sprite.FATT, 16, 6, null, null, State.FattDie8, 0, 0), // State.FattDie7
            new StateDef(Sprite.FATT, 17, 6, null, null, State.FattDie9, 0, 0), // State.FattDie8
            new StateDef(Sprite.FATT, 18, 6, null, null, State.FattDie10, 0, 0), // State.FattDie9
            new StateDef(Sprite.FATT, 19, -1, null, MobjActions.BossDeath, State.Null, 0, 0), // State.FattDie10
            new StateDef(Sprite.FATT, 17, 5, null, null, State.FattRaise2, 0, 0), // State.FattRaise1
            new StateDef(Sprite.FATT, 16, 5, null, null, State.FattRaise3, 0, 0), // State.FattRaise2
            new StateDef(Sprite.FATT, 15, 5, null, null, State.FattRaise4, 0, 0), // State.FattRaise3
            new StateDef(Sprite.FATT, 14, 5, null, null, State.FattRaise5, 0, 0), // State.FattRaise4
            new StateDef(Sprite.FATT, 13, 5, null, null, State.FattRaise6, 0, 0), // State.FattRaise5
            new StateDef(Sprite.FATT, 12, 5, null, null, State.FattRaise7, 0, 0), // State.FattRaise6
            new StateDef(Sprite.FATT, 11, 5, null, null, State.FattRaise8, 0, 0), // State.FattRaise7
            new StateDef(Sprite.FATT, 10, 5, null, null, State.FattRun1, 0, 0), // State.FattRaise8
            new StateDef(Sprite.CPOS, 0, 10, null, MobjActions.Look, State.CposStnd2, 0, 0), // State.CposStnd
            new StateDef(Sprite.CPOS, 1, 10, null, MobjActions.Look, State.CposStnd, 0, 0), // State.CposStnd2
            new StateDef(Sprite.CPOS, 0, 3, null, MobjActions.Chase, State.CposRun2, 0, 0), // State.CposRun1
            new StateDef(Sprite.CPOS, 0, 3, null, MobjActions.Chase, State.CposRun3, 0, 0), // State.CposRun2
            new StateDef(Sprite.CPOS, 1, 3, null, MobjActions.Chase, State.CposRun4, 0, 0), // State.CposRun3
            new StateDef(Sprite.CPOS, 1, 3, null, MobjActions.Chase, State.CposRun5, 0, 0), // State.CposRun4
            new StateDef(Sprite.CPOS, 2, 3, null, MobjActions.Chase, State.CposRun6, 0, 0), // State.CposRun5
            new StateDef(Sprite.CPOS, 2, 3, null, MobjActions.Chase, State.CposRun7, 0, 0), // State.CposRun6
            new StateDef(Sprite.CPOS, 3, 3, null, MobjActions.Chase, State.CposRun8, 0, 0), // State.CposRun7
            new StateDef(Sprite.CPOS, 3, 3, null, MobjActions.Chase, State.CposRun1, 0, 0), // State.CposRun8
            new StateDef(Sprite.CPOS, 4, 10, null, MobjActions.FaceTarget, State.CposAtk2, 0, 0), // State.CposAtk1
            new StateDef(Sprite.CPOS, 32773, 4, null, MobjActions.CPosAttack, State.CposAtk3, 0, 0), // State.CposAtk2
            new StateDef(Sprite.CPOS, 32772, 4, null, MobjActions.CPosAttack, State.CposAtk4, 0, 0), // State.CposAtk3
            new StateDef(Sprite.CPOS, 5, 1, null, MobjActions.CPosRefire, State.CposAtk2, 0, 0), // State.CposAtk4
            new StateDef(Sprite.CPOS, 6, 3, null, null, State.CposPain2, 0, 0), // State.CposPain
            new StateDef(Sprite.CPOS, 6, 3, null, MobjActions.Pain, State.CposRun1, 0, 0), // State.CposPain2
            new StateDef(Sprite.CPOS, 7, 5, null, null, State.CposDie2, 0, 0), // State.CposDie1
            new StateDef(Sprite.CPOS, 8, 5, null, MobjActions.Scream, State.CposDie3, 0, 0), // State.CposDie2
            new StateDef(Sprite.CPOS, 9, 5, null, MobjActions.Fall, State.CposDie4, 0, 0), // State.CposDie3
            new StateDef(Sprite.CPOS, 10, 5, null, null, State.CposDie5, 0, 0), // State.CposDie4
            new StateDef(Sprite.CPOS, 11, 5, null, null, State.CposDie6, 0, 0), // State.CposDie5
            new StateDef(Sprite.CPOS, 12, 5, null, null, State.CposDie7, 0, 0), // State.CposDie6
            new StateDef(Sprite.CPOS, 13, -1, null, null, State.Null, 0, 0), // State.CposDie7
            new StateDef(Sprite.CPOS, 14, 5, null, null, State.CposXdie2, 0, 0), // State.CposXdie1
            new StateDef(Sprite.CPOS, 15, 5, null, MobjActions.XScream, State.CposXdie3, 0, 0), // State.CposXdie2
            new StateDef(Sprite.CPOS, 16, 5, null, MobjActions.Fall, State.CposXdie4, 0, 0), // State.CposXdie3
            new StateDef(Sprite.CPOS, 17, 5, null, null, State.CposXdie5, 0, 0), // State.CposXdie4
            new StateDef(Sprite.CPOS, 18, 5, null, null, State.CposXdie6, 0, 0), // State.CposXdie5
            new StateDef(Sprite.CPOS, 19, -1, null, null, State.Null, 0, 0), // State.CposXdie6
            new StateDef(Sprite.CPOS, 13, 5, null, null, State.CposRaise2, 0, 0), // State.CposRaise1
            new StateDef(Sprite.CPOS, 12, 5, null, null, State.CposRaise3, 0, 0), // State.CposRaise2
            new StateDef(Sprite.CPOS, 11, 5, null, null, State.CposRaise4, 0, 0), // State.CposRaise3
            new StateDef(Sprite.CPOS, 10, 5, null, null, State.CposRaise5, 0, 0), // State.CposRaise4
            new StateDef(Sprite.CPOS, 9, 5, null, null, State.CposRaise6, 0, 0), // State.CposRaise5
            new StateDef(Sprite.CPOS, 8, 5, null, null, State.CposRaise7, 0, 0), // State.CposRaise6
            new StateDef(Sprite.CPOS, 7, 5, null, null, State.CposRun1, 0, 0), // State.CposRaise7
            new StateDef(Sprite.TROO, 0, 10, null, MobjActions.Look, State.TrooStnd2, 0, 0), // State.TrooStnd
            new StateDef(Sprite.TROO, 1, 10, null, MobjActions.Look, State.TrooStnd, 0, 0), // State.TrooStnd2
            new StateDef(Sprite.TROO, 0, 3, null, MobjActions.Chase, State.TrooRun2, 0, 0), // State.TrooRun1
            new StateDef(Sprite.TROO, 0, 3, null, MobjActions.Chase, State.TrooRun3, 0, 0), // State.TrooRun2
            new StateDef(Sprite.TROO, 1, 3, null, MobjActions.Chase, State.TrooRun4, 0, 0), // State.TrooRun3
            new StateDef(Sprite.TROO, 1, 3, null, MobjActions.Chase, State.TrooRun5, 0, 0), // State.TrooRun4
            new StateDef(Sprite.TROO, 2, 3, null, MobjActions.Chase, State.TrooRun6, 0, 0), // State.TrooRun5
            new StateDef(Sprite.TROO, 2, 3, null, MobjActions.Chase, State.TrooRun7, 0, 0), // State.TrooRun6
            new StateDef(Sprite.TROO, 3, 3, null, MobjActions.Chase, State.TrooRun8, 0, 0), // State.TrooRun7
            new StateDef(Sprite.TROO, 3, 3, null, MobjActions.Chase, State.TrooRun1, 0, 0), // State.TrooRun8
            new StateDef(Sprite.TROO, 4, 8, null, MobjActions.FaceTarget, State.TrooAtk2, 0, 0), // State.TrooAtk1
            new StateDef(Sprite.TROO, 5, 8, null, MobjActions.FaceTarget, State.TrooAtk3, 0, 0), // State.TrooAtk2
            new StateDef(Sprite.TROO, 6, 6, null, MobjActions.TroopAttack, State.TrooRun1, 0, 0), // State.TrooAtk3
            new StateDef(Sprite.TROO, 7, 2, null, null, State.TrooPain2, 0, 0), // State.TrooPain
            new StateDef(Sprite.TROO, 7, 2, null, MobjActions.Pain, State.TrooRun1, 0, 0), // State.TrooPain2
            new StateDef(Sprite.TROO, 8, 8, null, null, State.TrooDie2, 0, 0), // State.TrooDie1
            new StateDef(Sprite.TROO, 9, 8, null, MobjActions.Scream, State.TrooDie3, 0, 0), // State.TrooDie2
            new StateDef(Sprite.TROO, 10, 6, null, null, State.TrooDie4, 0, 0), // State.TrooDie3
            new StateDef(Sprite.TROO, 11, 6, null, MobjActions.Fall, State.TrooDie5, 0, 0), // State.TrooDie4
            new StateDef(Sprite.TROO, 12, -1, null, null, State.Null, 0, 0), // State.TrooDie5
            new StateDef(Sprite.TROO, 13, 5, null, null, State.TrooXdie2, 0, 0), // State.TrooXdie1
            new StateDef(Sprite.TROO, 14, 5, null, MobjActions.XScream, State.TrooXdie3, 0, 0), // State.TrooXdie2
            new StateDef(Sprite.TROO, 15, 5, null, null, State.TrooXdie4, 0, 0), // State.TrooXdie3
            new StateDef(Sprite.TROO, 16, 5, null, MobjActions.Fall, State.TrooXdie5, 0, 0), // State.TrooXdie4
            new StateDef(Sprite.TROO, 17, 5, null, null, State.TrooXdie6, 0, 0), // State.TrooXdie5
            new StateDef(Sprite.TROO, 18, 5, null, null, State.TrooXdie7, 0, 0), // State.TrooXdie6
            new StateDef(Sprite.TROO, 19, 5, null, null, State.TrooXdie8, 0, 0), // State.TrooXdie7
            new StateDef(Sprite.TROO, 20, -1, null, null, State.Null, 0, 0), // State.TrooXdie8
            new StateDef(Sprite.TROO, 12, 8, null, null, State.TrooRaise2, 0, 0), // State.TrooRaise1
            new StateDef(Sprite.TROO, 11, 8, null, null, State.TrooRaise3, 0, 0), // State.TrooRaise2
            new StateDef(Sprite.TROO, 10, 6, null, null, State.TrooRaise4, 0, 0), // State.TrooRaise3
            new StateDef(Sprite.TROO, 9, 6, null, null, State.TrooRaise5, 0, 0), // State.TrooRaise4
            new StateDef(Sprite.TROO, 8, 6, null, null, State.TrooRun1, 0, 0), // State.TrooRaise5
            new StateDef(Sprite.SARG, 0, 10, null, MobjActions.Look, State.SargStnd2, 0, 0), // State.SargStnd
            new StateDef(Sprite.SARG, 1, 10, null, MobjActions.Look, State.SargStnd, 0, 0), // State.SargStnd2
            new StateDef(Sprite.SARG, 0, 2, null, MobjActions.Chase, State.SargRun2, 0, 0), // State.SargRun1
            new StateDef(Sprite.SARG, 0, 2, null, MobjActions.Chase, State.SargRun3, 0, 0), // State.SargRun2
            new StateDef(Sprite.SARG, 1, 2, null, MobjActions.Chase, State.SargRun4, 0, 0), // State.SargRun3
            new StateDef(Sprite.SARG, 1, 2, null, MobjActions.Chase, State.SargRun5, 0, 0), // State.SargRun4
            new StateDef(Sprite.SARG, 2, 2, null, MobjActions.Chase, State.SargRun6, 0, 0), // State.SargRun5
            new StateDef(Sprite.SARG, 2, 2, null, MobjActions.Chase, State.SargRun7, 0, 0), // State.SargRun6
            new StateDef(Sprite.SARG, 3, 2, null, MobjActions.Chase, State.SargRun8, 0, 0), // State.SargRun7
            new StateDef(Sprite.SARG, 3, 2, null, MobjActions.Chase, State.SargRun1, 0, 0), // State.SargRun8
            new StateDef(Sprite.SARG, 4, 8, null, MobjActions.FaceTarget, State.SargAtk2, 0, 0), // State.SargAtk1
            new StateDef(Sprite.SARG, 5, 8, null, MobjActions.FaceTarget, State.SargAtk3, 0, 0), // State.SargAtk2
            new StateDef(Sprite.SARG, 6, 8, null, MobjActions.SargAttack, State.SargRun1, 0, 0), // State.SargAtk3
            new StateDef(Sprite.SARG, 7, 2, null, null, State.SargPain2, 0, 0), // State.SargPain
            new StateDef(Sprite.SARG, 7, 2, null, MobjActions.Pain, State.SargRun1, 0, 0), // State.SargPain2
            new StateDef(Sprite.SARG, 8, 8, null, null, State.SargDie2, 0, 0), // State.SargDie1
            new StateDef(Sprite.SARG, 9, 8, null, MobjActions.Scream, State.SargDie3, 0, 0), // State.SargDie2
            new StateDef(Sprite.SARG, 10, 4, null, null, State.SargDie4, 0, 0), // State.SargDie3
            new StateDef(Sprite.SARG, 11, 4, null, MobjActions.Fall, State.SargDie5, 0, 0), // State.SargDie4
            new StateDef(Sprite.SARG, 12, 4, null, null, State.SargDie6, 0, 0), // State.SargDie5
            new StateDef(Sprite.SARG, 13, -1, null, null, State.Null, 0, 0), // State.SargDie6
            new StateDef(Sprite.SARG, 13, 5, null, null, State.SargRaise2, 0, 0), // State.SargRaise1
            new StateDef(Sprite.SARG, 12, 5, null, null, State.SargRaise3, 0, 0), // State.SargRaise2
            new StateDef(Sprite.SARG, 11, 5, null, null, State.SargRaise4, 0, 0), // State.SargRaise3
            new StateDef(Sprite.SARG, 10, 5, null, null, State.SargRaise5, 0, 0), // State.SargRaise4
            new StateDef(Sprite.SARG, 9, 5, null, null, State.SargRaise6, 0, 0), // State.SargRaise5
            new StateDef(Sprite.SARG, 8, 5, null, null, State.SargRun1, 0, 0), // State.SargRaise6
            new StateDef(Sprite.HEAD, 0, 10, null, MobjActions.Look, State.HeadStnd, 0, 0), // State.HeadStnd
            new StateDef(Sprite.HEAD, 0, 3, null, MobjActions.Chase, State.HeadRun1, 0, 0), // State.HeadRun1
            new StateDef(Sprite.HEAD, 1, 5, null, MobjActions.FaceTarget, State.HeadAtk2, 0, 0), // State.HeadAtk1
            new StateDef(Sprite.HEAD, 2, 5, null, MobjActions.FaceTarget, State.HeadAtk3, 0, 0), // State.HeadAtk2
            new StateDef(Sprite.HEAD, 32771, 5, null, MobjActions.HeadAttack, State.HeadRun1, 0, 0), // State.HeadAtk3
            new StateDef(Sprite.HEAD, 4, 3, null, null, State.HeadPain2, 0, 0), // State.HeadPain
            new StateDef(Sprite.HEAD, 4, 3, null, MobjActions.Pain, State.HeadPain3, 0, 0), // State.HeadPain2
            new StateDef(Sprite.HEAD, 5, 6, null, null, State.HeadRun1, 0, 0), // State.HeadPain3
            new StateDef(Sprite.HEAD, 6, 8, null, null, State.HeadDie2, 0, 0), // State.HeadDie1
            new StateDef(Sprite.HEAD, 7, 8, null, MobjActions.Scream, State.HeadDie3, 0, 0), // State.HeadDie2
            new StateDef(Sprite.HEAD, 8, 8, null, null, State.HeadDie4, 0, 0), // State.HeadDie3
            new StateDef(Sprite.HEAD, 9, 8, null, null, State.HeadDie5, 0, 0), // State.HeadDie4
            new StateDef(Sprite.HEAD, 10, 8, null, MobjActions.Fall, State.HeadDie6, 0, 0), // State.HeadDie5
            new StateDef(Sprite.HEAD, 11, -1, null, null, State.Null, 0, 0), // State.HeadDie6
            new StateDef(Sprite.HEAD, 11, 8, null, null, State.HeadRaise2, 0, 0), // State.HeadRaise1
            new StateDef(Sprite.HEAD, 10, 8, null, null, State.HeadRaise3, 0, 0), // State.HeadRaise2
            new StateDef(Sprite.HEAD, 9, 8, null, null, State.HeadRaise4, 0, 0), // State.HeadRaise3
            new StateDef(Sprite.HEAD, 8, 8, null, null, State.HeadRaise5, 0, 0), // State.HeadRaise4
            new StateDef(Sprite.HEAD, 7, 8, null, null, State.HeadRaise6, 0, 0), // State.HeadRaise5
            new StateDef(Sprite.HEAD, 6, 8, null, null, State.HeadRun1, 0, 0), // State.HeadRaise6
            new StateDef(Sprite.BAL7, 32768, 4, null, null, State.Brball2, 0, 0), // State.Brball1
            new StateDef(Sprite.BAL7, 32769, 4, null, null, State.Brball1, 0, 0), // State.Brball2
            new StateDef(Sprite.BAL7, 32770, 6, null, null, State.Brballx2, 0, 0), // State.Brballx1
            new StateDef(Sprite.BAL7, 32771, 6, null, null, State.Brballx3, 0, 0), // State.Brballx2
            new StateDef(Sprite.BAL7, 32772, 6, null, null, State.Null, 0, 0), // State.Brballx3
            new StateDef(Sprite.BOSS, 0, 10, null, MobjActions.Look, State.BossStnd2, 0, 0), // State.BossStnd
            new StateDef(Sprite.BOSS, 1, 10, null, MobjActions.Look, State.BossStnd, 0, 0), // State.BossStnd2
            new StateDef(Sprite.BOSS, 0, 3, null, MobjActions.Chase, State.BossRun2, 0, 0), // State.BossRun1
            new StateDef(Sprite.BOSS, 0, 3, null, MobjActions.Chase, State.BossRun3, 0, 0), // State.BossRun2
            new StateDef(Sprite.BOSS, 1, 3, null, MobjActions.Chase, State.BossRun4, 0, 0), // State.BossRun3
            new StateDef(Sprite.BOSS, 1, 3, null, MobjActions.Chase, State.BossRun5, 0, 0), // State.BossRun4
            new StateDef(Sprite.BOSS, 2, 3, null, MobjActions.Chase, State.BossRun6, 0, 0), // State.BossRun5
            new StateDef(Sprite.BOSS, 2, 3, null, MobjActions.Chase, State.BossRun7, 0, 0), // State.BossRun6
            new StateDef(Sprite.BOSS, 3, 3, null, MobjActions.Chase, State.BossRun8, 0, 0), // State.BossRun7
            new StateDef(Sprite.BOSS, 3, 3, null, MobjActions.Chase, State.BossRun1, 0, 0), // State.BossRun8
            new StateDef(Sprite.BOSS, 4, 8, null, MobjActions.FaceTarget, State.BossAtk2, 0, 0), // State.BossAtk1
            new StateDef(Sprite.BOSS, 5, 8, null, MobjActions.FaceTarget, State.BossAtk3, 0, 0), // State.BossAtk2
            new StateDef(Sprite.BOSS, 6, 8, null, MobjActions.BruisAttack, State.BossRun1, 0, 0), // State.BossAtk3
            new StateDef(Sprite.BOSS, 7, 2, null, null, State.BossPain2, 0, 0), // State.BossPain
            new StateDef(Sprite.BOSS, 7, 2, null, MobjActions.Pain, State.BossRun1, 0, 0), // State.BossPain2
            new StateDef(Sprite.BOSS, 8, 8, null, null, State.BossDie2, 0, 0), // State.BossDie1
            new StateDef(Sprite.BOSS, 9, 8, null, MobjActions.Scream, State.BossDie3, 0, 0), // State.BossDie2
            new StateDef(Sprite.BOSS, 10, 8, null, null, State.BossDie4, 0, 0), // State.BossDie3
            new StateDef(Sprite.BOSS, 11, 8, null, MobjActions.Fall, State.BossDie5, 0, 0), // State.BossDie4
            new StateDef(Sprite.BOSS, 12, 8, null, null, State.BossDie6, 0, 0), // State.BossDie5
            new StateDef(Sprite.BOSS, 13, 8, null, null, State.BossDie7, 0, 0), // State.BossDie6
            new StateDef(Sprite.BOSS, 14, -1, null, MobjActions.BossDeath, State.Null, 0, 0), // State.BossDie7
            new StateDef(Sprite.BOSS, 14, 8, null, null, State.BossRaise2, 0, 0), // State.BossRaise1
            new StateDef(Sprite.BOSS, 13, 8, null, null, State.BossRaise3, 0, 0), // State.BossRaise2
            new StateDef(Sprite.BOSS, 12, 8, null, null, State.BossRaise4, 0, 0), // State.BossRaise3
            new StateDef(Sprite.BOSS, 11, 8, null, null, State.BossRaise5, 0, 0), // State.BossRaise4
            new StateDef(Sprite.BOSS, 10, 8, null, null, State.BossRaise6, 0, 0), // State.BossRaise5
            new StateDef(Sprite.BOSS, 9, 8, null, null, State.BossRaise7, 0, 0), // State.BossRaise6
            new StateDef(Sprite.BOSS, 8, 8, null, null, State.BossRun1, 0, 0), // State.BossRaise7
            new StateDef(Sprite.BOS2, 0, 10, null, MobjActions.Look, State.Bos2Stnd2, 0, 0), // State.Bos2Stnd
            new StateDef(Sprite.BOS2, 1, 10, null, MobjActions.Look, State.Bos2Stnd, 0, 0), // State.Bos2Stnd2
            new StateDef(Sprite.BOS2, 0, 3, null, MobjActions.Chase, State.Bos2Run2, 0, 0), // State.Bos2Run1
            new StateDef(Sprite.BOS2, 0, 3, null, MobjActions.Chase, State.Bos2Run3, 0, 0), // State.Bos2Run2
            new StateDef(Sprite.BOS2, 1, 3, null, MobjActions.Chase, State.Bos2Run4, 0, 0), // State.Bos2Run3
            new StateDef(Sprite.BOS2, 1, 3, null, MobjActions.Chase, State.Bos2Run5, 0, 0), // State.Bos2Run4
            new StateDef(Sprite.BOS2, 2, 3, null, MobjActions.Chase, State.Bos2Run6, 0, 0), // State.Bos2Run5
            new StateDef(Sprite.BOS2, 2, 3, null, MobjActions.Chase, State.Bos2Run7, 0, 0), // State.Bos2Run6
            new StateDef(Sprite.BOS2, 3, 3, null, MobjActions.Chase, State.Bos2Run8, 0, 0), // State.Bos2Run7
            new StateDef(Sprite.BOS2, 3, 3, null, MobjActions.Chase, State.Bos2Run1, 0, 0), // State.Bos2Run8
            new StateDef(Sprite.BOS2, 4, 8, null, MobjActions.FaceTarget, State.Bos2Atk2, 0, 0), // State.Bos2Atk1
            new StateDef(Sprite.BOS2, 5, 8, null, MobjActions.FaceTarget, State.Bos2Atk3, 0, 0), // State.Bos2Atk2
            new StateDef(Sprite.BOS2, 6, 8, null, MobjActions.BruisAttack, State.Bos2Run1, 0, 0), // State.Bos2Atk3
            new StateDef(Sprite.BOS2, 7, 2, null, null, State.Bos2Pain2, 0, 0), // State.Bos2Pain
            new StateDef(Sprite.BOS2, 7, 2, null, MobjActions.Pain, State.Bos2Run1, 0, 0), // State.Bos2Pain2
            new StateDef(Sprite.BOS2, 8, 8, null, null, State.Bos2Die2, 0, 0), // State.Bos2Die1
            new StateDef(Sprite.BOS2, 9, 8, null, MobjActions.Scream, State.Bos2Die3, 0, 0), // State.Bos2Die2
            new StateDef(Sprite.BOS2, 10, 8, null, null, State.Bos2Die4, 0, 0), // State.Bos2Die3
            new StateDef(Sprite.BOS2, 11, 8, null, MobjActions.Fall, State.Bos2Die5, 0, 0), // State.Bos2Die4
            new StateDef(Sprite.BOS2, 12, 8, null, null, State.Bos2Die6, 0, 0), // State.Bos2Die5
            new StateDef(Sprite.BOS2, 13, 8, null, null, State.Bos2Die7, 0, 0), // State.Bos2Die6
            new StateDef(Sprite.BOS2, 14, -1, null, null, State.Null, 0, 0), // State.Bos2Die7
            new StateDef(Sprite.BOS2, 14, 8, null, null, State.Bos2Raise2, 0, 0), // State.Bos2Raise1
            new StateDef(Sprite.BOS2, 13, 8, null, null, State.Bos2Raise3, 0, 0), // State.Bos2Raise2
            new StateDef(Sprite.BOS2, 12, 8, null, null, State.Bos2Raise4, 0, 0), // State.Bos2Raise3
            new StateDef(Sprite.BOS2, 11, 8, null, null, State.Bos2Raise5, 0, 0), // State.Bos2Raise4
            new StateDef(Sprite.BOS2, 10, 8, null, null, State.Bos2Raise6, 0, 0), // State.Bos2Raise5
            new StateDef(Sprite.BOS2, 9, 8, null, null, State.Bos2Raise7, 0, 0), // State.Bos2Raise6
            new StateDef(Sprite.BOS2, 8, 8, null, null, State.Bos2Run1, 0, 0), // State.Bos2Raise7
            new StateDef(Sprite.SKUL, 32768, 10, null, MobjActions.Look, State.SkullStnd2, 0, 0), // State.SkullStnd
            new StateDef(Sprite.SKUL, 32769, 10, null, MobjActions.Look, State.SkullStnd, 0, 0), // State.SkullStnd2
            new StateDef(Sprite.SKUL, 32768, 6, null, MobjActions.Chase, State.SkullRun2, 0, 0), // State.SkullRun1
            new StateDef(Sprite.SKUL, 32769, 6, null, MobjActions.Chase, State.SkullRun1, 0, 0), // State.SkullRun2
            new StateDef(Sprite.SKUL, 32770, 10, null, MobjActions.FaceTarget, State.SkullAtk2, 0, 0), // State.SkullAtk1
            new StateDef(Sprite.SKUL, 32771, 4, null, MobjActions.SkullAttack, State.SkullAtk3, 0, 0), // State.SkullAtk2
            new StateDef(Sprite.SKUL, 32770, 4, null, null, State.SkullAtk4, 0, 0), // State.SkullAtk3
            new StateDef(Sprite.SKUL, 32771, 4, null, null, State.SkullAtk3, 0, 0), // State.SkullAtk4
            new StateDef(Sprite.SKUL, 32772, 3, null, null, State.SkullPain2, 0, 0), // State.SkullPain
            new StateDef(Sprite.SKUL, 32772, 3, null, MobjActions.Pain, State.SkullRun1, 0, 0), // State.SkullPain2
            new StateDef(Sprite.SKUL, 32773, 6, null, null, State.SkullDie2, 0, 0), // State.SkullDie1
            new StateDef(Sprite.SKUL, 32774, 6, null, MobjActions.Scream, State.SkullDie3, 0, 0), // State.SkullDie2
            new StateDef(Sprite.SKUL, 32775, 6, null, null, State.SkullDie4, 0, 0), // State.SkullDie3
            new StateDef(Sprite.SKUL, 32776, 6, null, MobjActions.Fall, State.SkullDie5, 0, 0), // State.SkullDie4
            new StateDef(Sprite.SKUL, 9, 6, null, null, State.SkullDie6, 0, 0), // State.SkullDie5
            new StateDef(Sprite.SKUL, 10, 6, null, null, State.Null, 0, 0), // State.SkullDie6
            new StateDef(Sprite.SPID, 0, 10, null, MobjActions.Look, State.SpidStnd2, 0, 0), // State.SpidStnd
            new StateDef(Sprite.SPID, 1, 10, null, MobjActions.Look, State.SpidStnd, 0, 0), // State.SpidStnd2
            new StateDef(Sprite.SPID, 0, 3, null, MobjActions.Metal, State.SpidRun2, 0, 0), // State.SpidRun1
            new StateDef(Sprite.SPID, 0, 3, null, MobjActions.Chase, State.SpidRun3, 0, 0), // State.SpidRun2
            new StateDef(Sprite.SPID, 1, 3, null, MobjActions.Chase, State.SpidRun4, 0, 0), // State.SpidRun3
            new StateDef(Sprite.SPID, 1, 3, null, MobjActions.Chase, State.SpidRun5, 0, 0), // State.SpidRun4
            new StateDef(Sprite.SPID, 2, 3, null, MobjActions.Metal, State.SpidRun6, 0, 0), // State.SpidRun5
            new StateDef(Sprite.SPID, 2, 3, null, MobjActions.Chase, State.SpidRun7, 0, 0), // State.SpidRun6
            new StateDef(Sprite.SPID, 3, 3, null, MobjActions.Chase, State.SpidRun8, 0, 0), // State.SpidRun7
            new StateDef(Sprite.SPID, 3, 3, null, MobjActions.Chase, State.SpidRun9, 0, 0), // State.SpidRun8
            new StateDef(Sprite.SPID, 4, 3, null, MobjActions.Metal, State.SpidRun10, 0, 0), // State.SpidRun9
            new StateDef(Sprite.SPID, 4, 3, null, MobjActions.Chase, State.SpidRun11, 0, 0), // State.SpidRun10
            new StateDef(Sprite.SPID, 5, 3, null, MobjActions.Chase, State.SpidRun12, 0, 0), // State.SpidRun11
            new StateDef(Sprite.SPID, 5, 3, null, MobjActions.Chase, State.SpidRun1, 0, 0), // State.SpidRun12
            new StateDef(Sprite.SPID, 32768, 20, null, MobjActions.FaceTarget, State.SpidAtk2, 0, 0), // State.SpidAtk1
            new StateDef(Sprite.SPID, 32774, 4, null, MobjActions.SPosAttack, State.SpidAtk3, 0, 0), // State.SpidAtk2
            new StateDef(Sprite.SPID, 32775, 4, null, MobjActions.SPosAttack, State.SpidAtk4, 0, 0), // State.SpidAtk3
            new StateDef(Sprite.SPID, 32775, 1, null, MobjActions.SpidRefire, State.SpidAtk2, 0, 0), // State.SpidAtk4
            new StateDef(Sprite.SPID, 8, 3, null, null, State.SpidPain2, 0, 0), // State.SpidPain
            new StateDef(Sprite.SPID, 8, 3, null, MobjActions.Pain, State.SpidRun1, 0, 0), // State.SpidPain2
            new StateDef(Sprite.SPID, 9, 20, null, MobjActions.Scream, State.SpidDie2, 0, 0), // State.SpidDie1
            new StateDef(Sprite.SPID, 10, 10, null, MobjActions.Fall, State.SpidDie3, 0, 0), // State.SpidDie2
            new StateDef(Sprite.SPID, 11, 10, null, null, State.SpidDie4, 0, 0), // State.SpidDie3
            new StateDef(Sprite.SPID, 12, 10, null, null, State.SpidDie5, 0, 0), // State.SpidDie4
            new StateDef(Sprite.SPID, 13, 10, null, null, State.SpidDie6, 0, 0), // State.SpidDie5
            new StateDef(Sprite.SPID, 14, 10, null, null, State.SpidDie7, 0, 0), // State.SpidDie6
            new StateDef(Sprite.SPID, 15, 10, null, null, State.SpidDie8, 0, 0), // State.SpidDie7
            new StateDef(Sprite.SPID, 16, 10, null, null, State.SpidDie9, 0, 0), // State.SpidDie8
            new StateDef(Sprite.SPID, 17, 10, null, null, State.SpidDie10, 0, 0), // State.SpidDie9
            new StateDef(Sprite.SPID, 18, 30, null, null, State.SpidDie11, 0, 0), // State.SpidDie10
            new StateDef(Sprite.SPID, 18, -1, null, MobjActions.BossDeath, State.Null, 0, 0), // State.SpidDie11
            new StateDef(Sprite.BSPI, 0, 10, null, MobjActions.Look, State.BspiStnd2, 0, 0), // State.BspiStnd
            new StateDef(Sprite.BSPI, 1, 10, null, MobjActions.Look, State.BspiStnd, 0, 0), // State.BspiStnd2
            new StateDef(Sprite.BSPI, 0, 20, null, null, State.BspiRun1, 0, 0), // State.BspiSight
            new StateDef(Sprite.BSPI, 0, 3, null, MobjActions.BabyMetal, State.BspiRun2, 0, 0), // State.BspiRun1
            new StateDef(Sprite.BSPI, 0, 3, null, MobjActions.Chase, State.BspiRun3, 0, 0), // State.BspiRun2
            new StateDef(Sprite.BSPI, 1, 3, null, MobjActions.Chase, State.BspiRun4, 0, 0), // State.BspiRun3
            new StateDef(Sprite.BSPI, 1, 3, null, MobjActions.Chase, State.BspiRun5, 0, 0), // State.BspiRun4
            new StateDef(Sprite.BSPI, 2, 3, null, MobjActions.Chase, State.BspiRun6, 0, 0), // State.BspiRun5
            new StateDef(Sprite.BSPI, 2, 3, null, MobjActions.Chase, State.BspiRun7, 0, 0), // State.BspiRun6
            new StateDef(Sprite.BSPI, 3, 3, null, MobjActions.BabyMetal, State.BspiRun8, 0, 0), // State.BspiRun7
            new StateDef(Sprite.BSPI, 3, 3, null, MobjActions.Chase, State.BspiRun9, 0, 0), // State.BspiRun8
            new StateDef(Sprite.BSPI, 4, 3, null, MobjActions.Chase, State.BspiRun10, 0, 0), // State.BspiRun9
            new StateDef(Sprite.BSPI, 4, 3, null, MobjActions.Chase, State.BspiRun11, 0, 0), // State.BspiRun10
            new StateDef(Sprite.BSPI, 5, 3, null, MobjActions.Chase, State.BspiRun12, 0, 0), // State.BspiRun11
            new StateDef(Sprite.BSPI, 5, 3, null, MobjActions.Chase, State.BspiRun1, 0, 0), // State.BspiRun12
            new StateDef(Sprite.BSPI, 32768, 20, null, MobjActions.FaceTarget, State.BspiAtk2, 0, 0), // State.BspiAtk1
            new StateDef(Sprite.BSPI, 32774, 4, null, MobjActions.BspiAttack, State.BspiAtk3, 0, 0), // State.BspiAtk2
            new StateDef(Sprite.BSPI, 32775, 4, null, null, State.BspiAtk4, 0, 0), // State.BspiAtk3
            new StateDef(Sprite.BSPI, 32775, 1, null, MobjActions.SpidRefire, State.BspiAtk2, 0, 0), // State.BspiAtk4
            new StateDef(Sprite.BSPI, 8, 3, null, null, State.BspiPain2, 0, 0), // State.BspiPain
            new StateDef(Sprite.BSPI, 8, 3, null, MobjActions.Pain, State.BspiRun1, 0, 0), // State.BspiPain2
            new StateDef(Sprite.BSPI, 9, 20, null, MobjActions.Scream, State.BspiDie2, 0, 0), // State.BspiDie1
            new StateDef(Sprite.BSPI, 10, 7, null, MobjActions.Fall, State.BspiDie3, 0, 0), // State.BspiDie2
            new StateDef(Sprite.BSPI, 11, 7, null, null, State.BspiDie4, 0, 0), // State.BspiDie3
            new StateDef(Sprite.BSPI, 12, 7, null, null, State.BspiDie5, 0, 0), // State.BspiDie4
            new StateDef(Sprite.BSPI, 13, 7, null, null, State.BspiDie6, 0, 0), // State.BspiDie5
            new StateDef(Sprite.BSPI, 14, 7, null, null, State.BspiDie7, 0, 0), // State.BspiDie6
            new StateDef(Sprite.BSPI, 15, -1, null, MobjActions.BossDeath, State.Null, 0, 0), // State.BspiDie7
            new StateDef(Sprite.BSPI, 15, 5, null, null, State.BspiRaise2, 0, 0), // State.BspiRaise1
            new StateDef(Sprite.BSPI, 14, 5, null, null, State.BspiRaise3, 0, 0), // State.BspiRaise2
            new StateDef(Sprite.BSPI, 13, 5, null, null, State.BspiRaise4, 0, 0), // State.BspiRaise3
            new StateDef(Sprite.BSPI, 12, 5, null, null, State.BspiRaise5, 0, 0), // State.BspiRaise4
            new StateDef(Sprite.BSPI, 11, 5, null, null, State.BspiRaise6, 0, 0), // State.BspiRaise5
            new StateDef(Sprite.BSPI, 10, 5, null, null, State.BspiRaise7, 0, 0), // State.BspiRaise6
            new StateDef(Sprite.BSPI, 9, 5, null, null, State.BspiRun1, 0, 0), // State.BspiRaise7
            new StateDef(Sprite.APLS, 32768, 5, null, null, State.ArachPlaz2, 0, 0), // State.ArachPlaz
            new StateDef(Sprite.APLS, 32769, 5, null, null, State.ArachPlaz, 0, 0), // State.ArachPlaz2
            new StateDef(Sprite.APBX, 32768, 5, null, null, State.ArachPlex2, 0, 0), // State.ArachPlex
            new StateDef(Sprite.APBX, 32769, 5, null, null, State.ArachPlex3, 0, 0), // State.ArachPlex2
            new StateDef(Sprite.APBX, 32770, 5, null, null, State.ArachPlex4, 0, 0), // State.ArachPlex3
            new StateDef(Sprite.APBX, 32771, 5, null, null, State.ArachPlex5, 0, 0), // State.ArachPlex4
            new StateDef(Sprite.APBX, 32772, 5, null, null, State.Null, 0, 0), // State.ArachPlex5
            new StateDef(Sprite.CYBR, 0, 10, null, MobjActions.Look, State.CyberStnd2, 0, 0), // State.CyberStnd
            new StateDef(Sprite.CYBR, 1, 10, null, MobjActions.Look, State.CyberStnd, 0, 0), // State.CyberStnd2
            new StateDef(Sprite.CYBR, 0, 3, null, MobjActions.Hoof, State.CyberRun2, 0, 0), // State.CyberRun1
            new StateDef(Sprite.CYBR, 0, 3, null, MobjActions.Chase, State.CyberRun3, 0, 0), // State.CyberRun2
            new StateDef(Sprite.CYBR, 1, 3, null, MobjActions.Chase, State.CyberRun4, 0, 0), // State.CyberRun3
            new StateDef(Sprite.CYBR, 1, 3, null, MobjActions.Chase, State.CyberRun5, 0, 0), // State.CyberRun4
            new StateDef(Sprite.CYBR, 2, 3, null, MobjActions.Chase, State.CyberRun6, 0, 0), // State.CyberRun5
            new StateDef(Sprite.CYBR, 2, 3, null, MobjActions.Chase, State.CyberRun7, 0, 0), // State.CyberRun6
            new StateDef(Sprite.CYBR, 3, 3, null, MobjActions.Metal, State.CyberRun8, 0, 0), // State.CyberRun7
            new StateDef(Sprite.CYBR, 3, 3, null, MobjActions.Chase, State.CyberRun1, 0, 0), // State.CyberRun8
            new StateDef(Sprite.CYBR, 4, 6, null, MobjActions.FaceTarget, State.CyberAtk2, 0, 0), // State.CyberAtk1
            new StateDef(Sprite.CYBR, 5, 12, null, MobjActions.CyberAttack, State.CyberAtk3, 0, 0), // State.CyberAtk2
            new StateDef(Sprite.CYBR, 4, 12, null, MobjActions.FaceTarget, State.CyberAtk4, 0, 0), // State.CyberAtk3
            new StateDef(Sprite.CYBR, 5, 12, null, MobjActions.CyberAttack, State.CyberAtk5, 0, 0), // State.CyberAtk4
            new StateDef(Sprite.CYBR, 4, 12, null, MobjActions.FaceTarget, State.CyberAtk6, 0, 0), // State.CyberAtk5
            new StateDef(Sprite.CYBR, 5, 12, null, MobjActions.CyberAttack, State.CyberRun1, 0, 0), // State.CyberAtk6
            new StateDef(Sprite.CYBR, 6, 10, null, MobjActions.Pain, State.CyberRun1, 0, 0), // State.CyberPain
            new StateDef(Sprite.CYBR, 7, 10, null, null, State.CyberDie2, 0, 0), // State.CyberDie1
            new StateDef(Sprite.CYBR, 8, 10, null, MobjActions.Scream, State.CyberDie3, 0, 0), // State.CyberDie2
            new StateDef(Sprite.CYBR, 9, 10, null, null, State.CyberDie4, 0, 0), // State.CyberDie3
            new StateDef(Sprite.CYBR, 10, 10, null, null, State.CyberDie5, 0, 0), // State.CyberDie4
            new StateDef(Sprite.CYBR, 11, 10, null, null, State.CyberDie6, 0, 0), // State.CyberDie5
            new StateDef(Sprite.CYBR, 12, 10, null, MobjActions.Fall, State.CyberDie7, 0, 0), // State.CyberDie6
            new StateDef(Sprite.CYBR, 13, 10, null, null, State.CyberDie8, 0, 0), // State.CyberDie7
            new StateDef(Sprite.CYBR, 14, 10, null, null, State.CyberDie9, 0, 0), // State.CyberDie8
            new StateDef(Sprite.CYBR, 15, 30, null, null, State.CyberDie10, 0, 0), // State.CyberDie9
            new StateDef(Sprite.CYBR, 15, -1, null, MobjActions.BossDeath, State.Null, 0, 0), // State.CyberDie10
            new StateDef(Sprite.PAIN, 0, 10, null, MobjActions.Look, State.PainStnd, 0, 0), // State.PainStnd
            new StateDef(Sprite.PAIN, 0, 3, null, MobjActions.Chase, State.PainRun2, 0, 0), // State.PainRun1
            new StateDef(Sprite.PAIN, 0, 3, null, MobjActions.Chase, State.PainRun3, 0, 0), // State.PainRun2
            new StateDef(Sprite.PAIN, 1, 3, null, MobjActions.Chase, State.PainRun4, 0, 0), // State.PainRun3
            new StateDef(Sprite.PAIN, 1, 3, null, MobjActions.Chase, State.PainRun5, 0, 0), // State.PainRun4
            new StateDef(Sprite.PAIN, 2, 3, null, MobjActions.Chase, State.PainRun6, 0, 0), // State.PainRun5
            new StateDef(Sprite.PAIN, 2, 3, null, MobjActions.Chase, State.PainRun1, 0, 0), // State.PainRun6
            new StateDef(Sprite.PAIN, 3, 5, null, MobjActions.FaceTarget, State.PainAtk2, 0, 0), // State.PainAtk1
            new StateDef(Sprite.PAIN, 4, 5, null, MobjActions.FaceTarget, State.PainAtk3, 0, 0), // State.PainAtk2
            new StateDef(Sprite.PAIN, 32773, 5, null, MobjActions.FaceTarget, State.PainAtk4, 0, 0), // State.PainAtk3
            new StateDef(Sprite.PAIN, 32773, 0, null, MobjActions.PainAttack, State.PainRun1, 0, 0), // State.PainAtk4
            new StateDef(Sprite.PAIN, 6, 6, null, null, State.PainPain2, 0, 0), // State.PainPain
            new StateDef(Sprite.PAIN, 6, 6, null, MobjActions.Pain, State.PainRun1, 0, 0), // State.PainPain2
            new StateDef(Sprite.PAIN, 32775, 8, null, null, State.PainDie2, 0, 0), // State.PainDie1
            new StateDef(Sprite.PAIN, 32776, 8, null, MobjActions.Scream, State.PainDie3, 0, 0), // State.PainDie2
            new StateDef(Sprite.PAIN, 32777, 8, null, null, State.PainDie4, 0, 0), // State.PainDie3
            new StateDef(Sprite.PAIN, 32778, 8, null, null, State.PainDie5, 0, 0), // State.PainDie4
            new StateDef(Sprite.PAIN, 32779, 8, null, MobjActions.PainDie, State.PainDie6, 0, 0), // State.PainDie5
            new StateDef(Sprite.PAIN, 32780, 8, null, null, State.Null, 0, 0), // State.PainDie6
            new StateDef(Sprite.PAIN, 12, 8, null, null, State.PainRaise2, 0, 0), // State.PainRaise1
            new StateDef(Sprite.PAIN, 11, 8, null, null, State.PainRaise3, 0, 0), // State.PainRaise2
            new StateDef(Sprite.PAIN, 10, 8, null, null, State.PainRaise4, 0, 0), // State.PainRaise3
            new StateDef(Sprite.PAIN, 9, 8, null, null, State.PainRaise5, 0, 0), // State.PainRaise4
            new StateDef(Sprite.PAIN, 8, 8, null, null, State.PainRaise6, 0, 0), // State.PainRaise5
            new StateDef(Sprite.PAIN, 7, 8, null, null, State.PainRun1, 0, 0), // State.PainRaise6
            new StateDef(Sprite.SSWV, 0, 10, null, MobjActions.Look, State.SswvStnd2, 0, 0), // State.SswvStnd
            new StateDef(Sprite.SSWV, 1, 10, null, MobjActions.Look, State.SswvStnd, 0, 0), // State.SswvStnd2
            new StateDef(Sprite.SSWV, 0, 3, null, MobjActions.Chase, State.SswvRun2, 0, 0), // State.SswvRun1
            new StateDef(Sprite.SSWV, 0, 3, null, MobjActions.Chase, State.SswvRun3, 0, 0), // State.SswvRun2
            new StateDef(Sprite.SSWV, 1, 3, null, MobjActions.Chase, State.SswvRun4, 0, 0), // State.SswvRun3
            new StateDef(Sprite.SSWV, 1, 3, null, MobjActions.Chase, State.SswvRun5, 0, 0), // State.SswvRun4
            new StateDef(Sprite.SSWV, 2, 3, null, MobjActions.Chase, State.SswvRun6, 0, 0), // State.SswvRun5
            new StateDef(Sprite.SSWV, 2, 3, null, MobjActions.Chase, State.SswvRun7, 0, 0), // State.SswvRun6
            new StateDef(Sprite.SSWV, 3, 3, null, MobjActions.Chase, State.SswvRun8, 0, 0), // State.SswvRun7
            new StateDef(Sprite.SSWV, 3, 3, null, MobjActions.Chase, State.SswvRun1, 0, 0), // State.SswvRun8
            new StateDef(Sprite.SSWV, 4, 10, null, MobjActions.FaceTarget, State.SswvAtk2, 0, 0), // State.SswvAtk1
            new StateDef(Sprite.SSWV, 5, 10, null, MobjActions.FaceTarget, State.SswvAtk3, 0, 0), // State.SswvAtk2
            new StateDef(Sprite.SSWV, 32774, 4, null, MobjActions.CPosAttack, State.SswvAtk4, 0, 0), // State.SswvAtk3
            new StateDef(Sprite.SSWV, 5, 6, null, MobjActions.FaceTarget, State.SswvAtk5, 0, 0), // State.SswvAtk4
            new StateDef(Sprite.SSWV, 32774, 4, null, MobjActions.CPosAttack, State.SswvAtk6, 0, 0), // State.SswvAtk5
            new StateDef(Sprite.SSWV, 5, 1, null, MobjActions.CPosRefire, State.SswvAtk2, 0, 0), // State.SswvAtk6
            new StateDef(Sprite.SSWV, 7, 3, null, null, State.SswvPain2, 0, 0), // State.SswvPain
            new StateDef(Sprite.SSWV, 7, 3, null, MobjActions.Pain, State.SswvRun1, 0, 0), // State.SswvPain2
            new StateDef(Sprite.SSWV, 8, 5, null, null, State.SswvDie2, 0, 0), // State.SswvDie1
            new StateDef(Sprite.SSWV, 9, 5, null, MobjActions.Scream, State.SswvDie3, 0, 0), // State.SswvDie2
            new StateDef(Sprite.SSWV, 10, 5, null, MobjActions.Fall, State.SswvDie4, 0, 0), // State.SswvDie3
            new StateDef(Sprite.SSWV, 11, 5, null, null, State.SswvDie5, 0, 0), // State.SswvDie4
            new StateDef(Sprite.SSWV, 12, -1, null, null, State.Null, 0, 0), // State.SswvDie5
            new StateDef(Sprite.SSWV, 13, 5, null, null, State.SswvXdie2, 0, 0), // State.SswvXdie1
            new StateDef(Sprite.SSWV, 14, 5, null, MobjActions.XScream, State.SswvXdie3, 0, 0), // State.SswvXdie2
            new StateDef(Sprite.SSWV, 15, 5, null, MobjActions.Fall, State.SswvXdie4, 0, 0), // State.SswvXdie3
            new StateDef(Sprite.SSWV, 16, 5, null, null, State.SswvXdie5, 0, 0), // State.SswvXdie4
            new StateDef(Sprite.SSWV, 17, 5, null, null, State.SswvXdie6, 0, 0), // State.SswvXdie5
            new StateDef(Sprite.SSWV, 18, 5, null, null, State.SswvXdie7, 0, 0), // State.SswvXdie6
            new StateDef(Sprite.SSWV, 19, 5, null, null, State.SswvXdie8, 0, 0), // State.SswvXdie7
            new StateDef(Sprite.SSWV, 20, 5, null, null, State.SswvXdie9, 0, 0), // State.SswvXdie8
            new StateDef(Sprite.SSWV, 21, -1, null, null, State.Null, 0, 0), // State.SswvXdie9
            new StateDef(Sprite.SSWV, 12, 5, null, null, State.SswvRaise2, 0, 0), // State.SswvRaise1
            new StateDef(Sprite.SSWV, 11, 5, null, null, State.SswvRaise3, 0, 0), // State.SswvRaise2
            new StateDef(Sprite.SSWV, 10, 5, null, null, State.SswvRaise4, 0, 0), // State.SswvRaise3
            new StateDef(Sprite.SSWV, 9, 5, null, null, State.SswvRaise5, 0, 0), // State.SswvRaise4
            new StateDef(Sprite.SSWV, 8, 5, null, null, State.SswvRun1, 0, 0), // State.SswvRaise5
            new StateDef(Sprite.KEEN, 0, -1, null, null, State.Keenstnd, 0, 0), // State.Keenstnd
            new StateDef(Sprite.KEEN, 0, 6, null, null, State.Commkeen2, 0, 0), // State.Commkeen
            new StateDef(Sprite.KEEN, 1, 6, null, null, State.Commkeen3, 0, 0), // State.Commkeen2
            new StateDef(Sprite.KEEN, 2, 6, null, MobjActions.Scream, State.Commkeen4, 0, 0), // State.Commkeen3
            new StateDef(Sprite.KEEN, 3, 6, null, null, State.Commkeen5, 0, 0), // State.Commkeen4
            new StateDef(Sprite.KEEN, 4, 6, null, null, State.Commkeen6, 0, 0), // State.Commkeen5
            new StateDef(Sprite.KEEN, 5, 6, null, null, State.Commkeen7, 0, 0), // State.Commkeen6
            new StateDef(Sprite.KEEN, 6, 6, null, null, State.Commkeen8, 0, 0), // State.Commkeen7
            new StateDef(Sprite.KEEN, 7, 6, null, null, State.Commkeen9, 0, 0), // State.Commkeen8
            new StateDef(Sprite.KEEN, 8, 6, null, null, State.Commkeen10, 0, 0), // State.Commkeen9
            new StateDef(Sprite.KEEN, 9, 6, null, null, State.Commkeen11, 0, 0), // State.Commkeen10
            new StateDef(Sprite.KEEN, 10, 6, null, MobjActions.KeenDie, State.Commkeen12, 0, 0), // State.Commkeen11
            new StateDef(Sprite.KEEN, 11, -1, null, null, State.Null, 0, 0), // State.Commkeen12
            new StateDef(Sprite.KEEN, 12, 4, null, null, State.Keenpain2, 0, 0), // State.Keenpain
            new StateDef(Sprite.KEEN, 12, 8, null, MobjActions.Pain, State.Keenstnd, 0, 0), // State.Keenpain2
            new StateDef(Sprite.BBRN, 0, -1, null, null, State.Null, 0, 0), // State.Brain
            new StateDef(Sprite.BBRN, 1, 36, null, MobjActions.BrainPain, State.Brain, 0, 0), // State.BrainPain
            new StateDef(Sprite.BBRN, 0, 100, null, MobjActions.BrainScream, State.BrainDie2, 0, 0), // State.BrainDie1
            new StateDef(Sprite.BBRN, 0, 10, null, null, State.BrainDie3, 0, 0), // State.BrainDie2
            new StateDef(Sprite.BBRN, 0, 10, null, null, State.BrainDie4, 0, 0), // State.BrainDie3
            new StateDef(Sprite.BBRN, 0, -1, null, MobjActions.BrainDie, State.Null, 0, 0), // State.BrainDie4
            new StateDef(Sprite.SSWV, 0, 10, null, MobjActions.Look, State.Braineye, 0, 0), // State.Braineye
            new StateDef(Sprite.SSWV, 0, 181, null, MobjActions.BrainAwake, State.Braineye1, 0, 0), // State.Braineyesee
            new StateDef(Sprite.SSWV, 0, 150, null, MobjActions.BrainSpit, State.Braineye1, 0, 0), // State.Braineye1
            new StateDef(Sprite.BOSF, 32768, 3, null, MobjActions.SpawnSound, State.Spawn2, 0, 0), // State.Spawn1
            new StateDef(Sprite.BOSF, 32769, 3, null, MobjActions.SpawnFly, State.Spawn3, 0, 0), // State.Spawn2
            new StateDef(Sprite.BOSF, 32770, 3, null, MobjActions.SpawnFly, State.Spawn4, 0, 0), // State.Spawn3
            new StateDef(Sprite.BOSF, 32771, 3, null, MobjActions.SpawnFly, State.Spawn1, 0, 0), // State.Spawn4
            new StateDef(Sprite.FIRE, 32768, 4, null, MobjActions.Fire, State.Spawnfire2, 0, 0), // State.Spawnfire1
            new StateDef(Sprite.FIRE, 32769, 4, null, MobjActions.Fire, State.Spawnfire3, 0, 0), // State.Spawnfire2
            new StateDef(Sprite.FIRE, 32770, 4, null, MobjActions.Fire, State.Spawnfire4, 0, 0), // State.Spawnfire3
            new StateDef(Sprite.FIRE, 32771, 4, null, MobjActions.Fire, State.Spawnfire5, 0, 0), // State.Spawnfire4
            new StateDef(Sprite.FIRE, 32772, 4, null, MobjActions.Fire, State.Spawnfire6, 0, 0), // State.Spawnfire5
            new StateDef(Sprite.FIRE, 32773, 4, null, MobjActions.Fire, State.Spawnfire7, 0, 0), // State.Spawnfire6
            new StateDef(Sprite.FIRE, 32774, 4, null, MobjActions.Fire, State.Spawnfire8, 0, 0), // State.Spawnfire7
            new StateDef(Sprite.FIRE, 32775, 4, null, MobjActions.Fire, State.Null, 0, 0), // State.Spawnfire8
            new StateDef(Sprite.MISL, 32769, 10, null, null, State.Brainexplode2, 0, 0), // State.Brainexplode1
            new StateDef(Sprite.MISL, 32770, 10, null, null, State.Brainexplode3, 0, 0), // State.Brainexplode2
            new StateDef(Sprite.MISL, 32771, 10, null, MobjActions.BrainExplode, State.Null, 0, 0), // State.Brainexplode3
            new StateDef(Sprite.ARM1, 0, 6, null, null, State.Arm1A, 0, 0), // State.Arm1
            new StateDef(Sprite.ARM1, 32769, 7, null, null, State.Arm1, 0, 0), // State.Arm1A
            new StateDef(Sprite.ARM2, 0, 6, null, null, State.Arm2A, 0, 0), // State.Arm2
            new StateDef(Sprite.ARM2, 32769, 6, null, null, State.Arm2, 0, 0), // State.Arm2A
            new StateDef(Sprite.BAR1, 0, 6, null, null, State.Bar2, 0, 0), // State.Bar1
            new StateDef(Sprite.BAR1, 1, 6, null, null, State.Bar1, 0, 0), // State.Bar2
            new StateDef(Sprite.BEXP, 32768, 5, null, null, State.Bexp2, 0, 0), // State.Bexp
            new StateDef(Sprite.BEXP, 32769, 5, null, MobjActions.Scream, State.Bexp3, 0, 0), // State.Bexp2
            new StateDef(Sprite.BEXP, 32770, 5, null, null, State.Bexp4, 0, 0), // State.Bexp3
            new StateDef(Sprite.BEXP, 32771, 10, null, MobjActions.Explode, State.Bexp5, 0, 0), // State.Bexp4
            new StateDef(Sprite.BEXP, 32772, 10, null, null, State.Null, 0, 0), // State.Bexp5
            new StateDef(Sprite.FCAN, 32768, 4, null, null, State.Bbar2, 0, 0), // State.Bbar1
            new StateDef(Sprite.FCAN, 32769, 4, null, null, State.Bbar3, 0, 0), // State.Bbar2
            new StateDef(Sprite.FCAN, 32770, 4, null, null, State.Bbar1, 0, 0), // State.Bbar3
            new StateDef(Sprite.BON1, 0, 6, null, null, State.Bon1A, 0, 0), // State.Bon1
            new StateDef(Sprite.BON1, 1, 6, null, null, State.Bon1B, 0, 0), // State.Bon1A
            new StateDef(Sprite.BON1, 2, 6, null, null, State.Bon1C, 0, 0), // State.Bon1B
            new StateDef(Sprite.BON1, 3, 6, null, null, State.Bon1D, 0, 0), // State.Bon1C
            new StateDef(Sprite.BON1, 2, 6, null, null, State.Bon1E, 0, 0), // State.Bon1D
            new StateDef(Sprite.BON1, 1, 6, null, null, State.Bon1, 0, 0), // State.Bon1E
            new StateDef(Sprite.BON2, 0, 6, null, null, State.Bon2A, 0, 0), // State.Bon2
            new StateDef(Sprite.BON2, 1, 6, null, null, State.Bon2B, 0, 0), // State.Bon2A
            new StateDef(Sprite.BON2, 2, 6, null, null, State.Bon2C, 0, 0), // State.Bon2B
            new StateDef(Sprite.BON2, 3, 6, null, null, State.Bon2D, 0, 0), // State.Bon2C
            new StateDef(Sprite.BON2, 2, 6, null, null, State.Bon2E, 0, 0), // State.Bon2D
            new StateDef(Sprite.BON2, 1, 6, null, null, State.Bon2, 0, 0), // State.Bon2E
            new StateDef(Sprite.BKEY, 0, 10, null, null, State.Bkey2, 0, 0), // State.Bkey
            new StateDef(Sprite.BKEY, 32769, 10, null, null, State.Bkey, 0, 0), // State.Bkey2
            new StateDef(Sprite.RKEY, 0, 10, null, null, State.Rkey2, 0, 0), // State.Rkey
            new StateDef(Sprite.RKEY, 32769, 10, null, null, State.Rkey, 0, 0), // State.Rkey2
            new StateDef(Sprite.YKEY, 0, 10, null, null, State.Ykey2, 0, 0), // State.Ykey
            new StateDef(Sprite.YKEY, 32769, 10, null, null, State.Ykey, 0, 0), // State.Ykey2
            new StateDef(Sprite.BSKU, 0, 10, null, null, State.Bskull2, 0, 0), // State.Bskull
            new StateDef(Sprite.BSKU, 32769, 10, null, null, State.Bskull, 0, 0), // State.Bskull2
            new StateDef(Sprite.RSKU, 0, 10, null, null, State.Rskull2, 0, 0), // State.Rskull
            new StateDef(Sprite.RSKU, 32769, 10, null, null, State.Rskull, 0, 0), // State.Rskull2
            new StateDef(Sprite.YSKU, 0, 10, null, null, State.Yskull2, 0, 0), // State.Yskull
            new StateDef(Sprite.YSKU, 32769, 10, null, null, State.Yskull, 0, 0), // State.Yskull2
            new StateDef(Sprite.STIM, 0, -1, null, null, State.Null, 0, 0), // State.Stim
            new StateDef(Sprite.MEDI, 0, -1, null, null, State.Null, 0, 0), // State.Medi
            new StateDef(Sprite.SOUL, 32768, 6, null, null, State.Soul2, 0, 0), // State.Soul
            new StateDef(Sprite.SOUL, 32769, 6, null, null, State.Soul3, 0, 0), // State.Soul2
            new StateDef(Sprite.SOUL, 32770, 6, null, null, State.Soul4, 0, 0), // State.Soul3
            new StateDef(Sprite.SOUL, 32771, 6, null, null, State.Soul5, 0, 0), // State.Soul4
            new StateDef(Sprite.SOUL, 32770, 6, null, null, State.Soul6, 0, 0), // State.Soul5
            new StateDef(Sprite.SOUL, 32769, 6, null, null, State.Soul, 0, 0), // State.Soul6
            new StateDef(Sprite.PINV, 32768, 6, null, null, State.Pinv2, 0, 0), // State.Pinv
            new StateDef(Sprite.PINV, 32769, 6, null, null, State.Pinv3, 0, 0), // State.Pinv2
            new StateDef(Sprite.PINV, 32770, 6, null, null, State.Pinv4, 0, 0), // State.Pinv3
            new StateDef(Sprite.PINV, 32771, 6, null, null, State.Pinv, 0, 0), // State.Pinv4
            new StateDef(Sprite.PSTR, 32768, -1, null, null, State.Null, 0, 0), // State.Pstr
            new StateDef(Sprite.PINS, 32768, 6, null, null, State.Pins2, 0, 0), // State.Pins
            new StateDef(Sprite.PINS, 32769, 6, null, null, State.Pins3, 0, 0), // State.Pins2
            new StateDef(Sprite.PINS, 32770, 6, null, null, State.Pins4, 0, 0), // State.Pins3
            new StateDef(Sprite.PINS, 32771, 6, null, null, State.Pins, 0, 0), // State.Pins4
            new StateDef(Sprite.MEGA, 32768, 6, null, null, State.Mega2, 0, 0), // State.Mega
            new StateDef(Sprite.MEGA, 32769, 6, null, null, State.Mega3, 0, 0), // State.Mega2
            new StateDef(Sprite.MEGA, 32770, 6, null, null, State.Mega4, 0, 0), // State.Mega3
            new StateDef(Sprite.MEGA, 32771, 6, null, null, State.Mega, 0, 0), // State.Mega4
            new StateDef(Sprite.SUIT, 32768, -1, null, null, State.Null, 0, 0), // State.Suit
            new StateDef(Sprite.PMAP, 32768, 6, null, null, State.Pmap2, 0, 0), // State.Pmap
            new StateDef(Sprite.PMAP, 32769, 6, null, null, State.Pmap3, 0, 0), // State.Pmap2
            new StateDef(Sprite.PMAP, 32770, 6, null, null, State.Pmap4, 0, 0), // State.Pmap3
            new StateDef(Sprite.PMAP, 32771, 6, null, null, State.Pmap5, 0, 0), // State.Pmap4
            new StateDef(Sprite.PMAP, 32770, 6, null, null, State.Pmap6, 0, 0), // State.Pmap5
            new StateDef(Sprite.PMAP, 32769, 6, null, null, State.Pmap, 0, 0), // State.Pmap6
            new StateDef(Sprite.PVIS, 32768, 6, null, null, State.Pvis2, 0, 0), // State.Pvis
            new StateDef(Sprite.PVIS, 1, 6, null, null, State.Pvis, 0, 0), // State.Pvis2
            new StateDef(Sprite.CLIP, 0, -1, null, null, State.Null, 0, 0), // State.Clip
            new StateDef(Sprite.AMMO, 0, -1, null, null, State.Null, 0, 0), // State.Ammo
            new StateDef(Sprite.ROCK, 0, -1, null, null, State.Null, 0, 0), // State.Rock
            new StateDef(Sprite.BROK, 0, -1, null, null, State.Null, 0, 0), // State.Brok
            new StateDef(Sprite.CELL, 0, -1, null, null, State.Null, 0, 0), // State.Cell
            new StateDef(Sprite.CELP, 0, -1, null, null, State.Null, 0, 0), // State.Celp
            new StateDef(Sprite.SHEL, 0, -1, null, null, State.Null, 0, 0), // State.Shel
            new StateDef(Sprite.SBOX, 0, -1, null, null, State.Null, 0, 0), // State.Sbox
            new StateDef(Sprite.BPAK, 0, -1, null, null, State.Null, 0, 0), // State.Bpak
            new StateDef(Sprite.BFUG, 0, -1, null, null, State.Null, 0, 0), // State.Bfug
            new StateDef(Sprite.MGUN, 0, -1, null, null, State.Null, 0, 0), // State.Mgun
            new StateDef(Sprite.CSAW, 0, -1, null, null, State.Null, 0, 0), // State.Csaw
            new StateDef(Sprite.LAUN, 0, -1, null, null, State.Null, 0, 0), // State.Laun
            new StateDef(Sprite.PLAS, 0, -1, null, null, State.Null, 0, 0), // State.Plas
            new StateDef(Sprite.SHOT, 0, -1, null, null, State.Null, 0, 0), // State.Shot
            new StateDef(Sprite.SGN2, 0, -1, null, null, State.Null, 0, 0), // State.Shot2
            new StateDef(Sprite.COLU, 32768, -1, null, null, State.Null, 0, 0), // State.Colu
            new StateDef(Sprite.SMT2, 0, -1, null, null, State.Null, 0, 0), // State.Stalag
            new StateDef(Sprite.GOR1, 0, 10, null, null, State.Bloodytwitch2, 0, 0), // State.Bloodytwitch
            new StateDef(Sprite.GOR1, 1, 15, null, null, State.Bloodytwitch3, 0, 0), // State.Bloodytwitch2
            new StateDef(Sprite.GOR1, 2, 8, null, null, State.Bloodytwitch4, 0, 0), // State.Bloodytwitch3
            new StateDef(Sprite.GOR1, 1, 6, null, null, State.Bloodytwitch, 0, 0), // State.Bloodytwitch4
            new StateDef(Sprite.PLAY, 13, -1, null, null, State.Null, 0, 0), // State.Deadtorso
            new StateDef(Sprite.PLAY, 18, -1, null, null, State.Null, 0, 0), // State.Deadbottom
            new StateDef(Sprite.POL2, 0, -1, null, null, State.Null, 0, 0), // State.Headsonstick
            new StateDef(Sprite.POL5, 0, -1, null, null, State.Null, 0, 0), // State.Gibs
            new StateDef(Sprite.POL4, 0, -1, null, null, State.Null, 0, 0), // State.Headonastick
            new StateDef(Sprite.POL3, 32768, 6, null, null, State.Headcandles2, 0, 0), // State.Headcandles
            new StateDef(Sprite.POL3, 32769, 6, null, null, State.Headcandles, 0, 0), // State.Headcandles2
            new StateDef(Sprite.POL1, 0, -1, null, null, State.Null, 0, 0), // State.Deadstick
            new StateDef(Sprite.POL6, 0, 6, null, null, State.Livestick2, 0, 0), // State.Livestick
            new StateDef(Sprite.POL6, 1, 8, null, null, State.Livestick, 0, 0), // State.Livestick2
            new StateDef(Sprite.GOR2, 0, -1, null, null, State.Null, 0, 0), // State.Meat2
            new StateDef(Sprite.GOR3, 0, -1, null, null, State.Null, 0, 0), // State.Meat3
            new StateDef(Sprite.GOR4, 0, -1, null, null, State.Null, 0, 0), // State.Meat4
            new StateDef(Sprite.GOR5, 0, -1, null, null, State.Null, 0, 0), // State.Meat5
            new StateDef(Sprite.SMIT, 0, -1, null, null, State.Null, 0, 0), // State.Stalagtite
            new StateDef(Sprite.COL1, 0, -1, null, null, State.Null, 0, 0), // State.Tallgrncol
            new StateDef(Sprite.COL2, 0, -1, null, null, State.Null, 0, 0), // State.Shrtgrncol
            new StateDef(Sprite.COL3, 0, -1, null, null, State.Null, 0, 0), // State.Tallredcol
            new StateDef(Sprite.COL4, 0, -1, null, null, State.Null, 0, 0), // State.Shrtredcol
            new StateDef(Sprite.CAND, 32768, -1, null, null, State.Null, 0, 0), // State.Candlestik
            new StateDef(Sprite.CBRA, 32768, -1, null, null, State.Null, 0, 0), // State.Candelabra
            new StateDef(Sprite.COL6, 0, -1, null, null, State.Null, 0, 0), // State.Skullcol
            new StateDef(Sprite.TRE1, 0, -1, null, null, State.Null, 0, 0), // State.Torchtree
            new StateDef(Sprite.TRE2, 0, -1, null, null, State.Null, 0, 0), // State.Bigtree
            new StateDef(Sprite.ELEC, 0, -1, null, null, State.Null, 0, 0), // State.Techpillar
            new StateDef(Sprite.CEYE, 32768, 6, null, null, State.Evileye2, 0, 0), // State.Evileye
            new StateDef(Sprite.CEYE, 32769, 6, null, null, State.Evileye3, 0, 0), // State.Evileye2
            new StateDef(Sprite.CEYE, 32770, 6, null, null, State.Evileye4, 0, 0), // State.Evileye3
            new StateDef(Sprite.CEYE, 32769, 6, null, null, State.Evileye, 0, 0), // State.Evileye4
            new StateDef(Sprite.FSKU, 32768, 6, null, null, State.Floatskull2, 0, 0), // State.Floatskull
            new StateDef(Sprite.FSKU, 32769, 6, null, null, State.Floatskull3, 0, 0), // State.Floatskull2
            new StateDef(Sprite.FSKU, 32770, 6, null, null, State.Floatskull, 0, 0), // State.Floatskull3
            new StateDef(Sprite.COL5, 0, 14, null, null, State.Heartcol2, 0, 0), // State.Heartcol
            new StateDef(Sprite.COL5, 1, 14, null, null, State.Heartcol, 0, 0), // State.Heartcol2
            new StateDef(Sprite.TBLU, 32768, 4, null, null, State.Bluetorch2, 0, 0), // State.Bluetorch
            new StateDef(Sprite.TBLU, 32769, 4, null, null, State.Bluetorch3, 0, 0), // State.Bluetorch2
            new StateDef(Sprite.TBLU, 32770, 4, null, null, State.Bluetorch4, 0, 0), // State.Bluetorch3
            new StateDef(Sprite.TBLU, 32771, 4, null, null, State.Bluetorch, 0, 0), // State.Bluetorch4
            new StateDef(Sprite.TGRN, 32768, 4, null, null, State.Greentorch2, 0, 0), // State.Greentorch
            new StateDef(Sprite.TGRN, 32769, 4, null, null, State.Greentorch3, 0, 0), // State.Greentorch2
            new StateDef(Sprite.TGRN, 32770, 4, null, null, State.Greentorch4, 0, 0), // State.Greentorch3
            new StateDef(Sprite.TGRN, 32771, 4, null, null, State.Greentorch, 0, 0), // State.Greentorch4
            new StateDef(Sprite.TRED, 32768, 4, null, null, State.Redtorch2, 0, 0), // State.Redtorch
            new StateDef(Sprite.TRED, 32769, 4, null, null, State.Redtorch3, 0, 0), // State.Redtorch2
            new StateDef(Sprite.TRED, 32770, 4, null, null, State.Redtorch4, 0, 0), // State.Redtorch3
            new StateDef(Sprite.TRED, 32771, 4, null, null, State.Redtorch, 0, 0), // State.Redtorch4
            new StateDef(Sprite.SMBT, 32768, 4, null, null, State.Btorchshrt2, 0, 0), // State.Btorchshrt
            new StateDef(Sprite.SMBT, 32769, 4, null, null, State.Btorchshrt3, 0, 0), // State.Btorchshrt2
            new StateDef(Sprite.SMBT, 32770, 4, null, null, State.Btorchshrt4, 0, 0), // State.Btorchshrt3
            new StateDef(Sprite.SMBT, 32771, 4, null, null, State.Btorchshrt, 0, 0), // State.Btorchshrt4
            new StateDef(Sprite.SMGT, 32768, 4, null, null, State.Gtorchshrt2, 0, 0), // State.Gtorchshrt
            new StateDef(Sprite.SMGT, 32769, 4, null, null, State.Gtorchshrt3, 0, 0), // State.Gtorchshrt2
            new StateDef(Sprite.SMGT, 32770, 4, null, null, State.Gtorchshrt4, 0, 0), // State.Gtorchshrt3
            new StateDef(Sprite.SMGT, 32771, 4, null, null, State.Gtorchshrt, 0, 0), // State.Gtorchshrt4
            new StateDef(Sprite.SMRT, 32768, 4, null, null, State.Rtorchshrt2, 0, 0), // State.Rtorchshrt
            new StateDef(Sprite.SMRT, 32769, 4, null, null, State.Rtorchshrt3, 0, 0), // State.Rtorchshrt2
            new StateDef(Sprite.SMRT, 32770, 4, null, null, State.Rtorchshrt4, 0, 0), // State.Rtorchshrt3
            new StateDef(Sprite.SMRT, 32771, 4, null, null, State.Rtorchshrt, 0, 0), // State.Rtorchshrt4
            new StateDef(Sprite.HDB1, 0, -1, null, null, State.Null, 0, 0), // State.Hangnoguts
            new StateDef(Sprite.HDB2, 0, -1, null, null, State.Null, 0, 0), // State.Hangbnobrain
            new StateDef(Sprite.HDB3, 0, -1, null, null, State.Null, 0, 0), // State.Hangtlookdn
            new StateDef(Sprite.HDB4, 0, -1, null, null, State.Null, 0, 0), // State.Hangtskull
            new StateDef(Sprite.HDB5, 0, -1, null, null, State.Null, 0, 0), // State.Hangtlookup
            new StateDef(Sprite.HDB6, 0, -1, null, null, State.Null, 0, 0), // State.Hangtnobrain
            new StateDef(Sprite.POB1, 0, -1, null, null, State.Null, 0, 0), // State.Colongibs
            new StateDef(Sprite.POB2, 0, -1, null, null, State.Null, 0, 0), // State.Smallpool
            new StateDef(Sprite.BRS1, 0, -1, null, null, State.Null, 0, 0), // State.Brainstem
            new StateDef(Sprite.TLMP, 32768, 4, null, null, State.Techlamp2, 0, 0), // State.Techlamp
            new StateDef(Sprite.TLMP, 32769, 4, null, null, State.Techlamp3, 0, 0), // State.Techlamp2
            new StateDef(Sprite.TLMP, 32770, 4, null, null, State.Techlamp4, 0, 0), // State.Techlamp3
            new StateDef(Sprite.TLMP, 32771, 4, null, null, State.Techlamp, 0, 0), // State.Techlamp4
            new StateDef(Sprite.TLP2, 32768, 4, null, null, State.Tech2Lamp2, 0, 0), // State.Tech2Lamp
            new StateDef(Sprite.TLP2, 32769, 4, null, null, State.Tech2Lamp3, 0, 0), // State.Tech2Lamp2
            new StateDef(Sprite.TLP2, 32770, 4, null, null, State.Tech2Lamp4, 0, 0), // State.Tech2Lamp3
            new StateDef(Sprite.TLP2, 32771, 4, null, null, State.Tech2Lamp, 0, 0), // State.Tech2Lamp4
        };

        public static readonly MobjInfo[] MobjInfos =
        {
            new MobjInfo( // MobjType.Player
                -1, // doomEdNum
                State.Play, // spawnState
                100, // spawnHealth
                State.PlayRun1, // seeState
                Sfx.NONE, // seeSound
                0, // reactionTime
                Sfx.NONE, // attackSound
                State.PlayPain, // painState
                255, // painChance
                Sfx.PLPAIN, // painSound
                State.Null, // meleeState
                State.PlayAtk1, // missileState
                State.PlayDie1, // deathState
                State.PlayXdie1, // xdeathState
                Sfx.PLDETH, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.DropOff | MobjFlags.PickUp | MobjFlags.NotDeathmatch, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Possessed
                3004, // doomEdNum
                State.PossStnd, // spawnState
                20, // spawnHealth
                State.PossRun1, // seeState
                Sfx.POSIT1, // seeSound
                8, // reactionTime
                Sfx.PISTOL, // attackSound
                State.PossPain, // painState
                200, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.PossAtk1, // missileState
                State.PossDie1, // deathState
                State.PossXdie1, // xdeathState
                Sfx.PODTH1, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.PossRaise1), // raiseState

            new MobjInfo( // MobjType.Shotguy
                9, // doomEdNum
                State.SposStnd, // spawnState
                30, // spawnHealth
                State.SposRun1, // seeState
                Sfx.POSIT2, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.SposPain, // painState
                170, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.SposAtk1, // missileState
                State.SposDie1, // deathState
                State.SposXdie1, // xdeathState
                Sfx.PODTH2, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SposRaise1), // raiseState

            new MobjInfo( // MobjType.Vile
                64, // doomEdNum
                State.VileStnd, // spawnState
                700, // spawnHealth
                State.VileRun1, // seeState
                Sfx.VILSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.VilePain, // painState
                10, // painChance
                Sfx.VIPAIN, // painSound
                State.Null, // meleeState
                State.VileAtk1, // missileState
                State.VileDie1, // deathState
                State.Null, // xdeathState
                Sfx.VILDTH, // deathSound
                15, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                500, // mass
                0, // damage
                Sfx.VILACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Fire
                -1, // doomEdNum
                State.Fire1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Undead
                66, // doomEdNum
                State.SkelStnd, // spawnState
                300, // spawnHealth
                State.SkelRun1, // seeState
                Sfx.SKESIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.SkelPain, // painState
                100, // painChance
                Sfx.POPAIN, // painSound
                State.SkelFist1, // meleeState
                State.SkelMiss1, // missileState
                State.SkelDie1, // deathState
                State.Null, // xdeathState
                Sfx.SKEDTH, // deathSound
                10, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                500, // mass
                0, // damage
                Sfx.SKEACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SkelRaise1), // raiseState

            new MobjInfo( // MobjType.Tracer
                -1, // doomEdNum
                State.Tracer, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.SKEATK, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Traceexp1, // deathState
                State.Null, // xdeathState
                Sfx.BAREXP, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(11), // radius
                Fixed.FromInt(8), // height
                100, // mass
                10, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Smoke
                -1, // doomEdNum
                State.Smoke1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Fatso
                67, // doomEdNum
                State.FattStnd, // spawnState
                600, // spawnHealth
                State.FattRun1, // seeState
                Sfx.MANSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.FattPain, // painState
                80, // painChance
                Sfx.MNPAIN, // painSound
                State.Null, // meleeState
                State.FattAtk1, // missileState
                State.FattDie1, // deathState
                State.Null, // xdeathState
                Sfx.MANDTH, // deathSound
                8, // speed
                Fixed.FromInt(48), // radius
                Fixed.FromInt(64), // height
                1000, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.FattRaise1), // raiseState

            new MobjInfo( // MobjType.Fatshot
                -1, // doomEdNum
                State.Fatshot1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Fatshotx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                20 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                8, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Chainguy
                65, // doomEdNum
                State.CposStnd, // spawnState
                70, // spawnHealth
                State.CposRun1, // seeState
                Sfx.POSIT2, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.CposPain, // painState
                170, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.CposAtk1, // missileState
                State.CposDie1, // deathState
                State.CposXdie1, // xdeathState
                Sfx.PODTH2, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.CposRaise1), // raiseState

            new MobjInfo( // MobjType.Troop
                3001, // doomEdNum
                State.TrooStnd, // spawnState
                60, // spawnHealth
                State.TrooRun1, // seeState
                Sfx.BGSIT1, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.TrooPain, // painState
                200, // painChance
                Sfx.POPAIN, // painSound
                State.TrooAtk1, // meleeState
                State.TrooAtk1, // missileState
                State.TrooDie1, // deathState
                State.TrooXdie1, // xdeathState
                Sfx.BGDTH1, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.BGACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.TrooRaise1), // raiseState

            new MobjInfo( // MobjType.Sergeant
                3002, // doomEdNum
                State.SargStnd, // spawnState
                150, // spawnHealth
                State.SargRun1, // seeState
                Sfx.SGTSIT, // seeSound
                8, // reactionTime
                Sfx.SGTATK, // attackSound
                State.SargPain, // painState
                180, // painChance
                Sfx.DMPAIN, // painSound
                State.SargAtk1, // meleeState
                State.Null, // missileState
                State.SargDie1, // deathState
                State.Null, // xdeathState
                Sfx.SGTDTH, // deathSound
                10, // speed
                Fixed.FromInt(30), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SargRaise1), // raiseState

            new MobjInfo( // MobjType.Shadows
                58, // doomEdNum
                State.SargStnd, // spawnState
                150, // spawnHealth
                State.SargRun1, // seeState
                Sfx.SGTSIT, // seeSound
                8, // reactionTime
                Sfx.SGTATK, // attackSound
                State.SargPain, // painState
                180, // painChance
                Sfx.DMPAIN, // painSound
                State.SargAtk1, // meleeState
                State.Null, // missileState
                State.SargDie1, // deathState
                State.Null, // xdeathState
                Sfx.SGTDTH, // deathSound
                10, // speed
                Fixed.FromInt(30), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Shadow | MobjFlags.CountKill, // flags
                State.SargRaise1), // raiseState

            new MobjInfo( // MobjType.Head
                3005, // doomEdNum
                State.HeadStnd, // spawnState
                400, // spawnHealth
                State.HeadRun1, // seeState
                Sfx.CACSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.HeadPain, // painState
                128, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.HeadAtk1, // missileState
                State.HeadDie1, // deathState
                State.Null, // xdeathState
                Sfx.CACDTH, // deathSound
                8, // speed
                Fixed.FromInt(31), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity | MobjFlags.CountKill, // flags
                State.HeadRaise1), // raiseState

            new MobjInfo( // MobjType.Bruiser
                3003, // doomEdNum
                State.BossStnd, // spawnState
                1000, // spawnHealth
                State.BossRun1, // seeState
                Sfx.BRSSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.BossPain, // painState
                50, // painChance
                Sfx.DMPAIN, // painSound
                State.BossAtk1, // meleeState
                State.BossAtk1, // missileState
                State.BossDie1, // deathState
                State.Null, // xdeathState
                Sfx.BRSDTH, // deathSound
                8, // speed
                Fixed.FromInt(24), // radius
                Fixed.FromInt(64), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.BossRaise1), // raiseState

            new MobjInfo( // MobjType.Bruisershot
                -1, // doomEdNum
                State.Brball1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Brballx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                15 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                8, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Knight
                69, // doomEdNum
                State.Bos2Stnd, // spawnState
                500, // spawnHealth
                State.Bos2Run1, // seeState
                Sfx.KNTSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Bos2Pain, // painState
                50, // painChance
                Sfx.DMPAIN, // painSound
                State.Bos2Atk1, // meleeState
                State.Bos2Atk1, // missileState
                State.Bos2Die1, // deathState
                State.Null, // xdeathState
                Sfx.KNTDTH, // deathSound
                8, // speed
                Fixed.FromInt(24), // radius
                Fixed.FromInt(64), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Bos2Raise1), // raiseState

            new MobjInfo( // MobjType.Skull
                3006, // doomEdNum
                State.SkullStnd, // spawnState
                100, // spawnHealth
                State.SkullRun1, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.SKLATK, // attackSound
                State.SkullPain, // painState
                256, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.SkullAtk1, // missileState
                State.SkullDie1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                8, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(56), // height
                50, // mass
                3, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Spider
                7, // doomEdNum
                State.SpidStnd, // spawnState
                3000, // spawnHealth
                State.SpidRun1, // seeState
                Sfx.SPISIT, // seeSound
                8, // reactionTime
                Sfx.SHOTGN, // attackSound
                State.SpidPain, // painState
                40, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.SpidAtk1, // missileState
                State.SpidDie1, // deathState
                State.Null, // xdeathState
                Sfx.SPIDTH, // deathSound
                12, // speed
                Fixed.FromInt(128), // radius
                Fixed.FromInt(100), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Baby
                68, // doomEdNum
                State.BspiStnd, // spawnState
                500, // spawnHealth
                State.BspiSight, // seeState
                Sfx.BSPSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.BspiPain, // painState
                128, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.BspiAtk1, // missileState
                State.BspiDie1, // deathState
                State.Null, // xdeathState
                Sfx.BSPDTH, // deathSound
                12, // speed
                Fixed.FromInt(64), // radius
                Fixed.FromInt(64), // height
                600, // mass
                0, // damage
                Sfx.BSPACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.BspiRaise1), // raiseState

            new MobjInfo( // MobjType.Cyborg
                16, // doomEdNum
                State.CyberStnd, // spawnState
                4000, // spawnHealth
                State.CyberRun1, // seeState
                Sfx.CYBSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.CyberPain, // painState
                20, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.CyberAtk1, // missileState
                State.CyberDie1, // deathState
                State.Null, // xdeathState
                Sfx.CYBDTH, // deathSound
                16, // speed
                Fixed.FromInt(40), // radius
                Fixed.FromInt(110), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Pain
                71, // doomEdNum
                State.PainStnd, // spawnState
                400, // spawnHealth
                State.PainRun1, // seeState
                Sfx.PESIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.PainPain, // painState
                128, // painChance
                Sfx.PEPAIN, // painSound
                State.Null, // meleeState
                State.PainAtk1, // missileState
                State.PainDie1, // deathState
                State.Null, // xdeathState
                Sfx.PEDTH, // deathSound
                8, // speed
                Fixed.FromInt(31), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity | MobjFlags.CountKill, // flags
                State.PainRaise1), // raiseState

            new MobjInfo( // MobjType.Wolfss
                84, // doomEdNum
                State.SswvStnd, // spawnState
                50, // spawnHealth
                State.SswvRun1, // seeState
                Sfx.SSSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.SswvPain, // painState
                170, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.SswvAtk1, // missileState
                State.SswvDie1, // deathState
                State.SswvXdie1, // xdeathState
                Sfx.SSDTH, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SswvRaise1), // raiseState

            new MobjInfo( // MobjType.Keen
                72, // doomEdNum
                State.Keenstnd, // spawnState
                100, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Keenpain, // painState
                256, // painChance
                Sfx.KEENPN, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Commkeen, // deathState
                State.Null, // xdeathState
                Sfx.KEENDT, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(72), // height
                10000000, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Bossbrain
                88, // doomEdNum
                State.Brain, // spawnState
                250, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.BrainPain, // painState
                255, // painChance
                Sfx.BOSPN, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.BrainDie1, // deathState
                State.Null, // xdeathState
                Sfx.BOSDTH, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                10000000, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Bossspit
                89, // doomEdNum
                State.Braineye, // spawnState
                1000, // spawnHealth
                State.Braineyesee, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(32), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Bosstarget
                87, // doomEdNum
                State.Null, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(32), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Spawnshot
                -1, // doomEdNum
                State.Spawn1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.BOSPIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(32), // height
                100, // mass
                3, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity | MobjFlags.NoClip, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Spawnfire
                -1, // doomEdNum
                State.Spawnfire1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Barrel
                2035, // doomEdNum
                State.Bar1, // spawnState
                20, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Bexp, // deathState
                State.Null, // xdeathState
                Sfx.BAREXP, // deathSound
                0, // speed
                Fixed.FromInt(10), // radius
                Fixed.FromInt(42), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.NoBlood, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Troopshot
                -1, // doomEdNum
                State.Tball1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Tballx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                3, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Headshot
                -1, // doomEdNum
                State.Rball1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Rballx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                5, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Rocket
                -1, // doomEdNum
                State.Rocket, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.RLAUNC, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Explode1, // deathState
                State.Null, // xdeathState
                Sfx.BAREXP, // deathSound
                20 * Fixed.FracUnit, // speed
                Fixed.FromInt(11), // radius
                Fixed.FromInt(8), // height
                100, // mass
                20, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Plasma
                -1, // doomEdNum
                State.Plasball, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.PLASMA, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Plasexp, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                25 * Fixed.FracUnit, // speed
                Fixed.FromInt(13), // radius
                Fixed.FromInt(8), // height
                100, // mass
                5, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Bfg
                -1, // doomEdNum
                State.Bfgshot, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Bfgland, // deathState
                State.Null, // xdeathState
                Sfx.RXPLOD, // deathSound
                25 * Fixed.FracUnit, // speed
                Fixed.FromInt(13), // radius
                Fixed.FromInt(8), // height
                100, // mass
                100, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Arachplaz
                -1, // doomEdNum
                State.ArachPlaz, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.PLASMA, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.ArachPlex, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                25 * Fixed.FracUnit, // speed
                Fixed.FromInt(13), // radius
                Fixed.FromInt(8), // height
                100, // mass
                5, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Puff
                -1, // doomEdNum
                State.Puff1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Blood
                -1, // doomEdNum
                State.Blood1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Tfog
                -1, // doomEdNum
                State.Tfog, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Ifog
                -1, // doomEdNum
                State.Ifog, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Teleportman
                14, // doomEdNum
                State.Null, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Extrabfg
                -1, // doomEdNum
                State.Bfgexp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc0
                2018, // doomEdNum
                State.Arm1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc1
                2019, // doomEdNum
                State.Arm2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc2
                2014, // doomEdNum
                State.Bon1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc3
                2015, // doomEdNum
                State.Bon2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc4
                5, // doomEdNum
                State.Bkey, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc5
                13, // doomEdNum
                State.Rkey, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc6
                6, // doomEdNum
                State.Ykey, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc7
                39, // doomEdNum
                State.Yskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc8
                38, // doomEdNum
                State.Rskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc9
                40, // doomEdNum
                State.Bskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc10
                2011, // doomEdNum
                State.Stim, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc11
                2012, // doomEdNum
                State.Medi, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc12
                2013, // doomEdNum
                State.Soul, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Inv
                2022, // doomEdNum
                State.Pinv, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc13
                2023, // doomEdNum
                State.Pstr, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Ins
                2024, // doomEdNum
                State.Pins, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc14
                2025, // doomEdNum
                State.Suit, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc15
                2026, // doomEdNum
                State.Pmap, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc16
                2045, // doomEdNum
                State.Pvis, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Mega
                83, // doomEdNum
                State.Mega, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Clip
                2007, // doomEdNum
                State.Clip, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc17
                2048, // doomEdNum
                State.Ammo, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc18
                2010, // doomEdNum
                State.Rock, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc19
                2046, // doomEdNum
                State.Brok, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc20
                2047, // doomEdNum
                State.Cell, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc21
                17, // doomEdNum
                State.Celp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc22
                2008, // doomEdNum
                State.Shel, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc23
                2049, // doomEdNum
                State.Sbox, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc24
                8, // doomEdNum
                State.Bpak, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc25
                2006, // doomEdNum
                State.Bfug, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Chaingun
                2002, // doomEdNum
                State.Mgun, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc26
                2005, // doomEdNum
                State.Csaw, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc27
                2003, // doomEdNum
                State.Laun, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc28
                2004, // doomEdNum
                State.Plas, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Shotgun
                2001, // doomEdNum
                State.Shot, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Supershotgun
                82, // doomEdNum
                State.Shot2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc29
                85, // doomEdNum
                State.Techlamp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc30
                86, // doomEdNum
                State.Tech2Lamp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc31
                2028, // doomEdNum
                State.Colu, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc32
                30, // doomEdNum
                State.Tallgrncol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc33
                31, // doomEdNum
                State.Shrtgrncol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc34
                32, // doomEdNum
                State.Tallredcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc35
                33, // doomEdNum
                State.Shrtredcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc36
                37, // doomEdNum
                State.Skullcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc37
                36, // doomEdNum
                State.Heartcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc38
                41, // doomEdNum
                State.Evileye, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc39
                42, // doomEdNum
                State.Floatskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc40
                43, // doomEdNum
                State.Torchtree, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc41
                44, // doomEdNum
                State.Bluetorch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc42
                45, // doomEdNum
                State.Greentorch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc43
                46, // doomEdNum
                State.Redtorch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc44
                55, // doomEdNum
                State.Btorchshrt, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc45
                56, // doomEdNum
                State.Gtorchshrt, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc46
                57, // doomEdNum
                State.Rtorchshrt, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc47
                47, // doomEdNum
                State.Stalagtite, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc48
                48, // doomEdNum
                State.Techpillar, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc49
                34, // doomEdNum
                State.Candlestik, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc50
                35, // doomEdNum
                State.Candelabra, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc51
                49, // doomEdNum
                State.Bloodytwitch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc52
                50, // doomEdNum
                State.Meat2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(84), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc53
                51, // doomEdNum
                State.Meat3, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(84), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc54
                52, // doomEdNum
                State.Meat4, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc55
                53, // doomEdNum
                State.Meat5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(52), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc56
                59, // doomEdNum
                State.Meat2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(84), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc57
                60, // doomEdNum
                State.Meat4, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc58
                61, // doomEdNum
                State.Meat3, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(52), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc59
                62, // doomEdNum
                State.Meat5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(52), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc60
                63, // doomEdNum
                State.Bloodytwitch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc61
                22, // doomEdNum
                State.HeadDie6, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc62
                15, // doomEdNum
                State.PlayDie7, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc63
                18, // doomEdNum
                State.PossDie5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc64
                21, // doomEdNum
                State.SargDie6, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc65
                23, // doomEdNum
                State.SkullDie6, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc66
                20, // doomEdNum
                State.TrooDie5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc67
                19, // doomEdNum
                State.SposDie5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc68
                10, // doomEdNum
                State.PlayXdie9, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc69
                12, // doomEdNum
                State.PlayXdie9, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc70
                28, // doomEdNum
                State.Headsonstick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc71
                24, // doomEdNum
                State.Gibs, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc72
                27, // doomEdNum
                State.Headonastick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc73
                29, // doomEdNum
                State.Headcandles, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc74
                25, // doomEdNum
                State.Deadstick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc75
                26, // doomEdNum
                State.Livestick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc76
                54, // doomEdNum
                State.Bigtree, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(32), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc77
                70, // doomEdNum
                State.Bbar1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc78
                73, // doomEdNum
                State.Hangnoguts, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(88), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc79
                74, // doomEdNum
                State.Hangbnobrain, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(88), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc80
                75, // doomEdNum
                State.Hangtlookdn, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc81
                76, // doomEdNum
                State.Hangtskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc82
                77, // doomEdNum
                State.Hangtlookup, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc83
                78, // doomEdNum
                State.Hangtnobrain, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc84
                79, // doomEdNum
                State.Colongibs, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc85
                80, // doomEdNum
                State.Smallpool, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null), // raiseState

            new MobjInfo( // MobjType.Misc86
                81, // doomEdNum
                State.Brainstem, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null), // raiseState
        };
    }
}
