using System;

namespace ManagedDoom
{
    public sealed class Intercept
    {
        private Fixed frac;
        private Mobj thing;
        private LineDef line;

        public void Make(Fixed frac, Mobj thing)
        {
            this.frac = frac;
            this.thing = thing;
            this.line = null;
        }

        public void Make(Fixed frac, LineDef line)
        {
            this.frac = frac;
            this.thing = null;
            this.line = line;
        }

        public Fixed Frac
        {
            get => frac;
            set => frac = value;
        }

        public Mobj Thing => thing;
        public LineDef Line => line;
    }
}
