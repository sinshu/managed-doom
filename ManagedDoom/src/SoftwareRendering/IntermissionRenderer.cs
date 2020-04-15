using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class IntermissionRenderer
    {
        private DrawScreen screen;

        private int ratio;

        public IntermissionRenderer(CommonPatches patches, DrawScreen screen)
        {
            this.screen = screen;

            ratio = screen.Width / 320;
        }
    }
}
