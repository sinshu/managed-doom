using System;
using SFML.System;
using SFML.Window;

namespace ManagedDoom
{
    public static class UserInput
    {
        private const int slowTurnTics = 6;

        private static readonly Fixed[] forwardmove =
        {
            new Fixed(0x19), new Fixed(0x32)
        };

        private static readonly Fixed[] sidemove =
        {
            new Fixed(0x18), new Fixed(0x28)
        };

        private static readonly Fixed[] angleturn =
        {
            new Fixed(640), new Fixed(1280), new Fixed(320) // + slow turn
        };

        private static readonly Fixed maxplmove = forwardmove[1];

        private static int turnheld;

        static UserInput()
        {
            turnheld = 0;
        }

        public static void BuildTicCmd(TicCmd cmd)
        {
            var keyLeft = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            var keyRight = Keyboard.IsKeyPressed(Keyboard.Key.Right);
            var keyUp = Keyboard.IsKeyPressed(Keyboard.Key.Up);
            var keyDown = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            var keySpeed = Keyboard.IsKeyPressed(Keyboard.Key.LShift);
            var keyStrafe = Keyboard.IsKeyPressed(Keyboard.Key.LAlt);
            var keyFire = Keyboard.IsKeyPressed(Keyboard.Key.LControl);
            var keyUse = Keyboard.IsKeyPressed(Keyboard.Key.Space);

            cmd.Clear();

            var strafe = keyStrafe;

            var speed = keySpeed ? 1 : 0;

            Fixed forward = Fixed.Zero;
            Fixed side = Fixed.Zero;

            if (keyLeft || keyRight)
            {
                turnheld++;
            }
            else
            {
                turnheld = 0;
            }

            int tspeed;
            if (turnheld < slowTurnTics)
            {
                tspeed = 2;
            }
            else
            {
                tspeed = speed;
            }

            if (strafe)
            {
                if (keyRight)
                {
                    side += sidemove[speed];
                }
                if (keyLeft)
                {
                    side -= sidemove[speed];
                }
            }
            else
            {
                if (keyRight)
                {
                    cmd.AngleTurn -= (short)angleturn[tspeed].Data;
                }
                if (keyLeft)
                {
                    cmd.AngleTurn += (short)angleturn[tspeed].Data;
                }
            }

            if (keyUp)
            {
                forward += forwardmove[speed];
            }
            if (keyDown)
            {
                forward -= forwardmove[speed];
            }

            if (keyFire)
            {
                cmd.Buttons |= Buttons.Attack;
            }

            if (keyUse)
            {
                cmd.Buttons |= Buttons.Use;
            }

            if (forward > maxplmove)
            {
                forward = maxplmove;
            }
            else if (forward < -maxplmove)
            {
                forward = -maxplmove;
            }
            if (side > maxplmove)
            {
                side = maxplmove;
            }
            else if (side < -maxplmove)
            {
                side = -maxplmove;
            }

            cmd.ForwardMove += (sbyte)forward.Data;
            cmd.SideMove += (sbyte)side.Data;
        }
    }
}
