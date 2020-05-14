using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is intended to exist only once per scene. It manages the remote game objects and transmits/receives data from the nakama server
public class NakamaDataRelay : SingletonBehaviour<NakamaDataRelay>
{
    List<IUserPresence> connectedOpponents = new List<IUserPresence>(2);

    NakamaApi nakama;
    IMatch match;
    ISocket socket;
    ISession session;
    string userId;

    String gameState;


    private void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();
        socket = nakama.MatchSocket;
        match = nakama.Match;
        session = nakama.Session;
        userId = session.UserId;

        EventManager.onLocalConnectedPlayer.Invoke();

        Debug.Log("My Client ID is " + userId);

        AddReceivedMatchPresenceListener();
        AddReceivedMatchStateListener();
    }

    public string GameState
    {
        get { return gameState; }
    }

    public string ClientId
    {
        get { return userId; }
    }

    public ISession Session
    {
        get { return session; }
    }

    public void SendData(GameObject obj)
    {
        long opCode = 1;
        string payload = JsonUtility.ToJson(new PlayerPayload(obj.transform));
        string newState = new Dictionary<string, string> { { "payload", payload } }.ToJson();
        socket.SendMatchStateAsync(match.Id, opCode, newState);
    }

    void AddReceivedMatchPresenceListener()
    {
        socket.ReceivedMatchPresence += presenceEvent =>
        {
            foreach (var presence in presenceEvent.Leaves)
            {
                EventManager.onRemoteDisconnectedPlayer.Invoke(presence);
                connectedOpponents.Remove(presence);
            }

            foreach (var presence in presenceEvent.Joins)
            {
                EventManager.onRemoteConnectedPlayer.Invoke(presence);
            }

            connectedOpponents.AddRange(presenceEvent.Joins);

        };

        //Remote players who are already logged in
        connectedOpponents.AddRange(match.Presences);
        for (int i = 0; i < connectedOpponents.Count; i++)
        {
            EventManager.onRemoteConnectedPlayer.Invoke(connectedOpponents[i]);
        }
    }

    void AddReceivedMatchStateListener()
    {
        socket.ReceivedMatchState += (state) =>
        {
            gameState = System.Text.Encoding.UTF8.GetString(state.State, 0, state.State.Length);
        };
    }


}
