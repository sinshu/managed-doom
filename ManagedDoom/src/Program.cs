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
            var players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player();
                players[i].PlayerState = PlayerState.Reborn;
            }
            players[0].InGame = true;

            var style = Styles.Close | Styles.Titlebar;
            using (var window = new RenderWindow(new VideoMode(640, 400), "Managed Doom", style))
            using (var resources = new Resources("DOOM2.WAD"))
            using (var renderer = new SoftwareRendering.Renderer(window, resources, true))
            {
                var world = new World(resources, "MAP01", new GameOptions(), players);
                //var demo = new Demo(resources, resources.Wad.ReadLump("DEMO1"));

                renderer.BindWorld(world);
                //renderer.BindWorld(demo.World);

                window.Closed += (sender, e) => window.Close();
                window.SetFramerateLimit(35);

                var sw = new Stopwatch();
                var count = 0;
                sw.Start();
                var prev = sw.Elapsed;
                while (window.IsOpen)
                {
                    window.DispatchEvents();

                    if (Keyboard.IsKeyPressed(Keyboard.Key.O)) world.Players[0].Cheats |= CheatFlags.NoClip;
                    if (Keyboard.IsKeyPressed(Keyboard.Key.P)) world.Players[0].Cheats &= ~CheatFlags.NoClip;

                    UserInput.BuildTicCmd(world.Players[0].Cmd);
                    world.Update();
                    //demo.Update();

                    //Console.WriteLine(world.levelTime + ": " + world.GetMobjHash().ToString("x8"));

                    renderer.Render();

                    count++;
                    var curr = sw.Elapsed;
                    var delta = curr - prev;
                    if (delta.TotalSeconds >= 1 && count >= 1)
                    {
                        var fps = count / delta.TotalSeconds;
                        //Console.WriteLine("FPS: " + fps.ToString("0.0"));
                        count = 0;
                        prev = curr;
                    }
                }
            }
        }
    }
}
