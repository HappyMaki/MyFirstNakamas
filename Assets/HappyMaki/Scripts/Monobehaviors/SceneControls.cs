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
    }

    void SwitchScenes()
    {
        Debug.Log("Switching Scenes");
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
