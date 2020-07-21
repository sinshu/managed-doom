using System;

namespace ManagedDoom
{
    public sealed class HelpScreen : MenuDef
    {
        public HelpScreen(DoomMenu menu) : base(menu)
        {
        }

        public override bool DoEvent(DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
            {
                return true;
            }

            if (e.Key == DoomKeys.Escape)
            {
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
            }

            return true;
        }
    }
}
