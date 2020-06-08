using System;

namespace ManagedDoom
{
    public static class SaveLoad
    {
        private enum ThinkerClass
        {
            End,
            Mobj
        }

        private enum Specials
        {
            Ceiling,
            Door,
            Floor,
            Plat,
            Flash,
            Strobe,
            Glow,
            EndSpecials
        }
    }
}
