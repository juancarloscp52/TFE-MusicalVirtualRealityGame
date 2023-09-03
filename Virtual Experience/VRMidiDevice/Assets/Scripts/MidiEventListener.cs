using System;
using UnityEngine;

public class MidiEventListener : MonoBehaviour
{

    public GameObject audioSource;
    public ButtonBoard board;
    public MidiFileManager smf;
    // Start is called before the first frame update
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {

    }


    // serialized data contains deviceID + the serialized data.
    // We only need the serialized data without the id.
    // AA:AA:AA:AA:AA:AA,SERIALIZED_DATA -> serialized data is separated by ','
    private int[] DeserializeData(string serializedData)
    {
        string[] data = serializedData.Split(",");
        //Debug.Log("DATA " + data.ToString());
        int[] intData = new int[data.Length-1];
        // We ignore the deviceID as it is not useful for us.
        for (int i = 1; i<data.Length;i++)
        {
            intData[i-1] = int.Parse(data[i]);
        }
        return intData;
    }

    /**
     * SysEx
     *
     * @param deviceId the device sent this message
     * @param systemExclusive received message
     */
    public void OnMidiSystemExclusive(String serializedData) {

        Debug.Log("MidiSystemExclusive: " + serializedData);

    }

    /**
	 * Note-off
	 *
     * @param deviceId the device sent this message
     * @param cable 0-15
	 * @param channel 0-15
	 * @param note 0-127
	 * @param velocity 0-127
	 */
    public void OnMidiNoteOff(String serializedData) {

        Debug.Log("Note:off " + DeserializeData(serializedData));
        byte note = (byte)DeserializeData(serializedData)[2];
        audioSource.GetComponent<Synthesizer>().notes.Remove(note);
        board.state[note % 16] = false;

    }

    /**
	 * Note-on
	 *
     * @param deviceId the device sent this message
     * @param cable 0-15
	 * @param channel 0-15
	 * @param note 0-127
	 * @param velocity 0-127
	 */
    public void OnMidiNoteOn(String serializedData) {

        Debug.Log("Note:on " + DeserializeData(serializedData));
        byte note = (byte)DeserializeData(serializedData)[2];
        audioSource.GetComponent<Synthesizer>().notes.Add(note, new Assets.Scripts.Note(note));
        board.state[note % 16] = true;

    }

    /**
	 * Poly-KeyPress
	 *
     * @param deviceId the device sent this message
	 * @param channel 0-15
	 * @param note 0-127
	 * @param pressure 0-127
	 */
    public void OnMidiPolyphonicAftertouch(String serializedData) {
        Debug.Log("PolyphonicAftertouch: " + DeserializeData(serializedData));
    }

    /**
	 * Control Change
	 *
     * @param deviceId the device sent this message
	 * @param channel 0-15
	 * @param function 0-127
	 * @param value 0-127
	 */
    public void OnMidiControlChange(String serializedData) {
        var data = DeserializeData(serializedData);

        switch (data[2])
        {
            case 0:  // note interval.
                board.setIntervalPot(data[3]);
                break;
            case 1:  // volume
                audioSource.GetComponent<AudioSource>().volume = data[3]/127f; // flaot range from 0 to 1.
                board.setVolumePot(audioSource.GetComponent<AudioSource>().volume);
                Debug.Log($"Volume: ({data[3]}) " + audioSource.GetComponent<AudioSource>().volume);
                break;
            case 2:  // bpm.
                smf.bpmMultiplier = data[3] / (127f / 2f); // float range from 0 to 2.
                board.setBPMPot(smf.bpmMultiplier);
                Debug.Log($"BPM:  ({data[3]})" + smf.bpmMultiplier);
                break;
        }
    }

    /**
	 * Program Change
	 *
     * @param deviceId the device sent this message
	 * @param channel 0-15
	 * @param program 0-127
	 */
    public void OnMidiProgramChange(String serializedData) {
        Debug.Log("MidiProgramChange: " + DeserializeData(serializedData));
    }

    /**
	 * Channel Pressure
	 *
     * @param deviceId the device sent this message
	 * @param channel 0-15
	 * @param pressure 0-127
	 */
    public void OnMidiChannelAftertouch(String serializedData) {
        Debug.Log("MidiChannelAftertouch: " + DeserializeData(serializedData));

    }

    /**
	 * PitchBend Change
	 *
     * @param deviceId the device sent this message
	 * @param channel 0-15
	 * @param amount 0(low)-8192(center)-16383(high)
	 */
    public void OnMidiPitchWheel(String serializedData) {
        var data = DeserializeData(serializedData);
        audioSource.GetComponent<AudioSource>().pitch = data[2] / (16384f / 3f); // flaot range from 0 to 1.
        board.setPitchPot(audioSource.GetComponent<AudioSource>().pitch);
        Debug.Log($"pitch: ({data[2]}) " + audioSource.GetComponent<AudioSource>().pitch);
    }

    /**
     * MIDI Time Code(MTC) Quarter Frame
     *
     * @param deviceId the device sent this message
     * @param timing 0-16383
     */
    public void OnMidiTimeCodeQuarterFrame(String serializedData) {
        Debug.Log("MidiTimeCodeQuarterFrame: " + DeserializeData(serializedData));

    }

    /**
     * Song Select
     *
     * @param deviceId the device sent this message
     * @param song 0-127
     */
    public void OnMidiSongSelect(String serializedData) {
        Debug.Log("MidiSongSelect: " + DeserializeData(serializedData));

    }

    /**
     * Song Position Pointer
     *
     * @param deviceId the device sent this message
     * @param position 0-16383
     */
    public void OnMidiSongPositionPointer(String serializedData) {
        Debug.Log("MidiSongPositionPointer: " + DeserializeData(serializedData));

    }

    /**
     * Tune Request
     *
     * @param deviceId the device sent this message
     */
    public void OnMidiTuneRequest(String deviceId) {
        Debug.Log("MidiTuneRequest: " + deviceId);

    }

    /**
     * Timing Clock
     *
     * @param deviceId the device sent this message
     */
    public void OnMidiTimingClock(String deviceId) {
        Debug.Log("MidiTimingClock: " + deviceId);

    }

    /**
     * Start Playing
     *
     * @param deviceId the device sent this message
     */
    public void OnMidiStart(String deviceId) {
        Debug.Log("MidiStart: " + deviceId);

    }

    /**
     * Continue Playing
     *
     * @param deviceId the device sent this message
     */
    public void OnMidiContinue(String deviceId) {
        Debug.Log("MidiContinue: " + deviceId);

    }

    /**
     * Stop Playing
     *
     * @param deviceId the device sent this message
     */
    public void OnMidiStop(String deviceId) {
        Debug.Log("MidiStop: " + deviceId);

    }

    /**
     * Active Sensing
     *
     * @param deviceId the device sent this message
     */
    public void OnMidiActiveSensing(String deviceId) {
        Debug.Log("MidiActiveSensing: " + deviceId);

    }

    /**
     * Reset Device
     *
     * @param deviceId the device sent this message
     */
    public void OnMidiReset(String deviceId) {
        Debug.Log("MidiReset: " + deviceId);
    }

    /**
     * MIDI input device has been attached
     *
     * @param deviceId attached MIDI Input device ID
     */
    public void OnMidiInputDeviceAttached(String deviceId) {
        Debug.Log("InputDeviceAttached:" + deviceId);
    }

    /**
     * MIDI output device has been attached
     *
     * @param deviceId attached MIDI Output device ID
     */
    public void OnMidiOutputDeviceAttached(String deviceId) 
    {
        Debug.Log("OutputDeviceAttached:" + deviceId);
    }

    /**
     * MIDI input device has been detached
     *
     * @param deviceId detached MIDI Input device ID
     */
    public void OnMidiInputDeviceDetached(String deviceId) {
        Debug.Log("InputDeviceDetached:" + deviceId);

    }

    /**
     * MIDI output device has been detached
     *
     * @param deviceId detached MIDI Output device ID
     */
    public void OnMidiOutputDeviceDetached(String deviceId) {
        Debug.Log("OutputDeviceDetached:" + deviceId);
    }
}
