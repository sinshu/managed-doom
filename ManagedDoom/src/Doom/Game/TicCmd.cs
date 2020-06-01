using System;

namespace ManagedDoom
{
    public sealed class TicCmd
    {
        public sbyte ForwardMove;   // *2048 for move
        public sbyte SideMove;  // *2048 for move
        public short AngleTurn;    // <<16 for angle delta
        public short Consistancy;  // checks for net game
        public byte ChatChar;
        public byte Buttons;

        public void Clear()
        {
            ForwardMove = 0;
            SideMove = 0;
            AngleTurn = 0;
            Consistancy = 0;
            ChatChar = 0;
            Buttons = 0;
        }

        public void CopyFrom(TicCmd cmd)
        {
            ForwardMove = cmd.ForwardMove;
            SideMove = cmd.SideMove;
            AngleTurn = cmd.AngleTurn;
            Consistancy = cmd.Consistancy;
            ChatChar = cmd.ChatChar;
            Buttons = cmd.Buttons;
        }
    }
}
