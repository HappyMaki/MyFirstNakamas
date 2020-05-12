using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is intended to exist only once per scene. It manages the remote game objects and transmits/receives data from the nakama server
public class NakamaDataRelay : SingletonBehaviour<NakamaDataRelay>
{
    NakamaApi nakama;
    ISocket socket;
    private void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();
        socket = nakama.MatchSocket;

        AddReceivedMatchStateListener();
    }

    private void AddReceivedMatchStateListener()
    {
        socket.ReceivedMatchState += (state) => Debug.Log(System.Text.Encoding.UTF8.GetString(state.State, 0, state.State.Length));
    }


}
