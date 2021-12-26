using System;
using System.IO;
using Raylib_CsLo;

namespace ManagedDoom.Raylib_CsLo
{
    public static class RaylibConfigUtilities
    {
        public static RaylibMusic GetMusicInstance(Config config, Wad wad, int audioBufferSize)
        {
            var sfPath = Path.Combine(ConfigUtilities.GetExeDirectory(), config.audio_soundfont);
            if (File.Exists(sfPath))
            {
                return new RaylibMusic(config, wad, sfPath, audioBufferSize);
            }
            else
            {
                Console.WriteLine("SoundFont '" + config.audio_soundfont + "' was not found!");
                return null;
            }
        }
    }
}