using System;

namespace ManagedDoom
{
    public sealed class WeaponInfo
    {
        public AmmoType Ammo;
        public State UpState;
        public State DownState;
        public State ReadyState;
        public State AttackState;
        public State FlashState;

        public WeaponInfo(
            AmmoType ammo,
            State upState,
            State downState,
            State readyState,
            State attackState,
            State flashState)
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
