using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class NakamaClient : SingletonBehaviour<NakamaClient>
{
    public string serverIpAddress;
    public int serverPort;

    Client client;
    ISession session;

    async void Start()
    {
        client = new Client("http", serverIpAddress, serverPort, "ZdsG11p&y13zl6a");

        string deviceId = SystemInfo.deviceUniqueIdentifier;
        session = await client.AuthenticateDeviceAsync(deviceId);
        Debug.LogFormat("New user: {0}, {1}", session.Created, session);
        EventManager.onServerDiscovery.Invoke();
        
    }
}
