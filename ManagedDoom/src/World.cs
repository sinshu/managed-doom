using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public class World
    {
        private Map map;

        private Vertex player;
        private Angle playerViewAngle;

        public World(TextureLookup textures, FlatLookup flats, Wad wad, string mapName)
        {
            map = new Map(textures, flats, wad, mapName);

            var playerThing = map.Things.First(t => (int)t.Type == 1);
            player = new Vertex(playerThing.X, playerThing.Y);
            playerViewAngle = playerThing.Facing;

            var viewZ = GetSector(player.X, player.Y).FloorHeight + Fixed.FromInt(41);
        }

        public void Update(bool up, bool down, bool left, bool right)
        {
            var speed = 8;

            if (up)
            {
                var x = player.X + speed * Trig.Cos(playerViewAngle);
                var y = player.Y + speed * Trig.Sin(playerViewAngle);
                player = new Vertex(x, y);
            }

            if (down)
            {
                var x = player.X - speed * Trig.Cos(playerViewAngle);
                var y = player.Y - speed * Trig.Sin(playerViewAngle);
                player = new Vertex(x, y);
            }

            if (left)
            {
                playerViewAngle += Angle.FromDegree(3);
            }

            if (right)
            {
                playerViewAngle -= Angle.FromDegree(3);
            }
        }

        private Sector GetSector(Fixed x, Fixed y)
        {
            var node = map.Nodes.Last();
            Subsector target = null;
            while (true)
            {
                if (Geometry.PointOnSide(x, y, node) == 0)
                {
                    if (Node.IsSubsector(node.Children[0]))
                    {
                        target = map.Subsectors[Node.GetSubsector(node.Children[0])];
                        break;
                    }
                    else
                    {
                        node = map.Nodes[node.Children[0]];
                    }
                }
                else
                {
                    if (Node.IsSubsector(node.Children[1]))
                    {
                        target = map.Subsectors[Node.GetSubsector(node.Children[1])];
                        break;
                    }
                    else
                    {
                        node = map.Nodes[node.Children[1]];
                    }
                }
            }
            var sec = target.Sector;
            return sec;
        }

        public Map Map => map;
        public Fixed ViewX => player.X;
        public Fixed ViewY => player.Y;
        public Fixed ViewZ => GetSector(player.X, player.Y).FloorHeight + Fixed.FromInt(41);
        public Angle ViewAngle => playerViewAngle;
    }
}
