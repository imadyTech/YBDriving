//https://kannmu.github.io/2023/06/10/Unity-Python-Interaction/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEditor;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;

public class heyCar : PythonConnector
{
    private void Start()
    {
        base.StartPython();
        base.DataReceived += (sender, e) => { };
    }

    public void SendVision(byte[] msg)
    {
        base.SendMessageToPython(msg);
    }

}
