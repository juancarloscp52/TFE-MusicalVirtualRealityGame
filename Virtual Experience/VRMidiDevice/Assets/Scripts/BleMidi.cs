using UnityEngine;

public class BleMidi : MonoBehaviour
{

    AndroidJavaObject bleMidiPlugin;
    // Start is called before the first frame update
    void Start()
    {

        // Invoke Java Native Librarie
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"); 
        bleMidiPlugin = new AndroidJavaObject("jp.kshoji.unity.midi.BleMidiUnityPlugin");
        bleMidiPlugin.Call("initialize", currentActivity);
        Debug.Log("Initialized BLE:yes");
        bleMidiPlugin.Call("startScanDevice", 0); // Scan for devices, without timeout.
        Debug.Log("started scan:yes");

    }

    private void OnDestroy()
    {
        if(bleMidiPlugin != null)
        {
            bleMidiPlugin.Call("terminate");

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
