using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBoard : MonoBehaviour
{

    public List<GameObject> buttons = new();
    public GameObject controllerAnchor;
    public ControllerActiveState controllerActiveState;
    public GameObject pitchPot;
    public GameObject bpmPot;
    public GameObject volumePot;
    public GameObject intervalPot;

    public bool[] state;
    Vector3 lastGoodPos;
    Quaternion lastGoodRot;

    private float expectedPitchPot;
    private float expectedBPMPot;
    private float expectedVolumePot;
    private float expectedIntervalPot;


    private float VolumePotRot;
    private float PitchPotRot;
    private float BPMPotRot;
    private float IntervalPotPos;

    private const int maxPotRot = 295;

    private const float minIntervalPos = 0.25f;
    //private const float maxIntervalPos = -0.07f;
    private const float maxDis = 0.32f;


    // Start is called before the first frame update
    void Start()
    {
        state = new bool[buttons.Count];
    }

    // Update is called once per frame
    void Update()
    {
        // Update Potentiometer rotation: 

        VolumePotRot = volumePot.transform.localRotation.eulerAngles.y;
        volumePot.transform.SetLocalPositionAndRotation(
            volumePot.transform.localPosition,
            Quaternion.Euler(0,Mathf.Lerp(VolumePotRot,expectedVolumePot,0.1f),0));

        PitchPotRot = pitchPot.transform.localRotation.eulerAngles.y;
        pitchPot.transform.SetLocalPositionAndRotation(
            pitchPot.transform.localPosition,
            Quaternion.Euler(0, Mathf.Lerp(PitchPotRot, expectedPitchPot, 0.1f), 0));

        BPMPotRot = bpmPot.transform.localRotation.eulerAngles.y;
        bpmPot.transform.SetLocalPositionAndRotation(
            bpmPot.transform.localPosition,
            Quaternion.Euler(0, Mathf.Lerp(BPMPotRot, expectedBPMPot, 0.1f), 0));

        // Update Slider position

        IntervalPotPos = intervalPot.transform.localPosition.z;
        intervalPot.transform.SetLocalPositionAndRotation(
            new Vector3(
                intervalPot.transform.localPosition.x,
                intervalPot.transform.localPosition.y,
                Mathf.Lerp(IntervalPotPos, expectedIntervalPot, 0.1f)
            ),
            intervalPot.transform.localRotation
        );


        // Move board position to tracked controller.
        Debug.Log($"PosBoard: {this.transform.parent.position}");
        Debug.Log($"LastPosBoard: {lastGoodPos}");
        if (controllerActiveState.Active)
        {
            transform.parent.SetPositionAndRotation(
                controllerAnchor.transform.position,
                Quaternion.Euler(controllerAnchor.transform.rotation.eulerAngles - new Vector3(23f, 0, 0))
                );

            // Store current position when grip is pressed.
            if (Input.GetAxis("XRI_Left_Grip") > 0)
            {
                lastGoodPos = controllerAnchor.transform.position;
                lastGoodRot = Quaternion.Euler(controllerAnchor.transform.rotation.eulerAngles - new Vector3(23f, 0, 0));
            }
        }

        // If in hand tracking, set the board position to the last tracked position.
        else if(lastGoodPos !=null && lastGoodRot != null)
        {
            transform.parent.SetPositionAndRotation(lastGoodPos, lastGoodRot);
        }

        // Change button position if button pressed.
        for (int i = 0; i<buttons.Count; i++)
        {
            var postion = buttons[i].transform.GetChild(1).localPosition;
            if (state[i])
            {
                postion.y = -0.013f;
            }
            else
            {
                postion.y = 0f;
            }

            buttons[i].transform.GetChild(1).localPosition = postion;
        }

    }
    public void PlayAnim(int buttonIndex)
    {
        buttons[buttonIndex].transform.GetChild(2).GetComponent<Animation>().Play();
    }

    public void setPitchPot(float pitch)
    {
        expectedPitchPot = maxPotRot * (pitch/3);
    }
    public void setBPMPot(float bpmMultiplier)
    {
        expectedBPMPot = maxPotRot * (bpmMultiplier/2);
    }
    public void setVolumePot(float volume)
    {
        expectedVolumePot = maxPotRot * volume;
    }

    public void setIntervalPot(float pos)
    {
        expectedIntervalPot = minIntervalPos - (maxDis * (pos / 127f));
    }
}
