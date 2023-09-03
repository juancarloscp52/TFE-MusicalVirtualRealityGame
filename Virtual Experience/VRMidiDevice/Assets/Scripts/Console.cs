using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();
    public Text display;

    private void Update()
    {
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {

        // Extract log key and value
        string[] splitString = logString.Split(char.Parse(":"),2);
        string debugKey = splitString[0];
        string debugValue = splitString.Length > 1 ? splitString[1] : "";

        // Update value of key if exists, add key and value if it doesn't
        if (debugLogs.ContainsKey(debugKey))
            debugLogs[debugKey] = debugValue;
        else
            debugLogs.Add(debugKey, debugValue);


        // Display log.
        string displayText = "";
        foreach (KeyValuePair<string, string> log in debugLogs)
        {
            if (log.Value == "")
                displayText += log.Key + "\n";
            else
                displayText += log.Key + ": " + log.Value + "\n";
        }

        display.text = displayText;
    }
}
