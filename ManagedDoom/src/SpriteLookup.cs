using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public sealed class SpriteLookup
    {
        private SpriteDef[] spriteDefs;

        public SpriteLookup(Wad wad)
        {
            var temp = new Dictionary<string, List<SpriteInfo>>();
            for (var i = 0; i < (int)Sprite.Count; i++)
            {
                temp.Add(Info.SpriteNames[i], new List<SpriteInfo>());
            }

            var cache = new Dictionary<int, Patch>();

            foreach (var lump in EnumerateSprites(wad))
            {
                var name = wad.LumpInfos[lump].Name.Substring(0, 4);

                if (!temp.ContainsKey(name))
                {
                    continue;
                }

                var list = temp[name];

                {
                    var frame = wad.LumpInfos[lump].Name[4] - 'A';
                    var rotation = wad.LumpInfos[lump].Name[5] - '0';

                    while (list.Count < frame + 1)
                    {
                        list.Add(new SpriteInfo());
                    }

                    if (rotation == 0)
                    {
                        for (var i = 0; i < 8; i++)
                        {
                            if (list[frame].Patches[i] == null)
                            {
                                list[frame].Patches[i] = CachedRead(lump, wad, cache);
                                list[frame].Flip[i] = false;
                            }
                        }
                    }
                    else
                    {
                        if (list[frame].Patches[rotation - 1] == null)
                        {
                            list[frame].Patches[rotation - 1] = CachedRead(lump, wad, cache);
                            list[frame].Flip[rotation - 1] = false;
                        }
                    }
                }

                if (wad.LumpInfos[lump].Name.Length == 8)
                {
                    var frame = wad.LumpInfos[lump].Name[6] - 'A';
                    var rotation = wad.LumpInfos[lump].Name[7] - '0';

                    while (list.Count < frame + 1)
                    {
                        list.Add(new SpriteInfo());
                    }

                    if (rotation == 0)
                    {
                        for (var i = 0; i < 8; i++)
                        {
                            if (list[frame].Patches[i] == null)
                            {
                                list[frame].Patches[i] = CachedRead(lump, wad, cache);
                                list[frame].Flip[i] = true;
                            }
                        }
                    }
                    else
                    {
                        if (list[frame].Patches[rotation - 1] == null)
                        {
                            list[frame].Patches[rotation - 1] = CachedRead(lump, wad, cache);
                            list[frame].Flip[rotation - 1] = true;
                        }
                    }
                }
            }

            spriteDefs = new SpriteDef[(int)Sprite.Count];
            for (var i = 0; i < spriteDefs.Length; i++)
            {
                var list = temp[Info.SpriteNames[i]];

                var frames = new SpriteFrame[list.Count];
                for (var j = 0; j < frames.Length; j++)
                {
                    list[j].CheckCompletion();

                    var frame = new SpriteFrame(list[j].HasRotation(), list[j].Patches, list[j].Flip);
                    frames[j] = frame;
                }

                spriteDefs[i] = new SpriteDef(frames);
            }
        }

        public void DumpInfo()
        {
            Console.WriteLine("Count: " + spriteDefs.Length);
            Console.WriteLine();

            for (var i = 0; i < (int)Sprite.Count; i++)
            {
                var spriteDef = spriteDefs[i];
                Console.WriteLine(Info.SpriteNames[i]);
                for (var j = 0; j < spriteDef.Frames.Length; j++)
                {
                    var frame = spriteDef.Frames[j];
                    Console.Write("Frame " + (char)('A' + j) + ":");
                    if (frame.Rotate)
                    {
                        for (var k = 0; k < 8; k++)
                        {
                            Console.Write(" ");
                            if (frame.Flip[k])
                            {
                                Console.Write("!");
                            }
                            Console.Write(frame.Patches[k].Name);
                        }
                    }
                    else
                    {
                        Console.Write(" ");
                        if (frame.Flip[0])
                        {
                            Console.Write("!");
                        }
                        Console.Write(frame.Patches[0].Name);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

        private static IEnumerable<int> EnumerateSprites(Wad wad)
        {
            var spriteSection = false;

            for (var lump = wad.LumpInfos.Count - 1; lump >= 0; lump--)
            {
                var name = wad.LumpInfos[lump].Name;

                if (name.StartsWith("S"))
                {
                    if (name.EndsWith("_END"))
                    {
                        spriteSection = true;
                        continue;
                    }
                    else if (name.EndsWith("_START"))
                    {
                        spriteSection = false;
                        continue;
                    }
                }

                if (spriteSection)
                {
                    if (wad.LumpInfos[lump].Size > 0)
                    {
                        yield return lump;
                    }
                }
            }
        }

        private static Patch CachedRead(int lump, Wad wad, Dictionary<int, Patch> cache)
        {
            if (!cache.ContainsKey(lump))
            {
                var name = wad.LumpInfos[lump].Name;
                cache.Add(lump, Patch.FromData(name, wad.ReadLump(lump)));
            }

            return cache[lump];
        }



        private class SpriteInfo
        {
            public Patch[] Patches;
            public bool[] Flip;

            public SpriteInfo()
            {
                Patches = new Patch[8];
                Flip = new bool[8];
            }

            public void CheckCompletion()
            {
                for (var i = 0; i < Patches.Length; i++)
                {
                    if (Patches[i] == null)
                    {
                        throw new Exception("Missing sprite!");
                    }
                }
            }

            public bool HasRotation()
            {
                for (var i = 1; i < Patches.Length; i++)
                {
                    if (Patches[i] != Patches[0])
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
