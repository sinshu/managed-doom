using System;

namespace ManagedDoom
{
    public class VlDoor : Thinker
    {
        public VlDoorType Type;
        public Sector Sector;
        Fixed TopHeight;
        Fixed Speed;

        // 1 = up, 0 = waiting at top, -1 = down
        public int Direction;

        // tics to wait at the top
        public int TopWait;

        // (keep in case a door going down is reset)
        // when it reaches 0, start going down
        public int TopCountDown;

        public VlDoor(World world) : base(world)
        {
        }
    }
}
