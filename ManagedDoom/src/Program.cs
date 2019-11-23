using System;
using System.Collections.Generic;
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
            using (var texture = new SFML.Graphics.Texture(256, 512))
            using (var sprite = new Sprite(texture, new IntRect(0, 0, 200, 320)))
            using (var wad = new Wad("DOOM2.WAD"))
            {
                var world = new World(wad, "MAP01");

                var renderer = new SoftwareRenderer(wad);

                window.Closed += (sender, e) => window.Close();
                window.SetFramerateLimit(35);

                sprite.Position = new Vector2f(0, 0);
                sprite.Rotation = 90;
                sprite.Scale = new Vector2f(2, -2);

                while (window.IsOpen)
                {
                    window.DispatchEvents();

                    var up = Keyboard.IsKeyPressed(Keyboard.Key.Up);
                    var dowm = Keyboard.IsKeyPressed(Keyboard.Key.Down);
                    var left = Keyboard.IsKeyPressed(Keyboard.Key.Left);
                    var right = Keyboard.IsKeyPressed(Keyboard.Key.Right);
                    world.Update(up, dowm, left, right);
                    renderer.Render(world);
                    texture.Update(renderer.GlTextureData, 200, 320, 0, 0);
                    window.Draw(sprite);
                    window.Display();
                }
            }
        }
    }
}
