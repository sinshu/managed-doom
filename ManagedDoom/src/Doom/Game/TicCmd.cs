using System;

namespace ManagedDoom
{
    public sealed class TicCmd
    {
        private sbyte forwardMove;
        private sbyte sideMove;
        private short angleTurn;
        private byte buttons;

        public void Clear()
        {
            forwardMove = 0;
            sideMove = 0;
            angleTurn = 0;
            buttons = 0;
        }

        public void CopyFrom(TicCmd cmd)
        {
            forwardMove = cmd.forwardMove;
            sideMove = cmd.sideMove;
            angleTurn = cmd.angleTurn;
            buttons = cmd.buttons;
        }

        public sbyte ForwardMove
        {
            get => forwardMove;
            set => forwardMove = value;
        }

        public sbyte SideMove
        {
            get => sideMove;
            set => sideMove = value;
        }

        public short AngleTurn
        {
            get => angleTurn;
            set => angleTurn = value;
        }

        public byte Buttons
        {
            get => buttons;
            set => buttons = value;
        }
    }
}
