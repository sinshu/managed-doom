namespace AudioSynthesis.Wave
{
    using System;
    using System.IO;

    public sealed class WaveFileWriter : IDisposable
    {
        //--Fields
        private BinaryWriter writer;
        private string ftemp; //tmp file name
        private string fname; //output file name
        private Int32 length;
        private int channels;
        private int bits;
        private int sRate;

        //--Methods
        public WaveFileWriter(int sampleRate, int channels, int bitsPerSample, string fileName)
        {
            this.sRate = sampleRate;
            this.channels = channels;
            this.bits = bitsPerSample;
            this.fname = fileName;
            this.ftemp = GetTemporaryFileName(Path.GetDirectoryName(fileName));
            this.writer = new BinaryWriter(CrossPlatformHelper.CreateResource(ftemp));
        }
        public void Write(byte[] buffer)
        {
            writer.Write(buffer);
            length += buffer.Length;
        }
        public void Write(float[] buffer)
        {
            CrossPlatformHelper.Assert(channels == 1, "Mismatched channels! Output expects : " + channels + " channel.");
            if (buffer == null)
                throw new ArgumentException("One or more input buffers were null!");
            Write(WaveHelper.GetRawData(buffer, bits));
        }
        public void Write(float[] left_buffer, float[] right_buffer)
        {
            CrossPlatformHelper.Assert(channels == 2, "Mismatched channels! Output expects : " + channels + " channels.");
            if (left_buffer == null || right_buffer == null)
                throw new ArgumentException("One or more input buffers were null!");
            Write(WaveHelper.GetRawData(left_buffer, right_buffer, bits));
        }
        public void Close()
        {
            if (writer == null)
                return;
            writer.Close();
            writer = null;
            BinaryWriter bw2 = new BinaryWriter(CrossPlatformHelper.CreateResource(fname));
            bw2.Write((Int32)1179011410);
            bw2.Write((Int32)44 + length - 8);
            bw2.Write((Int32)1163280727);
            bw2.Write((Int32)544501094);
            bw2.Write((Int32)16);
            bw2.Write((Int16)1);
            bw2.Write((Int16)channels);
            bw2.Write((Int32)sRate);
            bw2.Write((Int32)(sRate * channels * (bits / 8)));
            bw2.Write((Int16)(channels * (bits / 8)));
            bw2.Write((Int16)bits);
            bw2.Write((Int32)1635017060);
            bw2.Write((Int32)length);
            using (BinaryReader br = new BinaryReader(CrossPlatformHelper.OpenResource(ftemp)))
            {
                byte[] buffer = new byte[1024];
                int count = br.Read(buffer, 0, buffer.Length);
                while (count > 0)
                {
                    bw2.Write(buffer, 0, count);
                    count = br.Read(buffer, 0, buffer.Length);
                }
            }
            bw2.Close();
            CrossPlatformHelper.RemoveResource(ftemp);
        }
        public void Dispose()
        {
            if (writer == null)
                return;
            Close();
        }

        private static string GetTemporaryFileName(string path)
        {
            int x = 0;
            string baseName = path + "Raw_Wave_Data_";
            while(CrossPlatformHelper.ResourceExists(baseName + x + ".dat"))
            { x++; }
            return baseName + x + ".dat";
        }
    }
}
