using System;

namespace ManagedDoom
{
    public enum FloorMoveType
    {
        // lower floor to highest surrounding floor
        LowerFloor,

        // lower floor to lowest surrounding floor
        LowerFloorToLowest,

        // lower floor to highest surrounding floor VERY FAST
        TurboLower,

        // raise floor to lowest surrounding CEILING
        RaiseFloor,

        // raise floor to next highest surrounding floor
        RaiseFloorToNearest,

        // raise floor to shortest height texture around it
        RaiseToTexture,

        // lower floor to lowest surrounding floor
        //  and change floorpic
        LowerAndChange,

        RaiseFloor24,
        RaiseFloor24AndChange,
        RaiseFloorCrush,

        // raise to next highest floor, turbo-speed
        RaiseFloorTurbo,
        DonutRaise,
        RaiseFloor512
    }
}
