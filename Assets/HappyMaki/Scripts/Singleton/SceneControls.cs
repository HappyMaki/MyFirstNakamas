using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : SingletonBehaviour<SceneControls>
{
    public string nextScene;
    NakamaApi nakama;

    void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();

        EventManager.onServerDiscovery.AddListener(SwitchScenes); //This only runs locally before we connect
        EventManager.onLoginAttempt.AddListener(HandleNetworkLogin); //This only runs locally before we connect

        EventManager.onGetMatchId.AddListener(JoinNetworkScene);
        EventManager.onRoomJoin.AddListener(SwitchNetworkScenes);
    }

    void SwitchScenes()
    {
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }

    void HandleNetworkLogin(AccountLoginResolution resolution)
    {
        if (resolution == AccountLoginResolution.SUCCESS)
        {
            //JoinNetworkScene(nextScene, LoadSceneMode.Single);
            StartCoroutine(nakama.RPC_GetMatchID(nextScene));
        }
    }

    void SwitchNetworkScenes(string scene)
    {
        if (nextScene != scene) //if more than one door exists
            return;

        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    void JoinNetworkScene(MatchJoinResponse response)
    {
        string matchId = response.payload;
        Debug.Log("Joining room: " + matchId);
        nakama.JoinMatchIdAsync(matchId, nextScene);
    }

    
}
