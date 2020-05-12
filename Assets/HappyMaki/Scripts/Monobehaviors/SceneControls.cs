using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour
{
    public string nextScene;
    NakamaApi nakama;

    void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();

        EventManager.onServerDiscovery.AddListener(SwitchScenes);
        EventManager.onLoginAttempt.AddListener(SwitchScenes);
        EventManager.onRoomJoin.AddListener(JoinNetworkScene);
    }

    void SwitchScenes()
    {
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
    void SwitchScenes(AccountLoginResolution resolution)
    {
        if (resolution == AccountLoginResolution.SUCCESS)
        {
            //JoinNetworkScene(nextScene, LoadSceneMode.Single);
            StartCoroutine(nakama.RPC_GetMatchID(nextScene));
        }
    }

    void JoinNetworkScene(MatchJoinResponse response)
    {
        string matchId = response.payload;
        Debug.Log("Joining room: " + matchId);
    }

    
}
