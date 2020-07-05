using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        public static readonly AnimationDef[] TextureAnimation = new AnimationDef[]
        {
            new AnimationDef(false, "NUKAGE3",  "NUKAGE1",  8),
            new AnimationDef(false, "FWATER4",  "FWATER1",  8),
            new AnimationDef(false, "SWATER4",  "SWATER1",  8),
            new AnimationDef(false, "LAVA4",    "LAVA1",    8),
            new AnimationDef(false, "BLOOD3",   "BLOOD1",   8),

            // DOOM II flat animations.
            new AnimationDef(false, "RROCK08",  "RROCK05",  8),
            new AnimationDef(false, "SLIME04",  "SLIME01",  8),
            new AnimationDef(false, "SLIME08",  "SLIME05",  8),
            new AnimationDef(false, "SLIME12",  "SLIME09",  8),

            new AnimationDef(true,  "BLODGR4",  "BLODGR1",  8),
            new AnimationDef(true,  "SLADRIP3", "SLADRIP1", 8),

            new AnimationDef(true,  "BLODRIP4", "BLODRIP1", 8),
            new AnimationDef(true,  "FIREWALL", "FIREWALA", 8),
            new AnimationDef(true,  "GSTFONT3", "GSTFONT1", 8),
            new AnimationDef(true,  "FIRELAVA", "FIRELAV3", 8),
            new AnimationDef(true,  "FIREMAG3", "FIREMAG1", 8),
            new AnimationDef(true,  "FIREBLU2", "FIREBLU1", 8),
            new AnimationDef(true,  "ROCKRED3", "ROCKRED1", 8),

            new AnimationDef(true,  "BFALL4",   "BFALL1",   8),
            new AnimationDef(true,  "SFALL4",   "SFALL1",   8),
            new AnimationDef(true,  "WFALL4",   "WFALL1",   8),
            new AnimationDef(true,  "DBRAIN4",  "DBRAIN1",  8)
        };
    }
}
