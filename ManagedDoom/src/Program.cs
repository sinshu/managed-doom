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
                    UserInput.BuildTicCmd(world.Players[0].Cmd);
                    world.Update(up, dowm, left, right);

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
