using System;
using AudioSynthesis.Bank.Descriptors;
using AudioSynthesis.Synthesis;

namespace AudioSynthesis.Bank.Components
{
    public class Filter
    {
        private FilterTypeEnum filterType;
        private float b1, b2, a1, a2;
        private float m1, m2, m3;
        private double cutOff;
        private double resonance;
        private double lastFc;

        public FilterTypeEnum FilterMethod
        {
            get { return filterType; }
        }
        public double Cutoff
        {
            get { return cutOff; }
        }
        public double Resonance
        {
            get { return resonance; }
        }
        public bool Enabled
        {
            get { return filterType != FilterTypeEnum.None; }
        }

        public void Disable()
        {
            filterType = FilterTypeEnum.None;
        }
        public void QuickSetup(int sampleRate, int note, float velocity, FilterDescriptor filterInfo)
        {
            cutOff = filterInfo.CutOff;
            resonance = filterInfo.Resonance;
            filterType = filterInfo.FilterMethod;
            lastFc = -1000;
            m1 = 0f;
            m2 = 0f;
            m3 = 0f;
            if (filterType == FilterTypeEnum.None || cutOff <= 0.0 || resonance <= 0.0)
            {
                filterType = FilterTypeEnum.None;
            }
            else
            {
                double fc = cutOff * SynthHelper.CentsToPitch((note - filterInfo.RootKey) * filterInfo.KeyTrack + (int)(velocity * filterInfo.VelTrack));
                UpdateCoefficients(SynthHelper.Clamp(fc / sampleRate, 0, .5), resonance);
            }
        }
        //call sparingly inside process to actively update the cutoff and q
        public void UpdateCoefficients(double fc, double q)
        {
            fc = SynthHelper.Clamp(fc, 0, .49);
            if (Math.Abs(lastFc - fc) > .001)
            {
                switch (filterType)
                {
                    case FilterTypeEnum.BiquadLowpass:
                        ConfigBiquadLowpass(fc, q);
                        break;
                    case FilterTypeEnum.BiquadHighpass:
                        ConfigBiquadHighpass(fc, q);
                        break;
                    case FilterTypeEnum.OnePoleLowpass:
                        ConfigOnePoleLowpass(fc);
                        break;
                }
                lastFc = fc;
            }
        }
        public float ApplyFilter(float sample)
        {
            switch(filterType)
            {
                case FilterTypeEnum.BiquadHighpass:
                case FilterTypeEnum.BiquadLowpass:
                    m3 = sample - a1 * m1 - a2 * m2;
                    sample = b2 * (m3 + m2) + b1 * m1;
                    m2 = m1;
                    m1 = m3;
                    return sample;
                case FilterTypeEnum.OnePoleLowpass:
                    m1 += a1 * (sample - m1);
                    return m1;
                default:
                    return 0f;
            }
        }
        public void ApplyFilter(float[] data)
        {
            switch (filterType)
            {
                case FilterTypeEnum.BiquadHighpass:
                case FilterTypeEnum.BiquadLowpass:
                    for (int x = 0; x < data.Length; x++)
                    {
                        m3 = data[x] - a1 * m1 - a2 * m2;
                        data[x] = b2 * (m3 + m2) + b1 * m1;
                        m2 = m1;
                        m1 = m3;
                    }
                    break;
                case FilterTypeEnum.OnePoleLowpass:
                    for (int x = 0; x < data.Length; x++)
                    {
                        m1 += a1 * (data[x] - m1);
                        data[x] = m1;
                    }
                    break;
                default:
                    break;
            }
        }
        //helper methods for coeff update
        private void ConfigBiquadLowpass(double fc, double q)
        {
            double w0 = Synthesizer.TwoPi * fc;
            double cosw0 = Math.Cos(w0);
            double alpha = Math.Sin(w0) / (2.0 * q);
            double a0inv = 1.0 / (1.0 + alpha);
            a1 = (float)(-2.0 * cosw0 * a0inv);
            a2 = (float)((1.0 - alpha) * a0inv);
            b1 = (float)((1.0 - cosw0) * a0inv * (1.0 / Math.Sqrt(q)));
            b2 = b1 * 0.5f;
        }
        private void ConfigBiquadHighpass(double fc, double q)
        {
            double w0 = Synthesizer.TwoPi * fc;
            double cosw0 = Math.Cos(w0);
            double alpha = Math.Sin(w0) / (2.0 * q);
            double a0inv = 1.0 / (1.0 + alpha);
            double qinv = 1.0 / Math.Sqrt(q);
            a1 = (float)(-2.0 * cosw0 * a0inv);
            a2 = (float)((1.0 - alpha) * a0inv);
            b1 = (float)((-1.0 - cosw0) * a0inv * qinv);
            b2 = (float)((1.0 + cosw0) * a0inv * qinv * 0.5);
        }
        private void ConfigOnePoleLowpass(double fc)
        {
            a1 = 1.0f - (float)Math.Exp(-2.0 * Math.PI * fc);
        }
    }
}
