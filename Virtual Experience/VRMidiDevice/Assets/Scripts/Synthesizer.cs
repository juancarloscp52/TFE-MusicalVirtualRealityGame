using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{
    public Dictionary<byte,Note> notes = new();
    public float gain; // Gain (power) of the oscilator.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Notes playing: {string.Join(",", notes.Keys.Select(x => x.ToString()).ToArray())}");
    }

    // Helped by: https://www.mcvuk.com/development-news/procedural-audio-with-unity/
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (notes.Count == 0)
            return;

        for (int i = 0; i < data.Length; i+= channels) // We increment the number of channels for each iteration, because all channels will output the same data, as we are using mono audio.
        {

            float noteSum = 0;
            foreach (Note note in notes.Values)
            {
                noteSum += note.getSinWave();
            }

            data[i] = gain * noteSum;

            // Copy data to the rest of the channels.
            if(channels == 2)
            {
                data[i+1] = data[i];
            }

        }
    }

}
