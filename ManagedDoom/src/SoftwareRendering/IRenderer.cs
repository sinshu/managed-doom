using System;

namespace ManagedDoom.SoftwareRendering
{
    public interface IRenderer
    {
        public int MaxWindowSize { get; }
        public int WindowSize { get; set; }
    }
}
