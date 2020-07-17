using System;

namespace ManagedDoom
{
    public sealed class WeaponInfo
    {
        private AmmoType ammo;
        private MobjState upState;
        private MobjState downState;
        private MobjState readyState;
        private MobjState attackState;
        private MobjState flashState;

        public WeaponInfo(
            AmmoType ammo,
            MobjState upState,
            MobjState downState,
            MobjState readyState,
            MobjState attackState,
            MobjState flashState)
        {
            this.ammo = ammo;
            this.upState = upState;
            this.downState = downState;
            this.readyState = readyState;
            this.attackState = attackState;
            this.flashState = flashState;
        }

        public AmmoType Ammo => ammo;
        public MobjState UpState => upState;
        public MobjState DownState => downState;
        public MobjState ReadyState => readyState;
        public MobjState AttackState => attackState;
        public MobjState FlashState => flashState;
    }
}
