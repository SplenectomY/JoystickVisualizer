using Assets;
using System.Collections.Generic;
using UnityEngine;

public class VPCStarCitizenR : MonoBehaviour {
    public const string USB_ID = "3344:412f";
    //public const string USB_ID = "044f:0402";

    public GameObject Model;
    public GameObject Joystick;

    float x = 0f;
    float y = 0f;
    float z = 0f;

    void Start()
    {
        UDPListener.StickEventListener += StickEvent;
        Model.SetActive(true);
    }

    void StickEvent(JoystickState state)
    {
        if (state.UsbID != USB_ID)
        {
            return;
        }

        bool updateStickRotation = false;

        foreach (KeyValuePair<string, int> entry in state.Data)
        {
            switch (entry.Key)
            {
                //case "Connected":
                //    if (Model.activeInHierarchy)
                //        Model.SetActive(entry.Value == 1);
                //    break;

                case "X":
                    //Joystick.transform.localEulerAngles = new Vector3(Joystick.transform.localEulerAngles.x, Joystick.transform.localEulerAngles.y, );
                    z = ConvertRange(entry.Value, 0, 65535, 20, -20);
                    updateStickRotation = true;
                    break;
                case "Y":
                    //Joystick.transform.localEulerAngles = new Vector3(, Joystick.transform.localEulerAngles.y, Joystick.transform.localEulerAngles.z);
                    x = ConvertRange(entry.Value, 0, 65535, 20, -20);
                    updateStickRotation = true;
                    break;
                case "Z":
                    //Joystick.transform.localEulerAngles = new Vector3(Joystick.transform.localEulerAngles.x, , Joystick.transform.localEulerAngles.z);
                    y = -ConvertRange(entry.Value, 0, 65535, 20, -20);
                    updateStickRotation = true;
                    break;
            }
        }

        if (updateStickRotation)
        {
            Joystick.transform.localEulerAngles = new Vector3(x, y, z);
        }
    }

    public static float ConvertRange(
        double value, // value to convert
        double originalStart, double originalEnd, // original range
        double newStart, double newEnd) // desired range
    {
        double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
        return (float)(newStart + ((value - originalStart) * scale));
    }
}
