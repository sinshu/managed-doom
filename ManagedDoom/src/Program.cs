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
            var style = Styles.Close | Styles.Titlebar;
            using (var window = new RenderWindow(new VideoMode(640, 400), "Managed Doom", style))
            {
                window.Clear(new Color(128, 128, 128));
                window.Display();

                using (var wad = new Wad("DOOM2.WAD"))
                {
                    var palette = new Palette(wad);
                    var colorMap = new ColorMap(wad);

                    var textures = new TextureLookup(wad);
                    var flats = new FlatLookup(wad);
                    var sprites = new SpriteLookup(wad);

                    var renderer = new SoftwareRendering.Renderer(
                        window,
                        palette,
                        colorMap,
                        textures,
                        flats,
                        sprites,
                        true);

                    var world = new World(textures, flats, wad, "MAP01");

                    renderer.BindWorld(world);

                    window.Closed += (sender, e) => window.Close();
                    window.SetFramerateLimit(35);

                    var sw = new Stopwatch();
                    var count = 0;
                    sw.Start();
                    var prev = sw.Elapsed;
                    while (window.IsOpen)
                    {
                        window.DispatchEvents();

                        var up = Keyboard.IsKeyPressed(Keyboard.Key.Up);
                        var dowm = Keyboard.IsKeyPressed(Keyboard.Key.Down);
                        var left = Keyboard.IsKeyPressed(Keyboard.Key.Left);
                        var right = Keyboard.IsKeyPressed(Keyboard.Key.Right);
                        world.Update(up, dowm, left, right);

                        renderer.Render();
                        count++;

                        var curr = sw.Elapsed;
                        var delta = curr - prev;
                        if (delta.TotalSeconds >= 3 && count >= 3)
                        {
                            var fps = count / delta.TotalSeconds;
                            Console.WriteLine("FPS: " + fps.ToString("0.0"));
                            count = 0;
                            prev = curr;
                        }
                    }
                }
            }
        }
    }
}
