using Nakama;
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
    private void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();
        socket = nakama.MatchSocket;
        match = nakama.Match;

        AddReceivedMatchPresenceListener();
        AddReceivedMatchStateListener();
    }

    void AddReceivedMatchPresenceListener()
    {
        socket.ReceivedMatchPresence += presenceEvent =>
        {
            foreach (var presence in presenceEvent.Leaves)
            {
                connectedOpponents.Remove(presence);
            }
            connectedOpponents.AddRange(presenceEvent.Joins);
            // Remove yourself from connected opponents.
            //connectedOpponents.Remove(self);
            Debug.LogFormat("Connected opponents: [{0}]", string.Join(",\n  ", connectedOpponents));
        };

        connectedOpponents.AddRange(match.Presences);
        Debug.LogFormat("Connected opponents: [{0}]", string.Join(",\n  ", connectedOpponents));

    }

    void AddReceivedMatchStateListener()
    {
        socket.ReceivedMatchState += (state) => Debug.Log(System.Text.Encoding.UTF8.GetString(state.State, 0, state.State.Length));
    }


}
