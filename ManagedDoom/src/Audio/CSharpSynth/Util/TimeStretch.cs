/* Copyright (c) Olli Parviainen 2006 <oparviai@iki.fi> originial source from : http://www.surina.net/article/time-and-pitch-scaling.html
 ************************************************************************************************************************
 * Time stretching in the time domain using SOLA algorithm. This implementation needs optimization, 
 * but works well for small scale changes like x0.5 OR x2.0. Timescale changes of more than double or
 * less than half may result in distortions. 
 */
using System;
using AudioSynthesis.Synthesis;

namespace AudioSynthesis.Util
{
    public class TimeStretch
    {
        private double time_scale = 1.0; // Time scaling factor: values > 1.0 increase tempo, values < 1.0 decrease tempo
        private int sequence_size; // Processing sequence size
        private int overlap_size;  // Overlapping size
        private int seek_window;   // Best overlap offset seeking window
        private int flat_duration; // Processing sequence flat mid-section duration
        private int sequence_skip; // Theoretical interval between the processing seqeuences

        public TimeStretch(double timeScale, int sampleRate)
        {
            //uses default calculations: (scale, 100ms, 20ms, 15ms, srate)
            Configure(time_scale, .1, .02, .015, sampleRate);
        }

        /*change time stretch parameters*/
        public void Configure(double timeScale, double sequenceSec, double overlapSec, double windowSec, int sampleRate)
        {
            time_scale = SynthHelper.Clamp(timeScale, 0.0, 2.0);
            sequence_size = (int)SynthHelper.SamplesFromTime(sampleRate, sequenceSec);
            overlap_size = (int)SynthHelper.SamplesFromTime(sampleRate, overlapSec);
            seek_window = (int)SynthHelper.SamplesFromTime(sampleRate, windowSec);
            flat_duration = sequence_size - 2 * overlap_size;
            flat_duration = sequence_size - 2 * overlap_size;
            sequence_skip = (int)((sequence_size - overlap_size) * time_scale);
        }

        /* SOLA algorithm. Performs time scaling for sample data given in 'input', 
           write result to 'output'. Return number of output samples. */
        public float[] ApplyTimeStretching(float[] input)
        {
            float[] output = new float[GetOutputBufferSize(input.Length)];
            ApplyTimeStretching(input, output);
            return output;
        }
        public void ApplyTimeStretching(float[] input, float[] output)
        {
            int num_in_samples = input.Length;

            int seq_offset_pointer = 0;
            int prev_offset_pointer = 0;

            int input_pointer = 0;
            int output_pointer = 0;

            while (num_in_samples > sequence_skip + seek_window)
            {
                // copy flat mid-sequence from current processing sequence to output
                //memcpy(output, seq_offset, FLAT_DURATION * sizeof(SAMPLE));
                Array.Copy(input, seq_offset_pointer, output, output_pointer, flat_duration);
                // calculate a pointer to overlap at end of the processing sequence
                prev_offset_pointer = seq_offset_pointer + flat_duration;

                // update input pointer to theoretical next processing sequence begin
                input_pointer += sequence_skip - overlap_size;
                // seek actual best matching offset using cross-correlation
                seq_offset_pointer = input_pointer + Seek_Best_Overlap(prev_offset_pointer, input_pointer, input);

                // do overlapping between previous & new sequence, copy result to output
                Overlap(output_pointer + flat_duration, prev_offset_pointer, seq_offset_pointer, output, input);

                // Update input & sequence pointers by overlapping amount
                seq_offset_pointer += overlap_size;
                input_pointer += overlap_size;

                // Update output pointer & sample counters
                output_pointer += sequence_size - overlap_size;
                num_in_samples -= sequence_skip;
            }
        }

        /*calculates the new buffer size after time-streching given the input buffer's length*/
        public int GetOutputBufferSize(int inputLength)
        {
            int len = inputLength / sequence_skip;
            if (inputLength - (len * sequence_skip) < seek_window)
                len--;
            return len * (sequence_size - overlap_size);
        }

        /* Use cross-correlation function to find best overlapping offset 
           where input_prev and input_new match best with each other */
        private int Seek_Best_Overlap(int input_prev, int input_new, float[] input)
        {
           int i;
           int bestoffset = 0;
           float bestcorr = -1e30f;
           float[] temp = new float[overlap_size];

           // Precalculate overlapping slopes with input_prev
           for (i = 0; i < temp.Length; i++)
           {
               temp[i] = input[input_prev + i] * i * (overlap_size - i);
           }

           // Find best overlap offset within [0..SEEK_WINDOW]
           for (i = 0; i < seek_window; i++)
           {
              float crosscorr = 0;

              for (int j = 0; j < temp.Length; j++)
              {
                 crosscorr += input[input_new + i + j] * temp[j];
              }
              if (crosscorr > bestcorr)
              {
                 // found new best offset candidate
                 bestcorr = crosscorr;
                 bestoffset = i;
              }
           }
           return bestoffset;
        }

        /* Overlap 'input_prev' with 'input_new' by sliding the amplitudes during 
           OVERLAP samples. Store result to 'output'. */
        private void Overlap(int output_offset, int input_prev, int input_new, float[] output, float[] input)
        {
           for (int i = 0; i < overlap_size; i++)
           {
               output[output_offset + i] = (input[input_prev + i] * (overlap_size - i) + input[input_new + i] * i) / overlap_size;
           }
        }
    }
}
