using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Text;
using Assets;

public class UDPListener : MonoBehaviour 
{
    public int Port = 11011;

    public delegate void StickEvent (JoystickState state);
	public static event StickEvent StickEventListener;

    private UdpClient _listener;
    private IPEndPoint _groupEP;

    void Start () 
    {
        _groupEP = new IPEndPoint(IPAddress.Any, Port);
        Debug.Log("Starting UDP listener on port " + Port);

        _listener = new UdpClient(Port);

    }

	void Update () 
    {
        try
        {
            while (_listener.Available > 0)
            {
                byte[] recieveBytes = _listener.Receive(ref _groupEP);
                string[] message = Encoding.ASCII.GetString(recieveBytes).Split(',');

                Debug.Log("Got packet: " + String.Join(",", message));

                StickEventListener?.Invoke(new JoystickState(message));
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to read from UDP socket\n" + e.ToString());
        }
    }
}
