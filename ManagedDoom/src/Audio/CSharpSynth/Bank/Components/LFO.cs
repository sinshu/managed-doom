using AudioSynthesis.Bank.Descriptors;
using AudioSynthesis.Bank.Components.Generators;

namespace AudioSynthesis.Bank.Components
{
    public class Lfo
    {
        private LfoStateEnum lfoState;
        private double phase;
        private double value;
        private double increment;
        private double frequency;
        private double depth;
        private int delayTime;
        private Generator generator;
        
        public double Frequency
        {
            get { return frequency; }
        }
        public LfoStateEnum CurrentState
        {
            get { return lfoState; }
        }
        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public double Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        public void QuickSetup(int sampleRate, LfoDescriptor lfoInfo)
        {
            generator = lfoInfo.Generator;
            delayTime = (int)(sampleRate * lfoInfo.DelayTime);
            frequency = lfoInfo.Frequency;
            increment = generator.Period * frequency / sampleRate;
            depth = lfoInfo.Depth;
            Reset();
        }
        public void Increment(int amount)
        {
            if (lfoState == LfoStateEnum.Delay)
            {
                phase -= amount;
                if (phase <= 0.0)
                {
                    phase = generator.LoopStartPhase + increment * -phase;
                    value = generator.GetValue(phase);
                    lfoState = LfoStateEnum.Sustain;
                }
            }
            else
            {
                phase += increment * amount;
                if (phase >= generator.LoopEndPhase)
                    phase = generator.LoopStartPhase + (phase - generator.LoopEndPhase) % (generator.LoopEndPhase - generator.LoopStartPhase);
                value = generator.GetValue(phase);
            }
        }
        public double GetNext()
        {
            if (lfoState == LfoStateEnum.Delay)
            {
                phase--;
                if (phase <= 0.0)
                {
                    phase = generator.LoopStartPhase;
                    lfoState = LfoStateEnum.Sustain;
                }
                return 0.0;
            }
            else
            {
                phase += increment;
                if (phase >= generator.LoopEndPhase)
                    phase = generator.LoopStartPhase + (phase - generator.LoopEndPhase) % (generator.LoopEndPhase - generator.LoopStartPhase);
                return generator.GetValue(phase);
            }
        }
        public void Reset()
        {
            value = 0;
            if (delayTime > 0)
            {
                phase = delayTime;
                lfoState = LfoStateEnum.Delay;
            }
            else
            {
                phase = 0.0;
                lfoState = LfoStateEnum.Sustain;
            }
        }
        public override string ToString()
        {
            return string.Format("State: {0}, Frequency: {1}Hz, Value: {2:0.00}", lfoState, frequency, value);
        }
    }
}
