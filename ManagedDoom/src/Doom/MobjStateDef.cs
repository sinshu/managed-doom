using System;

namespace ManagedDoom
{
    public class MobjStateDef
    {
        public int Number;
        public Sprite Sprite;
        public int Frame;
        public int Tics;
        public Action<World, Player, PlayerSpriteDef> PlayerAction;
        public Action<World, Mobj> MobjAction;
        public MobjState Next;
        public int Misc1;
        public int Misc2;

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
            Number = number;

            Sprite = sprite;
            Frame = frame;
            Tics = tics;
            PlayerAction = playerAction;
            MobjAction = mobjAction;

            Next = next;
            Misc1 = misc1;
            Misc2 = misc2;
        }
    }
}
