using System;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Bank.Descriptors;
using AudioSynthesis.Bank.Components;
using AudioSynthesis.Bank.Components.Generators;

namespace AudioSynthesis.Util
{
    public static class Tables
    {
        internal static readonly float[][] EnvelopeTables;
        internal static readonly double[] SemitoneTable;
        internal static readonly double[] CentTable;
        internal static readonly float[] SincTable; 

        /*Creates tables statically*/
        static Tables()
        {
            const int EnvelopeSize = 64;
            const double ExponentialCoeff = .09;
            EnvelopeTables = new float[4][];  
            EnvelopeTables[0] = RemoveDenormals(CreateSustainTable(EnvelopeSize));
            EnvelopeTables[1] = RemoveDenormals(CreateLinearTable(EnvelopeSize));
            EnvelopeTables[2] = RemoveDenormals(CreateExponentialTable(EnvelopeSize, ExponentialCoeff));
            EnvelopeTables[3] = RemoveDenormals(CreateSineTable(EnvelopeSize));
            CentTable = CreateCentTable();
            SemitoneTable = CreateSemitoneTable();
            SincTable = CreateSincTable(Synthesizer.SincWidth, Synthesizer.SincResolution, .43, HammingWindow);
        }

        public static float[] CreateSquareTable(int size, int k)
        {//Uses Fourier Expansion up to k terms 
            const double FOUR_OVER_PI = 4 / Math.PI;
            float[] squaretable = new float[size];
            double inc = 1.0 / size;
            double phase = 0;
            for (int x = 0; x < squaretable.Length; x++)
            {
                double value = 0.0;
                for (int i = 1; i <= k; i++)
                {
                    int twokminus1 = (2 * i) - 1;
                    value += Math.Sin(Synthesizer.TwoPi * (twokminus1) * phase) / (twokminus1);
                }
                squaretable[x] = SynthHelper.Clamp((float)(FOUR_OVER_PI * value),-1f,1f);
                phase += inc;
            }
            return squaretable;
        }
        public static float[] CreateTable(int size, WaveformEnum type)
        {
            Generator generator;
            if (type == WaveformEnum.Sine)
                generator = new SineGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.Square)
                generator = new SquareGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.Triangle)
                generator = new TriangleGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.Saw)
                generator = new SawGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.WhiteNoise)
                generator = new WhiteNoiseGenerator(new GeneratorDescriptor());
            else
                return null;
            float[] table = new float[size];
            double phase, increment;
            phase = generator.StartPhase;
            increment = generator.Period / size;
            for (int x = 0; x < table.Length; x++)
            {
                table[x] = generator.GetValue(phase);
                phase += increment;
            }
            return table;
        }

        /*Cent table contains 2^12 ratio for pitches in the range of (-1 to +1) semitone.
          Accuracy between semitones is 1/100th of a note or 1 cent. */
        public static double[] CreateCentTable()
        {//-100 to 100 cents
            double[] cents = new double[201];
            for(int x = 0; x < cents.Length; x++)
            {
                cents[x] = Math.Pow(2.0, (x - 100.0) / 1200.0);
            }
            return cents;
        }
             
        /*Semitone table contains pitches for notes in range of -127 to 127 semitones.
          Used to calculate base pitch when voice is started. ex. (basepitch = semiTable[midinote - rootkey]) */
        public static double[] CreateSemitoneTable()
        {//-127 to 127 semitones
            double[] table = new double[255];
            for (int x = 0; x < table.Length; x++)
            {
                table[x] = Math.Pow(2.0, (x - 127.0) / 12.0);
            }
            return table;
        }
        
        /*Envelope Equations*/
        public static float[] CreateSustainTable(int size)
        {
            float[] graph = new float[size];
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = 1;
            }
            return graph;
        }
        public static float[] CreateLinearTable(int size)
        {
            float[] graph = new float[size];
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = x / (float)(size - 1);
            }
            return graph;
        }
        public static float[] CreateExponentialTable(int size, double coeff)
        {
            coeff = SynthHelper.Clamp(coeff, .001, .9);
            float[] graph = new float[size];
            double val = 0;
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = (float)val;
                val += coeff * ((1 / 0.63) - val);
            }
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = graph[x] / graph[graph.Length - 1];
            }
            return graph;
        }
        public static float[] CreateSineTable(int size)
        {
            float[] graph = new float[size];
            double inc = (3 * Math.PI / 2) / (size - 1);
            double phase = 0;
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = (float)Math.Abs(Math.Sin(phase));
                phase += inc;
            }
            return graph;
        }
        private static float[] RemoveDenormals(float[] data)
        {
            for (int x = 0; x < data.Length; x++)
            {
                if (Math.Abs(data[x]) < Synthesizer.DenormLimit)
                    data[x] = 0f;
            }
            return data;
        }
        /*Sinc windowing methods*/
        public static double VonHannWindow(double i, int size)
        {
            return 0.5 - 0.5 * Math.Cos(Synthesizer.TwoPi * (0.5 + i / size));
        }
        public static double HammingWindow(double i, int size)
        {
            return 0.54 - 0.46 * Math.Cos(Synthesizer.TwoPi * i / size);  
        }
        public static double BlackmanWindow(double i, int size)
        {
            return 0.42659 - 0.49656 * Math.Cos(Synthesizer.TwoPi * i / size) + 0.076849 * Math.Cos(4.0 * Math.PI * i / size);
        }
        /*Sinc Table Creation
           windowSize: determines the number of points that are interpolated.
           resolution: determines the acuracy of the calculation of values inbetween each window point.
          cornerRatio: is the cutoff / sampleRate.
            windoFunc: the function to use as the windowing function.*/
        public static float[] CreateSincTable(int windowSize, int resolution, double cornerRatio, Func<double, int, double> windowFunction)
        {
            int subWindow = (windowSize / 2) + 1; //sub window is 0 to windowSize / 2
            float[] table = new float[subWindow * resolution]; //only half of a table is needed
            double gain = 2.0 * cornerRatio; //calculate gain correction factor
            for (int x = 0; x < subWindow; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    double a = x + (y / (double)resolution);
                    double sinc = Synthesizer.TwoPi * cornerRatio * a;
                    if (Math.Abs(sinc) > 0.00001) //sinc(x), x != 0
                        sinc = Math.Sin(sinc) / sinc; //sinc(x) = sin(x)/x
                    else //sinc(0) = 1.0
                        sinc = 1.0;
                    table[x * Synthesizer.SincResolution + y] = (float)(gain * sinc * windowFunction(a, windowSize));
                }
            }
            return table;
        }
    }
}
