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

        public static readonly int ST_FACESX = 143;
        public static readonly int ST_FACESY = 168;

        public static readonly int ST_FX = 143;
        public static readonly int ST_FY = 169;

        private DrawScreen screen;

        private Patches patches;

        private int scale;

        private NumberWidget wReady;
        private PercentWidget wHealth;
        private PercentWidget wArmor;

        private NumberWidget[] wAmmo;
        private NumberWidget[] wMaxAmmo;

        private MultIconWidget[] wArms;

        private NumberWidget wFrags;

        public StatusBarRenderer(Wad wad, DrawScreen screen)
        {
            this.screen = screen;

            patches = new Patches(wad);

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

            // weapons owned
            wArms = new MultIconWidget[6];
            for (var i = 0; i < wArms.Length; i++)
            {
                wArms[i] = new MultIconWidget();
                wArms[i].X = ST_ARMSX + (i % 3) * ST_ARMSXSPACE;
                wArms[i].Y = ST_ARMSY + (i / 3) * ST_ARMSYSPACE;
                wArms[i].Patches = patches.Arms[i];
            }

            wFrags = new NumberWidget();
            wFrags.Patches = patches.TallNumbers;
            wFrags.Width = ST_FRAGSWIDTH;
            wFrags.X = ST_FRAGSX;
            wFrags.Y = ST_FRAGSY;
        }

        public void Render(StatusBar statusBar, Player player)
        {
            screen.DrawPatch(patches.StatusBar, 0, scale * (200 - 32), scale);

            if (DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo != AmmoType.NoAmmo)
            {
                var num = player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo];
                DrawNumber(wReady, num);
            }

            DrawPercent(wHealth, player.Health);
            DrawPercent(wArmor, player.ArmorPoints);

            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                DrawNumber(wAmmo[i], player.Ammo[i]);
                DrawNumber(wMaxAmmo[i], player.MaxAmmo[i]);
            }

            if (player.Mobj.World.Options.Deathmatch == 0)
            {
                screen.DrawPatch(
                    patches.ArmsBg,
                    scale * ST_ARMSBGX,
                    scale * ST_ARMSBGY,
                    scale);

                for (var i = 0; i < wArms.Length; i++)
                {
                    DrawMultIcon(wArms[i], player.WeaponOwned[i + 1] ? 1 : 0);
                }
            }
            else
            {
                var sum = 0;
                for (var i = 0; i < player.Frags.Length; i++)
                {
                    sum += player.Frags[i];
                }
                DrawNumber(wFrags, sum);
            }

            if (player.Mobj.World.Options.NetGame)
            {
                screen.DrawPatch(
                    patches.FaceBacks[player.Number],
                    scale * ST_FX,
                    scale * ST_FY,
                    scale);
            }

            screen.DrawPatch(
                patches.Faces[statusBar.Face],
                scale * ST_FACESX,
                scale * ST_FACESY,
                scale);
        }

        private void DrawNumber(NumberWidget widget, int num)
        {
            var numdigits = widget.Width;

            var w = widget.Patches[0].Width;
            var h = widget.Patches[0].Height;
            var x = widget.X;

            var neg = num < 0;

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
            x = widget.X - numdigits * w;

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

            x = widget.X;

            // in the special case of 0, you draw 0
            if (num == 0)
            {
                screen.DrawPatch(widget.Patches[0], scale * (x - w), scale * widget.Y, scale);
            }

            // draw the new number
            while (num != 0 && numdigits-- != 0)
            {
                x -= w;
                screen.DrawPatch(widget.Patches[num % 10], scale * x, scale * widget.Y, scale);
                num /= 10;
            }

            // draw a minus sign if necessary
            if (neg)
            {
                screen.DrawPatch(patches.TallMinus, scale * (x - 8), scale * widget.Y, scale);
            }
        }

        private void DrawPercent(PercentWidget per, int num)
        {
            screen.DrawPatch(
                per.Patch,
                scale * per.NumberWidget.X,
                scale * per.NumberWidget.Y,
                scale);

            DrawNumber(per.NumberWidget, num);
        }

        private void DrawMultIcon(MultIconWidget mi, int num)
        {
            screen.DrawPatch(
                mi.Patches[num],
                scale * mi.X,
                scale * mi.Y,
                scale);
        }



        private class NumberWidget
        {
            public int X;
            public int Y;
            public int Width;
            public Patch[] Patches;
        }

        private class PercentWidget
        {
            public NumberWidget NumberWidget = new NumberWidget();
            public Patch Patch;
        }

        private class MultIconWidget
        {
            public int X;
            public int Y;
            public Patch[] Patches;
        }




        private class Patches
        {
            private Patch statusBar;
            private Patch[] tallNumbers;
            private Patch[] shortNumbers;
            private Patch tallMinus;
            private Patch tallPercent;
            private Patch[] keys;
            private Patch armsBg;
            private Patch[][] arms;
            private Patch[] faces;
            private Patch[] faceBacks;

            public Patches(Wad wad)
            {
                statusBar = Patch.FromWad("STBAR", wad);
                tallNumbers = new Patch[10];
                shortNumbers = new Patch[10];
                for (var i = 0; i < 10; i++)
                {
                    tallNumbers[i] = Patch.FromWad("STTNUM" + i, wad);
                    shortNumbers[i] = Patch.FromWad("STYSNUM" + i, wad);
                }
                tallMinus = Patch.FromWad("STTMINUS", wad);
                tallPercent = Patch.FromWad("STTPRCNT", wad);
                keys = new Patch[(int)CardType.Count];
                for (var i = 0; i < keys.Length; i++)
                {
                    keys[i] = Patch.FromWad("STKEYS" + i, wad);
                }
                armsBg = Patch.FromWad("STARMS", wad);
                arms = new Patch[6][];
                for (var i = 0; i < 6; i++)
                {
                    var num = i + 2;
                    arms[i] = new Patch[2];
                    arms[i][0] = Patch.FromWad("STGNUM" + num, wad);
                    arms[i][1] = shortNumbers[num];
                }
                faceBacks = new Patch[Player.MaxPlayerCount];
                for (var i = 0; i < faceBacks.Length; i++)
                {
                    faceBacks[i] = Patch.FromWad("STFB" + i, wad);
                }

                faces = new Patch[DoomInfo.FaceInfos.ST_NUMFACES];
                var facenum = 0;
                for (var i = 0; i < DoomInfo.FaceInfos.ST_NUMPAINFACES; i++)
                {
                    for (var j = 0; j < DoomInfo.FaceInfos.ST_NUMSTRAIGHTFACES; j++)
                    {
                        faces[facenum++] = Patch.FromWad("STFST" + i + j, wad);
                    }
                    faces[facenum++] = Patch.FromWad("STFTR" + i + "0", wad);
                    faces[facenum++] = Patch.FromWad("STFTL" + i + "0", wad);
                    faces[facenum++] = Patch.FromWad("STFOUCH" + i, wad);
                    faces[facenum++] = Patch.FromWad("STFEVL" + i, wad);
                    faces[facenum++] = Patch.FromWad("STFKILL" + i, wad);
                }
                faces[facenum++] = Patch.FromWad("STFGOD0", wad);
                faces[facenum++] = Patch.FromWad("STFDEAD0", wad);

                Console.WriteLine("Status bar patches are OK.");
            }





            public Patch StatusBar => statusBar;
            public Patch[] TallNumbers => tallNumbers;
            public Patch[] ShortNumbers => shortNumbers;
            public Patch TallMinus => tallMinus;
            public Patch TallPercent => tallPercent;
            public Patch[] Keys => keys;
            public Patch ArmsBg => armsBg;
            public Patch[][] Arms => arms;
            public Patch[] Faces => faces;
            public Patch[] FaceBacks => faceBacks;
        }
    }
}
