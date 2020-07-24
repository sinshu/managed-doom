using AudioSynthesis.Synthesis;
using AudioSynthesis.Bank.Components;

namespace AudioSynthesis.Bank.Patches
{
    /* A simple single generator patch
     * 
     *    LFO1
     *     |
     *    GEN1 --> ENV1 --> OUT
     *    
     * LFO1 : Usually generates vibrato. Responds to the MOD Controller (MIDI Controlled).
     * GEN1 : Any generator. No restriction on sampler type.
     * ENV1 : An envelope controlling the amplitude of GEN1.
     */
    public class BasicPatch : Patch
    {
        public BasicPatch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            //calculate velocity
            float fVel = voiceparams.velocity / 127f;
            //reset generator
            voiceparams.generatorParams[0].QuickSetup(voiceparams.generators[0]);
            //reset envelope
            voiceparams.envelopes[0].QuickSetup(voiceparams.synth.SampleRate, fVel, EnvelopeInfo[0]);
            //reset lfo (vibra)
            voiceparams.lfos[0].QuickSetup(voiceparams.synth.SampleRate, LfoInfo[0]);
            //calculate initial pitch
            voiceparams.pitchOffset = (voiceparams.note - voiceparams.generators[0].RootKey) * voiceparams.generators[0].KeyTrack + (int)(fVel * voiceparams.generators[0].VelocityTrack) + voiceparams.generators[0].Tune;
            //calculate initial volume
            voiceparams.volOffset = fVel * voiceparams.synth.MixGain;
            //check if we have finished before we have begun
            return voiceparams.generatorParams[0].currentState != GeneratorStateEnum.Finished && voiceparams.envelopes[2].CurrentState != EnvelopeStateEnum.None;
        }
        public override void Stop(VoiceParameters voiceparams)
        {
            voiceparams.generators[0].Release(voiceparams.generatorParams[0]);
            if (voiceparams.generators[0].LoopMode != LoopModeEnum.OneShot)
                voiceparams.envelopes[0].Release();
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double basePitch = SynthHelper.CentsToPitch(voiceparams.pitchOffset + voiceparams.synth.totalPitch[voiceparams.channel])
                * voiceparams.generators[0].Period * voiceparams.generators[0].Frequency / voiceparams.synth.SampleRate;
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synth.AudioChannels)
            {
                //--Volume Envelope
                voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize);
                //--Lfo pitch modulation
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
                //--Get next block of samples
                voiceparams.generators[0].GetValues(voiceparams.generatorParams[0], voiceparams.blockBuffer, basePitch * pitchMod);
                //--Mix block based on number of channels
                float volume = voiceparams.volOffset * voiceparams.synth.totalVolume[voiceparams.channel] * voiceparams.envelopes[0].Value;
                if (voiceparams.synth.AudioChannels == 2)
                    SynthHelper.MixMonoToStereoInterpolation(x,
                        volume * voiceparams.synth.panPositions[voiceparams.channel].Left,
                        volume * voiceparams.synth.panPositions[voiceparams.channel].Right,
                        voiceparams);
                else
                    SynthHelper.MixMonoToMonoInterpolation(x, volume, voiceparams);
                //--Check and end early if necessary
                if (voiceparams.envelopes[0].CurrentState == EnvelopeStateEnum.None || voiceparams.generatorParams[0].currentState == GeneratorStateEnum.Finished)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }
            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            genList[0] = description.GenDescriptions[0].ToGenerator(assets);
            envList[0] = description.EnvelopeDescriptions[0];
            lfoList[0] = description.LfoDescriptions[0];
        }
    }
}
