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
            using (var resource = new CommonResource("DOOM2.WAD"))
            using (var renderer = new SoftwareRendering.Renderer(window, resource, true))
            {
                //var demo = new Demo(resource.Wad.ReadLump("DEMO1"));

                var options = new GameOptions();
                options.GameSkill = Skill.Hard;
                options.GameMode = GameMode.Commercial;
                options.Episode = 1;
                options.Map = 1;

                var world = new World(resource, options, players);
                //var world = new World(resource, demo.Options, demo.Players);

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

                    if (Keyboard.IsKeyPressed(Keyboard.Key.O)) world.Players[0].Cheats |= CheatFlags.NoClip;
                    if (Keyboard.IsKeyPressed(Keyboard.Key.P)) world.Players[0].Cheats &= ~CheatFlags.NoClip;

                    UserInput.BuildTicCmd(world.Players[0].Cmd);
                    //demo.ReadCmd();
                    world.Update();

                    //Console.WriteLine(world.levelTime + ": " + world.GetMobjHash().ToString("x8"));
                    //Console.WriteLine(world.levelTime + ": " + world.GetSectorHash().ToString("x8"));

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
