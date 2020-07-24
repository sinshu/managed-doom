using System;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Bank.Descriptors;
using AudioSynthesis.Bank.Components;

namespace AudioSynthesis.Bank.Patches
{
    /* A single generator patch with sfz parameters 
     * 
     * (Pitch)  (Cutoff)  (Volume)
     *    |        |        |  
     *   ENV0     ENV1     ENV2
     *    |        |        |
     *   LFO0     LFO1     LFO2
     *    |        |        |
     *   GEN0 --> FLT0 --> MIX --> OUTPUT
     *   
     * ENV0 : An envelope that effects pitch
     * ENV1 : An envelope that effects the filter's cutoff
     * ENV2 : An envelope that effects volume
     * LFO0 : LFO used for pitch modulation
     * LFO1 : LFO used to alter the filter's cutoff
     * LFO2 : LFO for tremulo effect on volume
     * GEN0 : A sample generator
     * FLT0 : A filter
     * MIX  : Handles volume mixing (interp and panning)
     */
    public class SfzPatch : Patch
    {
        private float sfzVolume;
        private float ampKeyTrack;
        private float ampVelTrack;
        private PanComponent sfzPan;
        private short ampRootKey;

        public SfzPatch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            //calculate velocity
            float fVel = voiceparams.velocity / 127f;
            //setup generator
            voiceparams.generatorParams[0].QuickSetup(voiceparams.generators[0]);
            //setup envelopes
            voiceparams.envelopes[0].QuickSetup(voiceparams.synth.SampleRate, fVel, this.EnvelopeInfo[0]);
            voiceparams.envelopes[1].QuickSetup(voiceparams.synth.SampleRate, fVel, this.EnvelopeInfo[1]);
            voiceparams.envelopes[2].QuickSetup(voiceparams.synth.SampleRate, fVel, this.EnvelopeInfo[2]);
            //setup lfos
            voiceparams.lfos[0].QuickSetup(voiceparams.synth.SampleRate, LfoInfo[0]);
            voiceparams.lfos[1].QuickSetup(voiceparams.synth.SampleRate, LfoInfo[1]);
            voiceparams.lfos[2].QuickSetup(voiceparams.synth.SampleRate, LfoInfo[2]);
            //setup filter
            voiceparams.filters[0].QuickSetup(voiceparams.synth.SampleRate, voiceparams.note, fVel, this.FilterInfo[0]);
            if (!voiceparams.filters[0].Enabled)
            {//disable filter components if necessary
                voiceparams.envelopes[1].Depth = 0f;
                voiceparams.lfos[1].Depth = 0f;
            }
            //setup sfz params        
            voiceparams.pitchOffset = (voiceparams.note - voiceparams.generators[0].RootKey) * voiceparams.generators[0].KeyTrack + (int)(fVel * voiceparams.generators[0].VelocityTrack) + voiceparams.generators[0].Tune;
            float dBVel = -20.0f * (float)Math.Log10(16129.0 / (voiceparams.velocity * voiceparams.velocity));
            voiceparams.volOffset = (float)SynthHelper.DBtoLinear((voiceparams.note - ampRootKey) * ampKeyTrack + dBVel * ampVelTrack + sfzVolume) * voiceparams.synth.MixGain;
            //check if we have finished before we have begun
            return voiceparams.generatorParams[0].currentState != GeneratorStateEnum.Finished && voiceparams.envelopes[2].CurrentState != EnvelopeStateEnum.None;
        }
        public override void Stop(VoiceParameters voiceparams)
        {
            voiceparams.generators[0].Release(voiceparams.generatorParams[0]);
            if (voiceparams.generators[0].LoopMode != LoopModeEnum.OneShot)
            {
                voiceparams.envelopes[0].Release();
                voiceparams.envelopes[1].Release();
                voiceparams.envelopes[2].Release();
            }
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double basePitch = SynthHelper.CentsToPitch(voiceparams.pitchOffset + voiceparams.synth.totalPitch[voiceparams.channel])
                * voiceparams.generators[0].Frequency / voiceparams.synth.SampleRate;
            //--Base volume calculation
            float baseVolume = voiceparams.volOffset * voiceparams.synth.totalVolume[voiceparams.channel];
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synth.AudioChannels)
            {
                //--Envelope Calculations
                if (voiceparams.envelopes[0].Depth != 0)
                    voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize); //pitch envelope
                if (voiceparams.envelopes[1].Depth != 0)
                    voiceparams.envelopes[1].Increment(Synthesizer.DefaultBlockSize); //filter envelope
                voiceparams.envelopes[2].Increment(Synthesizer.DefaultBlockSize); //amp envelope (do not skip)
                //--LFO Calculations
                if (voiceparams.lfos[0].Depth + voiceparams.synth.modWheel[voiceparams.channel] != 0)
                    voiceparams.lfos[0].Increment(Synthesizer.DefaultBlockSize); //pitch lfo
                if (voiceparams.lfos[1].Depth != 0)
                    voiceparams.lfos[1].Increment(Synthesizer.DefaultBlockSize); //filter lfo
                if (voiceparams.lfos[2].Depth != 1.0)//linear scale 1.0 = 0dB
                    voiceparams.lfos[2].Increment(Synthesizer.DefaultBlockSize); //amp lfo
                //--Calculate pitch and get next block of samples
                voiceparams.generators[0].GetValues(voiceparams.generatorParams[0], voiceparams.blockBuffer, basePitch *
                    SynthHelper.CentsToPitch((int)(voiceparams.envelopes[0].Value * voiceparams.envelopes[0].Depth +
                    voiceparams.lfos[0].Value * (voiceparams.lfos[0].Depth + voiceparams.synth.modWheel[voiceparams.channel]))));
                //--Filter if enabled
                //...
                //--Volume calculation
                float volume = baseVolume * voiceparams.envelopes[2].Value * (float)(Math.Pow(voiceparams.lfos[2].Depth, voiceparams.lfos[2].Value));
                //--Mix block based on number of channels
                if (voiceparams.synth.AudioChannels == 2)
                {
                    SynthHelper.MixMonoToStereoInterpolation(x ,
                        volume * sfzPan.Left  * voiceparams.synth.panPositions[voiceparams.channel].Left ,
                        volume * sfzPan.Right * voiceparams.synth.panPositions[voiceparams.channel].Right ,
                        voiceparams);
                }
                else
                {
                    SynthHelper.MixMonoToMonoInterpolation(x, volume, voiceparams);
                }
                //--Check and end early if necessary
                if (voiceparams.envelopes[2].CurrentState == EnvelopeStateEnum.None || voiceparams.generatorParams[0].currentState == GeneratorStateEnum.Finished)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }
            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            //read in sfz params
            CustomDescriptor sfzConfig = description.FindCustomDescriptor("sfzi");
            exTarget = (int)sfzConfig.Objects[0];
            exGroup = (int)sfzConfig.Objects[1];
            sfzVolume = (float)sfzConfig.Objects[2];
            sfzPan = new PanComponent((float)sfzConfig.Objects[3], PanFormulaEnum.Neg3dBCenter);
            ampKeyTrack = (float)sfzConfig.Objects[4];
            ampRootKey = (byte)sfzConfig.Objects[5];
            ampVelTrack = (float)sfzConfig.Objects[6];
            //read in the generator info
            GeneratorDescriptor gdes = description.GenDescriptions[0];
            if (gdes.SamplerType != WaveformEnum.SampleData)
                throw new Exception("Sfz can only support sample data generators.");
            this.genList[0] = gdes.ToGenerator(assets);
            //read in the envelope info
            this.envList[0] = description.EnvelopeDescriptions[0];
            this.envList[1] = description.EnvelopeDescriptions[1];
            this.envList[2] = description.EnvelopeDescriptions[2];
            //read in the lfo info
            this.lfoList[0] = description.LfoDescriptions[0];
            this.lfoList[1] = description.LfoDescriptions[1];
            this.lfoList[2] = description.LfoDescriptions[2];
            //read in the filter info
            this.fltrList[0] = description.FilterDescriptions[0];
        }
    }
}