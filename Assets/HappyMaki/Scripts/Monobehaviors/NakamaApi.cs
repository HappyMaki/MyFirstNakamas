using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.Networking;

public class NakamaApi : SingletonBehaviour<NakamaApi>
{
    public string serverIpAddress;
    public int serverPort;

    string deviceId;
    Client client;
    ISession session;
    

    async void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
        client = new Client("http", serverIpAddress, serverPort, "ZdsG11p&y13zl6a");
        ServerDiscovery();
        //session = await client.AuthenticateDeviceAsync(deviceId);
        //Debug.LogFormat("Server Discovery: {0}, {1}", session.Created, session);


    }

    private void ServerDiscovery()
    {
        StartCoroutine(PingServer());
    }

    IEnumerator PingServer()
    {
        string server_url = "http://" + serverIpAddress + ":" + serverPort.ToString();
        UnityWebRequest resp = UnityWebRequest.Get(server_url);
        yield return resp.SendWebRequest();

        if (resp.isNetworkError || resp.isHttpError)
        {
            throw new System.EntryPointNotFoundException(resp.error);
        }
        else
        {
            EventManager.onServerDiscovery.Invoke();

        }
    }

    public async void Register(string name, string email, string password)
    {
        try
        {
            session = await client.AuthenticateEmailAsync(email, password, username: name);
            EventManager.onAccountCreation.Invoke(AccountCreationResolution.SUCCESS);
            DebugInfo.SetToast("Registration Success", "Your account has been created. Please log in with your character name.");

        }
        catch (ApiResponseException e)
        {
            //if (e.Message == "Invalid credentials.")
            //{
            //    DebugInfo.SetToast("Error", e.Message);
            //    return;
            //}
            if (e.Message == "Username is already in use.")
            {
                DebugInfo.SetToast("Error", e.Message);
                EventManager.onAccountCreation.Invoke(AccountCreationResolution.FAILED);
            }
            else if (e.Message == "Invalid credentials.")
            {
                DebugInfo.SetToast("Error", "Email already in use.");
                EventManager.onAccountCreation.Invoke(AccountCreationResolution.FAILED);
            }
            else
            {
                DebugInfo.SetToast("Error", e.Message);
            }
            
        }

    }

    public async void Login(string account, string password)
    {
        Debug.Log("Login: " + account + ", " + password);
    }
}
