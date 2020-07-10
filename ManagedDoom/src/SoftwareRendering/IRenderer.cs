using System;

namespace ManagedDoom.SoftwareRendering
{
    public interface IRenderer
    {
        public int MaxWindowSize { get; }
        public int WindowSize { get; set; }
        public int MaxGammaCorrectionLevel { get; }
        public int GammaCorrectionLevel { get; set; }
    }
}
