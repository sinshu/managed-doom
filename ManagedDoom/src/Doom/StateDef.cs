using System;

namespace ManagedDoom
{
    public class StateDef
    {
        public Sprite Sprite;
        public int Frame;
        public int Tics;
        public Action<Player, PlayerSpriteDef> PlayerAction;
        public Action<Mobj> MobjAction;
        public State Next;
        public int Misc1;
        public int Misc2;

        public StateDef(
            Sprite sprite,
            int frame,
            int tics,
            Action<Player, PlayerSpriteDef> playerAction,
            Action<Mobj> mobjAction,
            State next,
            int misc1,
            int misc2)
        {
            Sprite = sprite;
            Frame = frame;
            Tics = tics;
            PlayerAction = playerAction;
            MobjAction = mobjAction;
            MobjAction = null;
            Next = next;
            Misc1 = misc1;
            Misc2 = misc2;
        }
    }
}
