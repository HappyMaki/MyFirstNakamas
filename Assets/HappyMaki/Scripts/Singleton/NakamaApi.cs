using System.Collections;
using UnityEngine;
using Nakama;
using UnityEngine.Networking;


public class NakamaApi : SingletonBehaviour<NakamaApi>
{
    public string serverIpAddress;
    public int serverPort;

    const string server_key = "ZdsG11p&y13zl6a";
    const string http_key = "XiHe41dci9";


    string deviceId;
    Client client;
    ISession session;
    string server_url;

    void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
        client = new Client("http", serverIpAddress, serverPort, server_key);
        ServerDiscovery();


    }




    public IEnumerator RPC_GetMatchID(string label)
    {
        string endpoint = server_url + "/v2/rpc/join_match_rpc?http_key=" + http_key;

        var request = new UnityWebRequest(endpoint, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        string dataJsonString = "\"{\\\"modulename\\\": \\\"match\\\",\\\"label\\\": \\\"" + label + "\\\" }\"";
        Debug.Log(dataJsonString);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(dataJsonString);
        UploadHandler uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.uploadHandler = uploadHandler;
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); 
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("Error" + request.error + ": " + request.downloadHandler.text);
        }
        else
        {
            //    Debug.Log("Status Code" + request.responseCode + ": " + request.downloadHandler.text);
            MatchJoinResponse response = JsonUtility.FromJson<MatchJoinResponse>(request.downloadHandler.text);
            EventManager.onRoomJoin.Invoke(response);
        }
    }

    #region MainMenu
    void ServerDiscovery()
    {
        StartCoroutine(PingServer());
    }

    IEnumerator PingServer()
    {
        server_url = "http://" + serverIpAddress + ":" + serverPort.ToString();
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
    #endregion

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
