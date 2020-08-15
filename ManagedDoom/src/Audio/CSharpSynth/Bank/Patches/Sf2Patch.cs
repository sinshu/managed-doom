using System;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Sf2;
using AudioSynthesis.Bank.Descriptors;
using AudioSynthesis.Bank.Components.Generators;
using AudioSynthesis.Bank.Components;

namespace AudioSynthesis.Bank.Patches
{
    public class Sf2Patch : Patch
    {
        private int iniFilterFc;
        private double filterQ;
        private float sustainVolEnv;
        private float initialAttn;
        private short keyOverride;
        private short velOverride;
        private short keynumToModEnvHold ;
        private short keynumToModEnvDecay;
        private short keynumToVolEnvHold ;
        private short keynumToVolEnvDecay;
        private PanComponent pan;
        private short modLfoToPitch;
        private short vibLfoToPitch;
        private short modEnvToPitch;
        private short modLfoToFilterFc;
        private short modEnvToFilterFc;
        private float modLfoToVolume;

        public Sf2Patch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            int note = keyOverride > -1 ? keyOverride : voiceparams.note;
            int vel = velOverride > -1 ? velOverride : voiceparams.velocity;
            //setup generator
            voiceparams.generatorParams[0].QuickSetup(voiceparams.generators[0]);
            //setup envelopes
            voiceparams.envelopes[0].QuickSetup(voiceparams.synth.SampleRate, note, keynumToModEnvHold, keynumToModEnvDecay, 1, this.EnvelopeInfo[0]);
            float susMod = (float)SynthHelper.DBtoLinear(-sustainVolEnv);
            if (susMod <= 1e-5f)
                susMod = 0;
            voiceparams.envelopes[1].QuickSetup(voiceparams.synth.SampleRate, note, keynumToVolEnvHold, keynumToVolEnvDecay, susMod, this.EnvelopeInfo[1]);
            //setup filter
            //if (filterQ == 1 && modLfoToFilterFc + modEnvToFilterFc + iniFilterFc > 14000)
                voiceparams.filters[0].Disable();
            //else
            //    voiceparams.filters[0].QuickSetup(44100, note, 1f, this.FilterInfo[0]);
            //setup lfos
            voiceparams.lfos[0].QuickSetup(voiceparams.synth.SampleRate, LfoInfo[0]);
            voiceparams.lfos[1].QuickSetup(voiceparams.synth.SampleRate, LfoInfo[1]);
            //calculate base volume and pitch
            voiceparams.pitchOffset = (note - voiceparams.generators[0].RootKey) * voiceparams.generators[0].KeyTrack + voiceparams.generators[0].Tune;
            voiceparams.volOffset = -20.0f * (float)Math.Log10(16129.0 / (vel * vel)) + -initialAttn;
            //check if we have finished before we have begun
            return voiceparams.generatorParams[0].currentState != GeneratorStateEnum.Finished && voiceparams.envelopes[1].CurrentState != EnvelopeStateEnum.None;
        }       
        public override void Stop(VoiceParameters voiceparams)
        {
            voiceparams.generators[0].Release(voiceparams.generatorParams[0]);
            if (voiceparams.generators[0].LoopMode != LoopModeEnum.OneShot)
            {
                voiceparams.envelopes[0].Release();
                voiceparams.envelopes[1].Release();
            }
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double basePitch = SynthHelper.CentsToPitch(voiceparams.pitchOffset + voiceparams.synth.totalPitch[voiceparams.channel])
                * voiceparams.generators[0].Frequency / voiceparams.synth.SampleRate;
            //--Base volume calculation
            //float baseVolume = voiceparams.volOffset * voiceparams.synth.totalVolume[voiceparams.channel];
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synth.AudioChannels)
            {
                voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.envelopes[1].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.lfos[0].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.lfos[1].Increment(Synthesizer.DefaultBlockSize);

                //--Calculate pitch and get next block of samples
                voiceparams.generators[0].GetValues(voiceparams.generatorParams[0], voiceparams.blockBuffer, basePitch *
                    SynthHelper.CentsToPitch((int)(voiceparams.envelopes[0].Value * modEnvToPitch +
                    voiceparams.lfos[0].Value * modLfoToPitch + voiceparams.lfos[1].Value * vibLfoToPitch)));
                //--Filter
                if (voiceparams.filters[0].Enabled)
                {
                    double centsFc = iniFilterFc + voiceparams.lfos[0].Value * modLfoToFilterFc + voiceparams.envelopes[0].Value * modEnvToFilterFc;
                    if (centsFc > 13500)
                        centsFc = 13500;
                    voiceparams.filters[0].UpdateCoefficients(SynthHelper.KeyToFrequency(centsFc / 100.0, 69) / voiceparams.synth.SampleRate, filterQ);
                    voiceparams.filters[0].ApplyFilter(voiceparams.blockBuffer);
                }
                //--Volume calculation
                float volume = (float)SynthHelper.DBtoLinear(voiceparams.volOffset + voiceparams.lfos[0].Value * modLfoToVolume) * voiceparams.envelopes[1].Value * voiceparams.synth.totalVolume[voiceparams.channel] * voiceparams.synth.MixGain;
                //--Mix block based on number of channels
                if (voiceparams.synth.AudioChannels == 2)
                {
                    SynthHelper.MixMonoToStereoInterpolation(x,
                        volume * pan.Left * voiceparams.synth.panPositions[voiceparams.channel].Left,
                        volume * pan.Right * voiceparams.synth.panPositions[voiceparams.channel].Right,
                        voiceparams);
                }
                else
                {
                    SynthHelper.MixMonoToMonoInterpolation(x, volume, voiceparams);
                }
                //--Check and end early if necessary
                if (voiceparams.generatorParams[0].currentState == GeneratorStateEnum.Finished)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }

            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            throw new Exception("Sf2 does not load from patch descriptions.");
        }
        public void Load(Sf2Region region, AssetManager assets)
        {
            this.exGroup = region.Generators[(int)GeneratorEnum.ExclusiveClass];
            this.exTarget = exGroup;
            iniFilterFc = region.Generators[(int)GeneratorEnum.InitialFilterCutoffFrequency];
            filterQ = SynthHelper.DBtoLinear(region.Generators[(int)GeneratorEnum.InitialFilterQ] / 10.0);
            sustainVolEnv = region.Generators[(int)GeneratorEnum.SustainVolumeEnvelope] / 10f;
            initialAttn = region.Generators[(int)GeneratorEnum.InitialAttenuation] / 10f;
            keyOverride = region.Generators[(int)GeneratorEnum.KeyNumber];
            velOverride = region.Generators[(int)GeneratorEnum.Velocity];
            keynumToModEnvHold = region.Generators[(int)GeneratorEnum.KeyNumberToModulationEnvelopeHold];
            keynumToModEnvDecay = region.Generators[(int)GeneratorEnum.KeyNumberToModulationEnvelopeDecay];
            keynumToVolEnvHold = region.Generators[(int)GeneratorEnum.KeyNumberToVolumeEnvelopeHold];
            keynumToVolEnvDecay = region.Generators[(int)GeneratorEnum.KeyNumberToVolumeEnvelopeDecay];
            pan = new PanComponent(region.Generators[(int)GeneratorEnum.Pan] / 500f, PanFormulaEnum.Neg3dBCenter);
            modLfoToPitch = region.Generators[(int)GeneratorEnum.ModulationLFOToPitch];
            vibLfoToPitch = region.Generators[(int)GeneratorEnum.VibratoLFOToPitch];
            modEnvToPitch = region.Generators[(int)GeneratorEnum.ModulationEnvelopeToPitch];
            modLfoToFilterFc = region.Generators[(int)GeneratorEnum.ModulationLFOToFilterCutoffFrequency];
            modEnvToFilterFc = region.Generators[(int)GeneratorEnum.ModulationEnvelopeToFilterCutoffFrequency];
            modLfoToVolume = region.Generators[(int)GeneratorEnum.ModulationLFOToVolume] / 10f;
            LoadGen(region, assets);
            LoadEnvelopes(region);
            LoadLfos(region);
            LoadFilter(region);
        }
        
        private void LoadGen(Sf2Region region, AssetManager assets)
        {
            SampleDataAsset sda = assets.SampleAssetList[region.Generators[(int)GeneratorEnum.SampleID]];
            SampleGenerator gen = new SampleGenerator();
            gen.EndPhase = sda.End + region.Generators[(int)GeneratorEnum.EndAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.EndAddressCoarseOffset];
            gen.Frequency = sda.SampleRate;
            gen.KeyTrack = region.Generators[(int)GeneratorEnum.ScaleTuning];
            gen.LoopEndPhase = sda.LoopEnd + region.Generators[(int)GeneratorEnum.EndLoopAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.EndLoopAddressCoarseOffset];
            switch (region.Generators[(int)GeneratorEnum.SampleModes] & 0x3)
            {
                case 0x0:
                case 0x2:
                    gen.LoopMode = LoopModeEnum.NoLoop;
                    break;
                case 0x1:
                    gen.LoopMode = LoopModeEnum.Continuous;
                    break;
                case 0x3:
                    gen.LoopMode = LoopModeEnum.LoopUntilNoteOff;
                    break;
            }
            gen.LoopStartPhase = sda.LoopStart + region.Generators[(int)GeneratorEnum.StartLoopAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.StartLoopAddressCoarseOffset];
            gen.Offset = 0;
            gen.Period = 1.0;
            if (region.Generators[(int)GeneratorEnum.OverridingRootKey] > -1)
                gen.RootKey = region.Generators[(int)GeneratorEnum.OverridingRootKey];
            else
                gen.RootKey = sda.RootKey;
            gen.StartPhase = sda.Start + region.Generators[(int)GeneratorEnum.StartAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.StartAddressCoarseOffset];
            gen.Tune = (short)(sda.Tune + region.Generators[(int)GeneratorEnum.FineTune] + 100 * region.Generators[(int)GeneratorEnum.CoarseTune]);
            gen.VelocityTrack = 0;
            gen.Samples = sda.SampleData;
            this.genList[0] = gen;
        }
        private void LoadEnvelopes(Sf2Region region)
        {
            //mod env
            this.envList[0] = new EnvelopeDescriptor();
            this.envList[0].AttackTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.AttackModulationEnvelope] / 1200.0);
            this.envList[0].DecayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DecayModulationEnvelope] / 1200.0);
            this.envList[0].DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayModulationEnvelope] / 1200.0);
            this.envList[0].HoldTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.HoldModulationEnvelope] / 1200.0);
            this.envList[0].PeakLevel = 1;
            this.envList[0].ReleaseTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.ReleaseModulationEnvelope] / 1200.0);
            this.envList[0].StartLevel = 0;
            this.envList[0].SustainLevel = 1f - region.Generators[(int)GeneratorEnum.SustainModulationEnvelope] / 1000f;
            //checks
            if (this.envList[0].AttackTime < 0.001f)
                this.envList[0].AttackTime = 0.001f;
            else if (this.envList[0].AttackTime > 100f)
                this.envList[0].AttackTime = 100f;
            if (this.envList[0].DecayTime < 0.001f)
                this.envList[0].DecayTime = 0;
            else if (this.envList[0].DecayTime > 100f)
                this.envList[0].DecayTime = 100f;
            if (this.envList[0].DelayTime < 0.001f)
                this.envList[0].DelayTime = 0;
            else if (this.envList[0].DelayTime > 20f)
                this.envList[0].DelayTime = 20f;
            if (this.envList[0].HoldTime < 0.001f)
                this.envList[0].HoldTime = 0;
            else if (this.envList[0].HoldTime > 20f)
                this.envList[0].HoldTime = 20f;
            if (this.envList[0].ReleaseTime < 0.001f)
                this.envList[0].ReleaseTime = 0.001f;
            else if (this.envList[0].ReleaseTime > 100f)
                this.envList[0].ReleaseTime = 100f;
            //volume env
            this.envList[1] = new EnvelopeDescriptor();
            // It seemed that the following time parameters were too big.
            // So I reduced these values to 1 / 6.
            this.envList[1].AttackTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.AttackVolumeEnvelope] / 1200.0) / 6;
            this.envList[1].DecayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DecayVolumeEnvelope] / 1200.0) / 6;
            this.envList[1].DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayVolumeEnvelope] / 1200.0) / 6;
            this.envList[1].HoldTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.HoldVolumeEnvelope] / 1200.0) / 6;
            this.envList[1].PeakLevel = 1;
            this.envList[1].ReleaseTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.ReleaseVolumeEnvelope] / 1200.0) / 6;
            this.envList[1].StartLevel = 0;
            this.envList[1].SustainLevel = 1;
            //checks
            if (this.envList[1].AttackTime < 0.001f)
                this.envList[1].AttackTime = 0.001f;
            else if (this.envList[1].AttackTime > 100f)
                this.envList[1].AttackTime = 100f;
            if (this.envList[1].DecayTime < 0.001f)
                this.envList[1].DecayTime = 0;
            else if (this.envList[1].DecayTime > 100f)
                this.envList[1].DecayTime = 100f;
            if (this.envList[1].DelayTime < 0.001f)
                this.envList[1].DelayTime = 0;
            else if (this.envList[1].DelayTime > 20f)
                this.envList[1].DelayTime = 20f;
            if (this.envList[1].HoldTime < 0.001f)
                this.envList[1].HoldTime = 0;
            else if (this.envList[1].HoldTime > 20f)
                this.envList[1].HoldTime = 20f;
            if (this.envList[1].ReleaseTime < 0.001f)
                this.envList[1].ReleaseTime = 0.001f;
            else if (this.envList[1].ReleaseTime > 100f)
                this.envList[1].ReleaseTime = 100f;
        }
        private void LoadLfos(Sf2Region region)
        {
            this.lfoList[0] = new LfoDescriptor();
            this.lfoList[0].DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayModulationLFO] / 1200.0);
            this.lfoList[0].Frequency = (float)(Math.Pow(2, region.Generators[(int)GeneratorEnum.FrequencyModulationLFO] / 1200.0) * 8.176);
            this.lfoList[0].Generator = AudioSynthesis.Bank.Components.Generators.Generator.DefaultSine;
            this.lfoList[1] = new LfoDescriptor();
            this.lfoList[1].DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayVibratoLFO] / 1200.0);
            this.lfoList[1].Frequency = (float)(Math.Pow(2, region.Generators[(int)GeneratorEnum.FrequencyVibratoLFO] / 1200.0) * 8.176);
            this.lfoList[1].Generator = AudioSynthesis.Bank.Components.Generators.Generator.DefaultSine;
        }
        private void LoadFilter(Sf2Region region)
        {
            this.fltrList[0] = new FilterDescriptor();
            this.fltrList[0].CutOff = 20000;
            this.fltrList[0].FilterMethod = FilterTypeEnum.BiquadLowpass;
            this.fltrList[0].Resonance = 1;
        }
    }
}
