using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour
{
    public string firstSceneOnNewAccount = "Kerfuffle";
    public string nextScene;
    NakamaApi nakama;

    void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();

        EventManager.onServerDiscovery.AddListener(SwitchScenes); //This only runs locally before we connect
        EventManager.onLoginAttempt.AddListener(GetLoginInformation); //This only runs locally before we connect
        EventManager.onGetMatchId.AddListener(JoinNetworkScene);
        EventManager.onRoomJoin.AddListener(SwitchNetworkScenes);
        EventManager.onGetLoginInformation.AddListener(FinallyLogin);
    }

    private void OnDestroy()
    {
        EventManager.onServerDiscovery.RemoveAllListeners(); 
        EventManager.onLoginAttempt.RemoveAllListeners(); 
        EventManager.onGetMatchId.RemoveAllListeners();
        EventManager.onRoomJoin.RemoveAllListeners();
        EventManager.onGetLoginInformation.RemoveAllListeners();
    }

    void SwitchScenes()
    {
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }

    void GetLoginInformation(AccountLoginResolution resolution)
    {
        if (resolution == AccountLoginResolution.SUCCESS)
        {
            nakama.GetLoginInfo();
        }
    }

    void FinallyLogin(PlayerDataResponse response)
    {
        if (response.scene == null)
        {
            nextScene = firstSceneOnNewAccount;
        }
        else
        {
            nextScene = response.scene;
        }
        Debug.Log(nextScene);
        StartCoroutine(nakama.ClientJoinMatchByMatchId(nextScene));
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
