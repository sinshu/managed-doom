namespace AudioSynthesis.Midi
{
    using System.Collections.Generic;
    using AudioSynthesis.Midi.Event;

    public class MidiTrack
    {
        private int notesPlayed;
        private int totalTime;
        private byte[] instPrograms;
        private byte[] drumPrograms;
        private byte[] activeChannels;
        private MidiEvent[] midiEvents;

        public int NoteOnCount
        {
            get { return notesPlayed; }
            set { notesPlayed = value; }
        }
        public int EndTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }
        public MidiEvent[] MidiEvents
        {
            get { return midiEvents; }
        }
        public IList<byte> Instruments
        {
            get { return instPrograms; }
        }
        public IList<byte> DrumInstruments
        {
            get { return drumPrograms; }
        }
        public IList<byte> ActiveChannels
        {
            get { return activeChannels; }
        }

        public MidiTrack(byte[] instPrograms, byte[] drumPrograms, byte[] activeChannels, MidiEvent[] midiEvents)
        {
            this.instPrograms = instPrograms;
            this.drumPrograms = drumPrograms;
            this.activeChannels = activeChannels;
            this.midiEvents = midiEvents;
        }
        public override string ToString()
        {
            return "MessageCount: " + midiEvents.Length + ", TotalTime: " + totalTime;
        }
    }
}
