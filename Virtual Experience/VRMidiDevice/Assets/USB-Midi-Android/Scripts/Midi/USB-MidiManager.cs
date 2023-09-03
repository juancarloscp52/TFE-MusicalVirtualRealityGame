using System;
using UnityEngine;

public class UsbMidiManager : MonoBehaviour
{
    public static UsbMidiManager Instance = null;
    private UnityMidiAndroid _midiAndroid;
    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEventHandler(IMidiEventHandler eventHandler)
    {
        _midiAndroid = new UnityMidiAndroid(eventHandler);
    }
}