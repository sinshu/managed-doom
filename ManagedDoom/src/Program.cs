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
                players[i] = new Player(i);
                players[i].PlayerState = PlayerState.Reborn;
            }
            players[0].InGame = true;

            var style = Styles.Close | Styles.Titlebar;
            using (var window = new RenderWindow(new VideoMode(640, 400), "Managed Doom", style))
            using (var resource = new CommonResource("DOOM2.WAD"))
            using (var renderer = new SoftwareRendering.SfmlRenderer(window, resource, true))
            //using (var audio = new SfmlAudio(resource.Wad))
            {
                var options = new GameOptions();
                options.Skill = GameSkill.Hard;
                options.GameMode = GameMode.Commercial;
                options.Episode = 1;
                options.Map = 1;

                var game = new DoomGame(resource, options);

                window.Closed += (sender, e) => window.Close();
                window.SetFramerateLimit(35);

                var sw = new Stopwatch();
                var count = 0;
                sw.Start();
                var prev = sw.Elapsed;
                while (window.IsOpen)
                {
                    window.DispatchEvents();

                    UserInput.BuildTicCmd(game.Players[0].Cmd);
                    game.Update();
                    renderer.Render(game);

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
