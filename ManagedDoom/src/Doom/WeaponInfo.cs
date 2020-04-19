using System;

namespace ManagedDoom
{
    public sealed class WeaponInfo
    {
        public AmmoType Ammo;
        public MobjState UpState;
        public MobjState DownState;
        public MobjState ReadyState;
        public MobjState AttackState;
        public MobjState FlashState;

        public WeaponInfo(
            AmmoType ammo,
            MobjState upState,
            MobjState downState,
            MobjState readyState,
            MobjState attackState,
            MobjState flashState)
        {
            Ammo = ammo;
            UpState = upState;
            DownState = downState;
            ReadyState = readyState;
            AttackState = attackState;
            FlashState = flashState;
        }
    }
}
