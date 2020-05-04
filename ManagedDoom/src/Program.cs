using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ManagedDoom
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var app = new DoomApplication())
            {
                app.Run();
            }
        }
    }
}
