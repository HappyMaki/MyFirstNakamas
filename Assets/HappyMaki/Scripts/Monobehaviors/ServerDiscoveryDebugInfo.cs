using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ServerDiscoveryDebugInfo : MonoBehaviour
{
    public TextMeshProUGUI text;
    public NakamaClient nakamaClient;

    private void Start()
    {
        text.text = "Server discovery... " + nakamaClient.serverIpAddress + ":" + nakamaClient.serverPort.ToString();
    }
}
