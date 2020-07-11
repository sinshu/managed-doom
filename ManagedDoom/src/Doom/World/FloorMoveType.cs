using System;

namespace ManagedDoom
{
    public enum FloorMoveType
    {
        // Lower floor to highest surrounding floor.
        LowerFloor,

        // Lower floor to lowest surrounding floor.
        LowerFloorToLowest,

        // Lower floor to highest surrounding floor very fast.
        TurboLower,

        // Raise floor to lowest surrounding ceiling.
        RaiseFloor,

        // Raise floor to next highest surrounding floor.
        RaiseFloorToNearest,

        // Raise floor to shortest height texture around it.
        RaiseToTexture,

        // Lower floor to lowest surrounding floor and
        // change floor texture.
        LowerAndChange,

        RaiseFloor24,
        RaiseFloor24AndChange,
        RaiseFloorCrush,

        // Raise to next highest floor, turbo-speed.
        RaiseFloorTurbo,
        DonutRaise,
        RaiseFloor512
    }
}
