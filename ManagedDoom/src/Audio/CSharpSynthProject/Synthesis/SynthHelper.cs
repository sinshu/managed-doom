using System;
using AudioSynthesis.Util;
using AudioSynthesis.Midi;

namespace AudioSynthesis.Synthesis
{
    //structs and enum
    public enum PanFormulaEnum { Neg3dBCenter, Neg6dBCenter, ZeroCenter }
    public enum VoiceStateEnum { Stopped, Stopping, Playing }
    public struct MidiMessage
    {
        public int delta;
        public byte channel;
        public byte command;
        public byte data1;
        public byte data2;

        public MidiMessage(byte channel, byte command, byte data1, byte data2)
            : this(0, channel, command, data1, data2) { }
        public MidiMessage(int delta, byte channel, byte command, byte data1, byte data2)
        {
            this.delta = delta;
            this.channel = channel;
            this.command = command;
            this.data1 = data1;
            this.data2 = data2;
        }
        public override string ToString()
        {
            if (command >= 0x80 && command <= 0xEF)
                return string.Format("Type: {0}, Channel: {1}, P1: {2}, P2: {3}", (MidiEventTypeEnum)(command & 0xF0), channel, data1, data2);
            else if (command >= 0xF0 && command <= 0xF7)
                return "System Common message";
            else if (command >= 0xF8 && command <= 0xFF)
                return "Realtime message";
            else
                return "Unknown midi message";
        }
    }
    public struct PanComponent
    {
        public float Left;
        public float Right;
        public PanComponent(float value, PanFormulaEnum formula)
        {
            value = SynthHelper.Clamp(value, -1f, 1f);
            switch (formula)
            {
                case PanFormulaEnum.Neg3dBCenter:
                {
                    double dvalue = Synthesizer.HalfPi * (value + 1f) / 2f;
                    Left = (float)Math.Cos(dvalue);
                    Right = (float)Math.Sin(dvalue);
                }
                    break;
                case PanFormulaEnum.Neg6dBCenter:
                {
                    Left = .5f + value * -.5f;
                    Right = .5f + value * .5f;
                }
                    break;
                case PanFormulaEnum.ZeroCenter:
                {
                    double dvalue = Synthesizer.HalfPi * (value + 1.0) / 2.0;
                    Left = (float)(Math.Cos(dvalue) / Synthesizer.InverseSqrtOfTwo);
                    Right = (float)(Math.Sin(dvalue) / Synthesizer.InverseSqrtOfTwo);
                }
                    break;
                default:
                    throw new Exception("Invalid pan law selected.");
            }
        }
        public PanComponent(float right, float left)
        {
            this.Right = right;
            this.Left = left;
        }
        public override string ToString()
        {
            return string.Format("Left: {0:0.0}, Right: {1:0.0}", Left, Right);
        }
    }

    //static helper methods
    public static class SynthHelper
    {
        //Math related calculations
        public static double Clamp(double value, double min, double max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }
        public static short Clamp(short value, short min, short max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }

        public static double NearestPowerOfTwo(double value)
        {
            return Math.Pow(2, Math.Round(Math.Log(value, 2)));
        }
        public static double SamplesFromTime(int sampleRate, double seconds)
        {
            return sampleRate * seconds;
        }
        public static double TimeFromSamples(int sampleRate, int samples)
        {
            return samples / (double)sampleRate;
        }
        
        public static double DBtoLinear(double dBvalue)
        {
            return Math.Pow(10.0, (dBvalue / 20.0));
        }
        public static double LineartoDB(double linearvalue)
        {
            return 20.0 * Math.Log10(linearvalue);
        }

        //Midi Note and Frequency Conversions
        public static double FrequencyToKey(double frequency, int rootkey)
        {
            return 12.0 * Math.Log(frequency / 440.0, 2.0) + rootkey;
        }
        public static double KeyToFrequency(double key, int rootkey)
        {
            return Math.Pow(2.0, (key - rootkey) / 12.0) * 440.0;
        }

        public static double SemitoneToPitch(int key)
        {//does not return a frequency, only the 2^(1/12) value.
            if (key < -127)
                key = -127;
            else if (key > 127)
                key = 127;
            return Tables.SemitoneTable[127 + key];
        }
        public static double CentsToPitch(int cents)
        {//does not return a frequency, only the 2^(1/12) value.
            int key = cents / 100;
            cents -= key * 100;
            if (key < -127)
                key = -127;
            else if (key > 127)
                key = 127;
            return Tables.SemitoneTable[127 + key] * Tables.CentTable[100 + cents];
        }

        //Mixing
        public static void MixStereoToStereoInterpolation(int startIndex, float leftVol, float rightVol, VoiceParameters voiceParams)
        {
            float inc_l = (leftVol - voiceParams.mixing[0]) / Synthesizer.DefaultBlockSize;
            float inc_r = (rightVol - voiceParams.mixing[1]) / Synthesizer.DefaultBlockSize;
            for (int i = 0; i < voiceParams.blockBuffer.Length; i++)
            {
                voiceParams.mixing[0] += inc_l;
                voiceParams.mixing[1] += inc_r;
                voiceParams.synth.sampleBuffer[startIndex + i] += voiceParams.blockBuffer[i++] * voiceParams.mixing[0];
                voiceParams.synth.sampleBuffer[startIndex + i] += voiceParams.blockBuffer[i] * voiceParams.mixing[1];
            }
            voiceParams.mixing[0] = leftVol;
            voiceParams.mixing[1] = rightVol;
        }      
        public static void MixMonoToStereoInterpolation(int startIndex, float leftVol, float rightVol, VoiceParameters voiceParams)
        {
            float inc_l = (leftVol - voiceParams.mixing[0]) / Synthesizer.DefaultBlockSize;
            float inc_r = (rightVol - voiceParams.mixing[1]) / Synthesizer.DefaultBlockSize;
            for (int i = 0; i < voiceParams.blockBuffer.Length; i++)
            {
                voiceParams.mixing[0] += inc_l;
                voiceParams.mixing[1] += inc_r;
                voiceParams.synth.sampleBuffer[startIndex] += voiceParams.blockBuffer[i] * voiceParams.mixing[0];
                voiceParams.synth.sampleBuffer[startIndex + 1] += voiceParams.blockBuffer[i] * voiceParams.mixing[1];
                startIndex += 2;
            }
            voiceParams.mixing[0] = leftVol;
            voiceParams.mixing[1] = rightVol;
        }
        public static void MixMonoToMonoInterpolation(int startIndex, float volume, VoiceParameters voiceParams)
        {
            float inc = (volume - voiceParams.mixing[0]) / Synthesizer.DefaultBlockSize;
            for (int i = 0; i < voiceParams.blockBuffer.Length; i++)
            {
                voiceParams.mixing[0] += inc;
                voiceParams.synth.sampleBuffer[startIndex + i] += voiceParams.blockBuffer[i] * voiceParams.mixing[0];
            }
            voiceParams.mixing[0] = volume;
        }
    }
}
