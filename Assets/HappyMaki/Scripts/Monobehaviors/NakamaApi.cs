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
    
    void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
        client = new Client("http", serverIpAddress, serverPort, "ZdsG11p&y13zl6a");
        ServerDiscovery();


    }

    void ServerDiscovery()
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

    public async void Register(string name, string email, string password, PlayerGender gender)
    {
        try
        {
            session = await client.AuthenticateEmailAsync(email, password, username: name);
            SaveGenderSelection(gender);
            EventManager.onAccountCreation.Invoke(AccountRegisterResolution.SUCCESS);
            DebugInfo.SetToast("Registration Success", "Your account has been created. Please log in with your character name.");
        }
        catch (ApiResponseException e)
        {
            if (e.Message == "Username is already in use.")
            {
                DebugInfo.SetToast("Error", e.Message);
                EventManager.onAccountCreation.Invoke(AccountRegisterResolution.FAILED);
            }
            else if (e.Message == "Invalid credentials.")
            {
                DebugInfo.SetToast("Error", "Email already in use.");
                EventManager.onAccountCreation.Invoke(AccountRegisterResolution.FAILED);
            }
            else
            {
                DebugInfo.SetToast("Error", e.Message);
                EventManager.onAccountCreation.Invoke(AccountRegisterResolution.FAILED);
            }
        }
    }

    public async void Login(string account, string password)
    {
        try
        {
            Debug.Log("Login: " + account + ", " + password);
            session = await client.AuthenticateEmailAsync(account, password, username: name, create: false);
            //GetPlayerCharacterInfo();

            EventManager.onLoginAttempt.Invoke(AccountLoginResolution.SUCCESS);
            DebugInfo.SetToast("Login Success", "Entering the world!");
        }
        catch (ApiResponseException e)
        {
            DebugInfo.SetToast("Error", e.Message);
            EventManager.onLoginAttempt.Invoke(AccountLoginResolution.FAILED);
        }
    }

    void SaveGenderSelection(PlayerGender gender)
    {
        WriteStorageObject[] obj = new WriteStorageObject[]
        {
            new WriteStorageObject{
                Collection = "character",
                Key = "base",
                Value = "{\"Gender\":\"" + gender.ToString() + "\"}"
            }
        };
        StoreData(obj);
    }

    void GetPlayerCharacterInfo()
    {
        StorageObjectId[] objs = new StorageObjectId[]
        {
            new StorageObjectId
            {
                Collection = "character",
                Key = "base",
                UserId = session.UserId
            }
        };
        GetData(objs);
    }


    public async void StoreData(WriteStorageObject[] objects)
    {
        var objectIds = await client.WriteStorageObjectsAsync(session, objects);
        Debug.LogFormat("Successfully stored objects: [{0}]", string.Join(",\n   ", objectIds));
    }

    public async void GetData(StorageObjectId[] objectIds)
    {
        var result = await client.ReadStorageObjectsAsync(session, objectIds);
        Debug.LogFormat("Read objects: [{0}]", string.Join(",\n  ", result.Objects));
    }
}
