using UnityEngine;

namespace Assets.Scripts
{
    public class Note
    {
        private static readonly double Sampling_Frequency = 48000.0;
        private readonly byte note;
        private readonly double increment; // Ammount of distance the wave travels each frame, based on frequency.
        private double phase; // location on the wave.

        public Note(byte note) {
            this.note = note;
            increment = getNoteFreq() * 2.0 * Mathf.PI / Sampling_Frequency;
        }

        //https://www.inspiredacoustics.com/en/MIDI_note_numbers_and_center_frequencies
        private float getNoteFreq()
        {
            if (note < 0 || note > 127)
                return 0;
            return 440.0f * Mathf.Pow(2, (note - 69.0f) / 12.0f);
        }

        public float getSinWave()
        {
            phase += increment;
            float sin = Mathf.Sin((float)phase);
            if (phase > (Mathf.PI * 2))
            {
                phase %= (Mathf.PI * 2);
            }
            return sin;
        }
    }
}