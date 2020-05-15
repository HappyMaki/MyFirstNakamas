using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpGate : MonoBehaviour
{
    public string nextScene;
    public Transform nextTransform;

    NakamaApi nakama;
    SceneControls sceneControls;

    private void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();
        sceneControls = FindObjectOfType<SceneControls>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "local_player")
        {
            Debug.Log("SENDING YOU AWAY");
            nakama.LeaveMatch();
            sceneControls.nextScene = nextScene;
            StartCoroutine(nakama.RPC_GetMatchID(nextScene));
        }
    }
}
