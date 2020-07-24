using System;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Bank.Components;
using AudioSynthesis.Bank.Components.Generators;
using AudioSynthesis.Bank.Descriptors;

namespace AudioSynthesis.Bank.Patches
{
    /* FM 2 Operator Patch
     *   M --> C --> OUT
     *    
     *    LFO1              LFO1
     *     |                 |
     *    GEN1 --> ENV1 --> GEN2 --> ENV2 --> OUT 
     *         
     * LFO1 : Usually generates vibrato. Responds to the MOD Controller (MIDI Controlled).
     * GEN1 : A generator with a continuous loop type. The Modulator.
     * ENV1 : An envelope controlling the amplitude of GEN1.
     * GEN2 : A generator with a continuous loop type. The Carrier.
     * ENV2 : An envelope controlling the amplitude of GEN2.
     * 
     * Note: GEN 1 & 2 must also wrap mathmatically on its input like Sin() does
     */
    public class Fm2Patch : Patch
    {
        public enum SyncMode { Soft, Hard };
        private SyncMode sync;
        private double mIndex;
        private double cIndex;
        private double feedBack;

        public SyncMode SynchronizationMethod
        {
            get { return sync; }
        }
        public double ModulationIndex
        {
            get { return mIndex; }
        }
        public double CarrierIndex
        {
            get { return cIndex; }
        }

        public Fm2Patch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            //calculate velocity
            float fVel = voiceparams.velocity / 127f;
            //reset counters
            voiceparams.counters[0] = voiceparams.generators[0].LoopStartPhase;
            voiceparams.counters[1] = voiceparams.generators[1].LoopStartPhase;
            voiceparams.counters[2] = 0.0;
            //reset envelopes
            voiceparams.envelopes[0].QuickSetup(voiceparams.synth.SampleRate, fVel, EnvelopeInfo[0]);
            voiceparams.envelopes[1].QuickSetup(voiceparams.synth.SampleRate, fVel, EnvelopeInfo[1]);
            //reset lfo (vibra)
            voiceparams.lfos[0].QuickSetup(voiceparams.synth.SampleRate, LfoInfo[0]);
            //calc initial volume
            float dBVel = -20.0f * (float)Math.Log10(16129.0 / (voiceparams.velocity * voiceparams.velocity));
            voiceparams.volOffset = (float)SynthHelper.DBtoLinear(dBVel) * voiceparams.synth.MixGain;
            //check if we have finished before we have begun
            return voiceparams.envelopes[0].CurrentState != EnvelopeStateEnum.None;
        }
        public override void Stop(VoiceParameters voiceparams)
        {
            voiceparams.envelopes[0].Release();
            voiceparams.envelopes[1].Release();
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double carrierPitch = SynthHelper.CentsToPitch((voiceparams.note - voiceparams.generators[0].RootKey) * voiceparams.generators[0].KeyTrack + voiceparams.generators[0].Tune + voiceparams.synth.totalPitch[voiceparams.channel])
                * voiceparams.generators[0].Period * voiceparams.generators[0].Frequency * cIndex / voiceparams.synth.SampleRate;
            double modulatorPitch = SynthHelper.CentsToPitch((voiceparams.note - voiceparams.generators[1].RootKey) * voiceparams.generators[1].KeyTrack + voiceparams.generators[1].Tune + voiceparams.synth.totalPitch[voiceparams.channel])
                * voiceparams.generators[1].Period * voiceparams.generators[1].Frequency * mIndex / voiceparams.synth.SampleRate;
            //--Base volume calculation
            float baseVolume = voiceparams.volOffset * voiceparams.synth.totalVolume[voiceparams.channel];
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synth.AudioChannels)
            {
                //--Calculate pitch modifications
                double pitchMod;
                if (voiceparams.synth.modWheel[voiceparams.channel] != 0.0)
                {
                    voiceparams.lfos[0].Increment(Synthesizer.DefaultBlockSize);
                    pitchMod = SynthHelper.CentsToPitch((int)(voiceparams.lfos[0].Value * voiceparams.synth.modWheel[voiceparams.channel]));
                }
                else
                {
                    pitchMod = 1;
                }
                //--Get amplitude values for carrier and modulator
                voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.envelopes[1].Increment(Synthesizer.DefaultBlockSize);
                float c_amp = baseVolume * voiceparams.envelopes[0].Value;
                float m_amp = voiceparams.envelopes[1].Value;
                //--Interpolator for modulator amplitude
                float linear_m_amp = (m_amp - voiceparams.mixing[2]) / Synthesizer.DefaultBlockSize;
                //--Process block
                for (int i = 0; i < voiceparams.blockBuffer.Length; i++)
                {
                    //calculate current modulator amplitude
                    voiceparams.mixing[2] += linear_m_amp;
                    //calculate sample
                    voiceparams.blockBuffer[i] = voiceparams.generators[0].GetValue(voiceparams.counters[0] + voiceparams.mixing[2] * voiceparams.generators[1].GetValue(voiceparams.counters[1] + voiceparams.counters[2] * feedBack));
                    //store sample for feedback calculation
                    voiceparams.counters[2] = voiceparams.blockBuffer[i];
                    //increment phase counters
                    voiceparams.counters[0] += carrierPitch * pitchMod;
                    voiceparams.counters[1] += modulatorPitch * pitchMod;
                }
                voiceparams.mixing[2] = m_amp;
                //--Mix block based on number of channels
                if (voiceparams.synth.AudioChannels == 2)
                    SynthHelper.MixMonoToStereoInterpolation(x,
                        c_amp * voiceparams.synth.panPositions[voiceparams.channel].Left,
                        c_amp * voiceparams.synth.panPositions[voiceparams.channel].Right,
                        voiceparams);
                else
                    SynthHelper.MixMonoToMonoInterpolation(x, c_amp, voiceparams);
                //--Bounds check
                if (sync == SyncMode.Soft)
                {
                    if (voiceparams.counters[0] >= voiceparams.generators[0].LoopEndPhase)
                        voiceparams.counters[0] = voiceparams.generators[0].LoopStartPhase + (voiceparams.counters[0] - voiceparams.generators[0].LoopEndPhase) % (voiceparams.generators[0].LoopEndPhase - voiceparams.generators[0].LoopStartPhase);
                    if (voiceparams.counters[1] >= voiceparams.generators[1].LoopEndPhase)
                        voiceparams.counters[1] = voiceparams.generators[1].LoopStartPhase + (voiceparams.counters[1] - voiceparams.generators[1].LoopEndPhase) % (voiceparams.generators[1].LoopEndPhase - voiceparams.generators[1].LoopStartPhase);
                }
                else
                {
                    if (voiceparams.counters[0] >= voiceparams.generators[0].LoopEndPhase)
                    {
                        voiceparams.counters[0] = voiceparams.generators[0].LoopStartPhase;
                        voiceparams.counters[1] = voiceparams.generators[1].LoopStartPhase;
                    }
                }
                //--Check and end early if necessary
                if (voiceparams.envelopes[0].CurrentState == EnvelopeStateEnum.None)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }
            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            CustomDescriptor fmConfig = description.FindCustomDescriptor("fm2c");
            cIndex = (double)fmConfig.Objects[0];
            mIndex = (double)fmConfig.Objects[1];
            feedBack = (double)fmConfig.Objects[2];
            sync = GetSyncModeFromString((string)fmConfig.Objects[3]);
            if (description.GenDescriptions[0].LoopMethod != LoopModeEnum.Continuous || description.GenDescriptions[1].LoopMethod != LoopModeEnum.Continuous)
                throw new Exception("Fm2 patches must have continuous generators with wrapping bounds.");
            genList[0] = description.GenDescriptions[0].ToGenerator(assets);
            genList[1] = description.GenDescriptions[1].ToGenerator(assets);
            envList[0] = description.EnvelopeDescriptions[0];
            envList[1] = description.EnvelopeDescriptions[1];
            lfoList[0] = description.LfoDescriptions[0];
        }

        public static SyncMode GetSyncModeFromString(string value)
        {
            switch (value)
            {
                case "hard":
                    return SyncMode.Hard;
                case "soft":
                    return SyncMode.Soft;
                default:
                    throw new Exception("Invalid sync mode: " + value + ".");
            }
        }
    }
}
