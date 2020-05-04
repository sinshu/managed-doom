using System;
using System.Runtime.ExceptionServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using ManagedDoom.SoftwareRendering;

namespace ManagedDoom
{
    public sealed class DoomApplication : IDisposable
    {
        private RenderWindow window;
        private CommonResource resource;
        private SfmlRenderer renderer;
        private SfmlAudio audio;

        private Player[] players;
        private TicCmd[] cmds;
        private GameOptions options;
        private DoomGame game;

        public DoomApplication()
        {
            try
            {
                var style = Styles.Close | Styles.Titlebar;
                window = new RenderWindow(new VideoMode(640, 400), "Managed Doom", style);
                window.Clear(new Color(128, 128, 128));
                window.Display();

                resource = new CommonResource("DOOM2.WAD");
                renderer = new SoftwareRendering.SfmlRenderer(window, resource, true);
                audio = new SfmlAudio(resource.Wad);

                players = new Player[Player.MaxPlayerCount];
                cmds = new TicCmd[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    players[i] = new Player(i);
                    cmds[i] = new TicCmd();
                }
                players[0].InGame = true;

                options = new GameOptions();
                options.Skill = GameSkill.Hard;
                options.GameMode = resource.Wad.GameMode;
                options.Episode = 1;
                options.Map = 1;

                game = new DoomGame(players, resource, options);
                game.Audio = audio;

                window.Closed += (sender, e) => window.Close();
                window.SetFramerateLimit(35);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
        }

        public void Run()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                Update();
            }
        }

        private void Update()
        {
            UserInput.BuildTicCmd(cmds[0]);
            game.Update(cmds);
            renderer.Render(game);
        }

        public void Dispose()
        {
            if (audio != null)
            {
                audio.Dispose();
                audio = null;
            }

            if (renderer != null)
            {
                renderer.Dispose();
                renderer = null;
            }

            if (resource != null)
            {
                resource.Dispose();
                resource = null;
            }

            if (window != null)
            {
                window.Dispose();
                window = null;
            }
        }
    }
}
