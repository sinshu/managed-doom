using System;
using System.Diagnostics;
using System.IO;

namespace ManagedDoom
{
    public sealed class SaveSlots
    {
        private static readonly int slotCount = 6;
        private static readonly int descriptionSize = 24;

        private string[] slots;

        private void ReadSlots()
        {
            slots = new string[slotCount];

            var directory = GetSaveDirectory();
            var buffer = new byte[descriptionSize];
            for (var i = 0; i < slots.Length; i++)
            {
                var file = Path.Combine(directory, "doomsav" + i + ".dsg");
                if (File.Exists(file))
                {
                    using (var reader = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        reader.Read(buffer, 0, buffer.Length);
                        slots[i] = DoomInterop.ToString(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        private string GetSaveDirectory()
        {
            return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        public string this[int number]
        {
            get
            {
                if (slots == null)
                {
                    ReadSlots();
                }

                return slots[number];
            }

            set
            {
                slots[number] = value;
            }
        }

        public int Count => slots.Length;
    }
}
