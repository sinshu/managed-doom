using SFML.System;
using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class QuitConfirm : MenuDef
    {
        private DoomApplication app;
        private DoomRandom random;
        private string[] text;

        public QuitConfirm(DoomMenu menu, DoomApplication app) : base(menu)
        {
            this.app = app;
            random = new DoomRandom(DateTime.Now.Millisecond);
        }

        public override void Open()
        {
            IReadOnlyList<DoomString> list;
            if (app.Options.GameMode == GameMode.Commercial)
            {
                if (app.Options.MissionPack == MissionPack.Doom2)
                {
                    list = DoomInfo.QuitMessages.Doom2;
                }
                else
                {
                    list = DoomInfo.QuitMessages.FinalDoom;
                }
            }
            else
            {
                list = DoomInfo.QuitMessages.Doom;
            }

            text = (list[random.Next() % list.Count] + "\n\n" + DoomInfo.Strings.PRESSYN).Split('\n');
        }

        public override bool DoEvent(DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
            {
                return true;
            }

            if (e.Key == DoomKeys.Y ||
                e.Key == DoomKeys.Enter ||
                e.Key == DoomKeys.Space)
            {
                app.Quit();
            }

            if (e.Key == DoomKeys.N ||
                e.Key == DoomKeys.Escape)
            {
                Menu.Close();
            }

            return true;
        }

        public IReadOnlyList<string> Text => text;
    }
}
