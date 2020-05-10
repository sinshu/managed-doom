using System;

namespace ManagedDoom
{
    public sealed class Menu
    {


        private MenuDef mainDef;

        // menu item skull is on
        private int itemOn;

        // skull animation counter
        private int skullAnimCounter;

        // which skull to draw
        private int whichSkull;

        public Menu()
        {
            mainDef = new MenuDef();
            mainDef.NumItems = (int)MainMenuChoice.MainEnd;
            mainDef.PrevMenu = null;
            mainDef.MenuItems.Add(new MenuItem(1, "M_NGAME", M_NewGame, 'n'));
            mainDef.MenuItems.Add(new MenuItem(1, "M_OPTION", M_Options, 'o'));
            mainDef.MenuItems.Add(new MenuItem(1, "M_LOADG", M_LoadGame, 'l'));
            mainDef.MenuItems.Add(new MenuItem(1, "M_RDTHIS", M_ReadThis, 'r'));
            mainDef.MenuItems.Add(new MenuItem(1, "M_QUITG", M_QuitDOOM, 'q'));
            mainDef.Routine = M_DrawMainMenu;
            mainDef.X = 97;
            mainDef.Y = 64;
            mainDef.LastOn = 0;
        }

        private void M_NewGame(int choice)
        {
        }

        private void M_Episode(int choice)
        {
        }

        private void M_ChooseSkill(int choice)
        {
        }

        private void M_LoadGame(int choice)
        {
        }

        private void M_SaveGame(int choice)
        {
        }

        private void M_Options(int choice)
        {
        }

        private void M_EndGame(int choice)
        {
        }

        private void M_ReadThis(int choice)
        {
        }

        private void M_ReadThis2(int choice)
        {
        }

        private void M_QuitDOOM(int choice)
        {
        }




        //
        // M_DrawMainMenu
        //
        private void M_DrawMainMenu()
        {
            //V_DrawPatchDirect(94, 2, 0, W_CacheLumpName("M_DOOM", PU_CACHE));
        }
    }
}
