using System;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class Wipe
    {
        private short[] y;
        private int height;
        private DoomRandom random;

        public Wipe(int width, int height)
        {
            y = new short[width];
            this.height = height;
            random = new DoomRandom(DateTime.Now.Millisecond);
        }

        public void Start()
        {
            y[0] = (short)(-(random.Next() % 16));
            for (var i = 1; i < y.Length; i++)
            {
                var r = (random.Next() % 3) - 1;
                y[i] = (short)(y[i - 1] + r);
                if (y[i] > 0)
                {
                    y[i] = 0;
                }
                else if (y[i] == -16)
                {
                    y[i] = -15;
                }
            }
        }

        public UpdateResult Update()
        {
            var done = true;

            for (var i = 0; i < y.Length; i++)
            {
                if (y[i] < 0)
                {
                    y[i]++;
                    done = false;
                }
                else if (y[i] < height)
                {
                    var dy = (y[i] < 16) ? y[i] + 1 : 8;
                    if (y[i] + dy >= height)
                    {
                        dy = height - y[i];
                    }
                    y[i] += (short)dy;
                    done = false;
                }
            }

            if (done)
            {
                return UpdateResult.Completed;
            }
            else
            {
                return UpdateResult.None;
            }
        }

        public short[] Y => y;
    }
}
