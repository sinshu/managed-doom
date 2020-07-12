using System;

namespace ManagedDoom
{
    public class MobjStateDef
    {
        private int number;
        private Sprite sprite;
        private int frame;
        private int tics;
        private Action<World, Player, PlayerSpriteDef> playerAction;
        private Action<World, Mobj> mobjAction;
        private MobjState next;
        private int misc1;
        private int misc2;

        public MobjStateDef(
            int number,
            Sprite sprite,
            int frame,
            int tics,
            Action<World, Player, PlayerSpriteDef> playerAction,
            Action<World, Mobj> mobjAction,
            MobjState next,
            int misc1,
            int misc2)
        {
            this.number = number;
            this.sprite = sprite;
            this.frame = frame;
            this.tics = tics;
            this.playerAction = playerAction;
            this.mobjAction = mobjAction;
            this.next = next;
            this.misc1 = misc1;
            this.misc2 = misc2;
        }

        public int Number => number;
        public Sprite Sprite => sprite;
        public int Frame => frame;
        public int Tics => tics;
        public Action<World, Player, PlayerSpriteDef> PlayerAction => playerAction;
        public Action<World, Mobj> MobjAction => mobjAction;
        public MobjState Next => next;
        public int Misc1 => misc1;
        public int Misc2 => misc2;
    }
}
