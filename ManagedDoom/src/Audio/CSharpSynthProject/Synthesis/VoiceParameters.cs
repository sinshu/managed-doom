using AudioSynthesis.Bank.Components;
using AudioSynthesis.Bank.Components.Generators;

namespace AudioSynthesis.Synthesis
{
    public class VoiceParameters
    {
        public int channel;
        public int note;
        public int velocity;
        public bool noteOffPending;
        public VoiceStateEnum state;
        public Synthesizer synth;
        public int pitchOffset;
        public float volOffset;
        public float[] blockBuffer;
        public float[] mixing;       //variables used during mixing (volume levels, filter coeffs, etc)
        public double[] counters;    //misc counters used by patches
        public GeneratorParameters[] generatorParams;
        public Generator[] generators;  //set directly
        public Envelope[] envelopes;    //set by parameters (quicksetup)
        public Filter[] filters;        //set by parameters (quicksetup)
        public Lfo[] lfos;              //set by parameters (quicksetup)
        public VoiceParameters(Synthesizer synth)
        {
            this.synth = synth;
            blockBuffer = new float[Synthesizer.DefaultBlockSize];
            //create default number of each component
            mixing = new float[Synthesizer.MaxVoiceComponents];
            counters = new double[Synthesizer.MaxVoiceComponents];
            generatorParams = new GeneratorParameters[Synthesizer.MaxVoiceComponents];
            generators = null; //since this is set directly there is no need to initialize
            envelopes = new Envelope[Synthesizer.MaxVoiceComponents];
            filters = new Filter[Synthesizer.MaxVoiceComponents];
            lfos = new Lfo[Synthesizer.MaxVoiceComponents];
            //initialize each component
            for (int x = 0; x < Synthesizer.MaxVoiceComponents; x++)
            {
                generatorParams[x] = new GeneratorParameters();
                envelopes[x] = new Envelope();
                filters[x] = new Filter();
                lfos[x] = new Lfo();
            }
        }
        public override string ToString()
        {
            return string.Format("Channel: {0}, Key: {1}, Velocity: {2}, State: {3}", channel, note, velocity, state);
        }
    }
}
