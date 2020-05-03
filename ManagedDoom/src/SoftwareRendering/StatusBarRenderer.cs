using System;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class StatusBarRenderer
    {
        private static readonly int ST_HEIGHT = 32;
        private static readonly int ST_WIDTH = 320;
        private static readonly int ST_Y = 200 - ST_HEIGHT;

        // AMMO number pos.
        private static readonly int ST_AMMOWIDTH = 3;
        private static readonly int ST_AMMOX = 44;
        private static readonly int ST_AMMOY = 171;

        // HEALTH number pos.
        private static readonly int ST_HEALTHWIDTH = 3;
        private static readonly int ST_HEALTHX = 90;
        private static readonly int ST_HEALTHY = 171;

        // Weapon pos.
        private static readonly int ST_ARMSX = 111;
        private static readonly int ST_ARMSY = 172;
        private static readonly int ST_ARMSBGX = 104;
        private static readonly int ST_ARMSBGY = 168;
        private static readonly int ST_ARMSXSPACE = 12;
        private static readonly int ST_ARMSYSPACE = 10;

        // Frags pos.
        private static readonly int ST_FRAGSX = 138;
        private static readonly int ST_FRAGSY = 171;
        private static readonly int ST_FRAGSWIDTH = 2;

        // ARMOR number pos.
        private static readonly int ST_ARMORWIDTH = 3;
        private static readonly int ST_ARMORX = 221;
        private static readonly int ST_ARMORY = 171;

        // Key icon positions.
        private static readonly int ST_KEY0WIDTH = 8;
        private static readonly int ST_KEY0HEIGHT = 5;
        private static readonly int ST_KEY0X = 239;
        private static readonly int ST_KEY0Y = 171;
        private static readonly int ST_KEY1WIDTH = ST_KEY0WIDTH;
        private static readonly int ST_KEY1X = 239;
        private static readonly int ST_KEY1Y = 181;
        private static readonly int ST_KEY2WIDTH = ST_KEY0WIDTH;
        private static readonly int ST_KEY2X = 239;
        private static readonly int ST_KEY2Y = 191;

        // Ammunition counter.
        private static readonly int ST_AMMO0WIDTH = 3;
        private static readonly int ST_AMMO0HEIGHT = 6;
        private static readonly int ST_AMMO0X = 288;
        private static readonly int ST_AMMO0Y = 173;
        private static readonly int ST_AMMO1WIDTH = ST_AMMO0WIDTH;
        private static readonly int ST_AMMO1X = 288;
        private static readonly int ST_AMMO1Y = 179;
        private static readonly int ST_AMMO2WIDTH = ST_AMMO0WIDTH;
        private static readonly int ST_AMMO2X = 288;
        private static readonly int ST_AMMO2Y = 191;
        private static readonly int ST_AMMO3WIDTH = ST_AMMO0WIDTH;
        private static readonly int ST_AMMO3X = 288;
        private static readonly int ST_AMMO3Y = 185;

        // Indicate maximum ammunition.
        // Only needed because backpack exists.
        private static readonly int ST_MAXAMMO0WIDTH = 3;
        private static readonly int ST_MAXAMMO0HEIGHT = 5;
        private static readonly int ST_MAXAMMO0X = 314;
        private static readonly int ST_MAXAMMO0Y = 173;
        private static readonly int ST_MAXAMMO1WIDTH = ST_MAXAMMO0WIDTH;
        private static readonly int ST_MAXAMMO1X = 314;
        private static readonly int ST_MAXAMMO1Y = 179;
        private static readonly int ST_MAXAMMO2WIDTH = ST_MAXAMMO0WIDTH;
        private static readonly int ST_MAXAMMO2X = 314;
        private static readonly int ST_MAXAMMO2Y = 191;
        private static readonly int ST_MAXAMMO3WIDTH = ST_MAXAMMO0WIDTH;
        private static readonly int ST_MAXAMMO3X = 314;
        private static readonly int ST_MAXAMMO3Y = 185;

        // pistol
        private static readonly int ST_WEAPON0X = 110;
        private static readonly int ST_WEAPON0Y = 172;

        // shotgun
        private static readonly int ST_WEAPON1X = 122;
        private static readonly int ST_WEAPON1Y = 172;

        // chain gun
        private static readonly int ST_WEAPON2X = 134;
        private static readonly int ST_WEAPON2Y = 172;

        // missile launcher
        private static readonly int ST_WEAPON3X = 110;
        private static readonly int ST_WEAPON3Y = 181;

        // plasma gun
        private static readonly int ST_WEAPON4X = 122;
        private static readonly int ST_WEAPON4Y = 181;

        // bfg
        private static readonly int ST_WEAPON5X = 134;
        private static readonly int ST_WEAPON5Y = 181;

        // WPNS title
        private static readonly int ST_WPNSX = 109;
        private static readonly int ST_WPNSY = 191;

        // DETH title
        private static readonly int ST_DETHX = 109;
        private static readonly int ST_DETHY = 191;





        private DrawScreen screen;
        private CommonPatches patches;

        private int scale;

        private NumberWidget wReady;
        private PercentWidget wHealth;
        private PercentWidget wArmor;

        private NumberWidget[] wAmmo;
        private NumberWidget[] wMaxAmmo;

        public StatusBarRenderer(CommonPatches patches, DrawScreen screen)
        {
            this.screen = screen;
            this.patches = patches;

            scale = screen.Width / 320;

            wReady = new NumberWidget();
            wReady.Patches = patches.TallNumbers;
            wReady.Width = ST_AMMOWIDTH;
            wReady.X = ST_AMMOX;
            wReady.Y = ST_AMMOY;

            wHealth = new PercentWidget();
            wHealth.NumberWidget.Patches = patches.TallNumbers;
            wHealth.NumberWidget.Width = 3;
            wHealth.NumberWidget.X = ST_HEALTHX;
            wHealth.NumberWidget.Y = ST_HEALTHY;
            wHealth.Patch = patches.TallPercent;

            wArmor = new PercentWidget();
            wArmor.NumberWidget.Patches = patches.TallNumbers;
            wArmor.NumberWidget.Width = 3;
            wArmor.NumberWidget.X = ST_ARMORX;
            wArmor.NumberWidget.Y = ST_ARMORY;
            wArmor.Patch = patches.TallPercent;

            wAmmo = new NumberWidget[(int)AmmoType.Count];
            wMaxAmmo = new NumberWidget[(int)AmmoType.Count];

            wAmmo[0] = new NumberWidget();
            wAmmo[0].Patches = patches.ShortNumbers;
            wAmmo[0].Width = ST_AMMO0WIDTH;
            wAmmo[0].X = ST_AMMO0X;
            wAmmo[0].Y = ST_AMMO0Y;

            wAmmo[1] = new NumberWidget();
            wAmmo[1].Patches = patches.ShortNumbers;
            wAmmo[1].Width = ST_AMMO1WIDTH;
            wAmmo[1].X = ST_AMMO1X;
            wAmmo[1].Y = ST_AMMO1Y;

            wAmmo[2] = new NumberWidget();
            wAmmo[2].Patches = patches.ShortNumbers;
            wAmmo[2].Width = ST_AMMO2WIDTH;
            wAmmo[2].X = ST_AMMO2X;
            wAmmo[2].Y = ST_AMMO2Y;

            wAmmo[3] = new NumberWidget();
            wAmmo[3].Patches = patches.ShortNumbers;
            wAmmo[3].Width = ST_AMMO3WIDTH;
            wAmmo[3].X = ST_AMMO3X;
            wAmmo[3].Y = ST_AMMO3Y;

            wMaxAmmo[0] = new NumberWidget();
            wMaxAmmo[0].Patches = patches.ShortNumbers;
            wMaxAmmo[0].Width = ST_MAXAMMO0WIDTH;
            wMaxAmmo[0].X = ST_MAXAMMO0X;
            wMaxAmmo[0].Y = ST_MAXAMMO0Y;

            wMaxAmmo[1] = new NumberWidget();
            wMaxAmmo[1].Patches = patches.ShortNumbers;
            wMaxAmmo[1].Width = ST_MAXAMMO1WIDTH;
            wMaxAmmo[1].X = ST_MAXAMMO1X;
            wMaxAmmo[1].Y = ST_MAXAMMO1Y;

            wMaxAmmo[2] = new NumberWidget();
            wMaxAmmo[2].Patches = patches.ShortNumbers;
            wMaxAmmo[2].Width = ST_MAXAMMO2WIDTH;
            wMaxAmmo[2].X = ST_MAXAMMO2X;
            wMaxAmmo[2].Y = ST_MAXAMMO2Y;

            wMaxAmmo[3] = new NumberWidget();
            wMaxAmmo[3].Patches = patches.ShortNumbers;
            wMaxAmmo[3].Width = ST_MAXAMMO3WIDTH;
            wMaxAmmo[3].X = ST_MAXAMMO3X;
            wMaxAmmo[3].Y = ST_MAXAMMO3Y;
        }

        public void Render(Player player)
        {
            screen.DrawPatch(patches.StatusBar, 0, scale * (200 - 32), scale);

            wReady.Number = player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo];
            DrawNumber(wReady);

            wHealth.NumberWidget.Number = player.Health;
            DrawPercent(wHealth);

            wArmor.NumberWidget.Number = player.ArmorPoints;
            DrawPercent(wArmor);

            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                wAmmo[i].Number = player.Ammo[i];
                DrawNumber(wAmmo[i]);

                wMaxAmmo[i].Number = player.MaxAmmo[i];
                DrawNumber(wMaxAmmo[i]);
            }
        }

        private void DrawNumber(NumberWidget n)
        {
            var numdigits = n.Width;
            var num = n.Number;

            var w = n.Patches[0].Width;
            var h = n.Patches[0].Height;
            var x = n.X;

            var neg = n.Number < 0;

            if (neg)
            {
                if (numdigits == 2 && num < -9)
                {
                    num = -9;
                }
                else if (numdigits == 3 && num < -99)
                {
                    num = -99;
                }

                num = -num;
            }

            // clear the area
            x = n.X - numdigits * w;

            /*
            if (n.Y - ST_Y < 0)
            {
                throw new Exception("drawNum: n->y - ST_Y < 0");
            }
            */

            //V_CopyRect(x, n->y - ST_Y, BG, w * numdigits, h, x, n->y, FG);

            // if non-number, do not draw it
            if (num == 1994)
            {
                return;
            }

            x = n.X;

            // in the special case of 0, you draw 0
            if (num == 0)
            {
                screen.DrawPatch(n.Patches[0], scale * (x - w), scale * n.Y, scale);
            }

            // draw the new number
            while (num != 0 && numdigits-- != 0)
            {
                x -= w;
                screen.DrawPatch(n.Patches[num % 10], scale * x, scale * n.Y, scale);
                num /= 10;
            }

            // draw a minus sign if necessary
            if (neg)
            {
                screen.DrawPatch(patches.TallMinus, scale * (x - 8), scale * n.Y, scale);
            }
        }

        private void DrawPercent(PercentWidget per)
        {
            screen.DrawPatch(
                per.Patch,
                scale * per.NumberWidget.X,
                scale * per.NumberWidget.Y,
                scale);

            DrawNumber(per.NumberWidget);
        }



        private class NumberWidget
        {
            public int X;
            public int Y;
            public int Width;
            public int Number;
            public Patch[] Patches;
        }

        private class PercentWidget
        {
            public NumberWidget NumberWidget = new NumberWidget();
            public Patch Patch;
        }
    }
}
