using SmfLite;
using System.Collections.Generic;
using UnityEngine;

public class MidiFileManager : MonoBehaviour
{

    public TextAsset source;
    public float bpm;
    public float bpmMultiplier = 1;

    MidiFileContainer song;

    List<MidiTrackSequencer> sequencers = new();
    public GameObject audioSource;
    public ButtonBoard board;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Start song if the trigger is pressed.
        if (Input.GetAxis("XRI_Left_Trigger")>=0.75)
        {
            StartSong();
            return;
        }

        // Load next song sequence and dispatch note events.
        foreach (var seq in sequencers)
        {
            if (seq != null && seq.Playing)
            {
                EventDispatcher(seq.Advance(Time.deltaTime));

            }
        }

    }

    private void EventDispatcher(List<MidiEvent> eventList)
    {

        if(eventList != null && eventList.Count > 0) 
        {
            foreach(MidiEvent midiEvent in eventList) 
            {

                // If MIDI message is NoteON for any note play hint animation.
                if (midiEvent.status >= 144 && midiEvent.status<160)
                {
                    board.PlayAnim(midiEvent.data1 % 16);
                }
                else if (midiEvent.status >= 128 && midiEvent.status <144)
                {
                    // Note off, do nothing.
                }

            }
        }
    }

    private void StartSong()
    {
        // Load song from file and add track content to sequencer list.
        song = MidiFileLoader.Load(source.bytes);
        sequencers = new();
        for (int i = 0; i < song.tracks.Count; i++)
        {
            sequencers.Add(new(song.tracks[i], song.division, bpm * bpmMultiplier));
            EventDispatcher(sequencers[i].Start(0));
        }

        Debug.Log($"MIDI Sequencer: Started with bpm: {bpm*bpmMultiplier} ({bpmMultiplier})");

    }

}
