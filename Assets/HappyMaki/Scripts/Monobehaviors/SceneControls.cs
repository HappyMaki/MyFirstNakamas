using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour
{
    public string nextScene;
    void Start()
    {
        EventManager.onServerDiscovery.AddListener(SwitchScenes);
        EventManager.onLoginAttempt.AddListener(SwitchScenes);
    }

    void SwitchScenes()
    {
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
    void SwitchScenes(AccountLoginResolution resolution)
    {
        switch (resolution)
        {
            case AccountLoginResolution.SUCCESS:
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
                break;

            default:
                break;
        }
    }
}
