using System;
using SFML.System;
using SFML.Window;

namespace ManagedDoom
{
    public static class UserInput
    {
        private static int turnheld;
        private static int next_weapon;
        private static bool[] weaponKeys;

        static UserInput()
        {
            turnheld = 0;
            next_weapon = 0;
            weaponKeys = new bool[7];
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
            weaponKeys[0] = Keyboard.IsKeyPressed(Keyboard.Key.Num1);
            weaponKeys[1] = Keyboard.IsKeyPressed(Keyboard.Key.Num2);
            weaponKeys[2] = Keyboard.IsKeyPressed(Keyboard.Key.Num3);
            weaponKeys[3] = Keyboard.IsKeyPressed(Keyboard.Key.Num4);
            weaponKeys[4] = Keyboard.IsKeyPressed(Keyboard.Key.Num5);
            weaponKeys[5] = Keyboard.IsKeyPressed(Keyboard.Key.Num6);
            weaponKeys[6] = Keyboard.IsKeyPressed(Keyboard.Key.Num7);

            cmd.Clear();

            var strafe = keyStrafe;

            var speed = keySpeed ? 1 : 0;

            var forward = 0;
            var side = 0;

            if (keyLeft || keyRight)
            {
                turnheld++;
            }
            else
            {
                turnheld = 0;
            }

            int tspeed;
            if (turnheld < PlayerBehavior.SlowTurnTics)
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
                    side += PlayerBehavior.SideMove[speed];
                }
                if (keyLeft)
                {
                    side -= PlayerBehavior.SideMove[speed];
                }
            }
            else
            {
                if (keyRight)
                {
                    cmd.AngleTurn -= (short)PlayerBehavior.AngleTurn[tspeed];
                }
                if (keyLeft)
                {
                    cmd.AngleTurn += (short)PlayerBehavior.AngleTurn[tspeed];
                }
            }

            if (keyUp)
            {
                forward += PlayerBehavior.ForwardMove[speed];
            }
            if (keyDown)
            {
                forward -= PlayerBehavior.ForwardMove[speed];
            }

            if (keyFire)
            {
                cmd.Buttons |= TicCmdButtons.Attack;
            }

            if (keyUse)
            {
                cmd.Buttons |= TicCmdButtons.Use;
            }

            // If the previous or next weapon button is pressed, the
            // next_weapon variable is set to change weapons when
            // we generate a ticcmd.  Choose a new weapon.

            /*
            if (gamestate == GS_LEVEL && next_weapon != 0)
            {
                i = G_NextWeapon(next_weapon);
                cmd->buttons |= BT_CHANGE;
                cmd->buttons |= i << BT_WEAPONSHIFT;
            }
            else
            */
            {
                // Check weapon keys.
                for (var i = 0; i < weaponKeys.Length; i++)
                {
                    if (weaponKeys[i])
                    {
                        cmd.Buttons |= TicCmdButtons.Change;
                        cmd.Buttons |= (byte)(i << TicCmdButtons.WeaponShift);
                        break;
                    }
                }
            }

            next_weapon = 0;

            if (forward > PlayerBehavior.MaxMove)
            {
                forward = PlayerBehavior.MaxMove;
            }
            else if (forward < -PlayerBehavior.MaxMove)
            {
                forward = -PlayerBehavior.MaxMove;
            }
            if (side > PlayerBehavior.MaxMove)
            {
                side = PlayerBehavior.MaxMove;
            }
            else if (side < -PlayerBehavior.MaxMove)
            {
                side = -PlayerBehavior.MaxMove;
            }

            cmd.ForwardMove += (sbyte)forward;
            cmd.SideMove += (sbyte)side;
        }
    }
}
