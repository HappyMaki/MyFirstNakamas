using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject localPlayerPrefab;
    public GameObject remotePlayerPrefab;

    GameObject localPlayer;
    NakamaDataRelay nakamaDataRelay;

    List<string> remotePlayersToSpawn = new List<string>();

    private void Start()
    {
        nakamaDataRelay = FindObjectOfType<NakamaDataRelay>();

        EventManager.onLocalConnectedPlayer.AddListener(SpawnLocalPlayer);
        EventManager.onRemoteConnectedPlayer.AddListener(SpawnRemotePlayer);
    }

    void SpawnLocalPlayer()
    {
        localPlayer = Instantiate(localPlayerPrefab, transform.position, transform.rotation);
        localPlayer.name = nakamaDataRelay.ClientId;
    }

    void SpawnRemotePlayer(IUserPresence presence)
    {
        remotePlayersToSpawn.Add(presence.UserId);
    }

    void InstantiateRemotePlayer(string id)
    {
        GameObject rPlayer = Instantiate(remotePlayerPrefab, transform.position, transform.rotation);
        rPlayer.name = id;
    }

    private void Update()
    {
        if (remotePlayersToSpawn.Count > 0)
        {
            for (int i = 0; i < remotePlayersToSpawn.Count; i++)
            {
                InstantiateRemotePlayer(remotePlayersToSpawn[i]);
                remotePlayersToSpawn.Clear();
            }
        }
    }

    private void FixedUpdate()
    {
        if (localPlayer)
            nakamaDataRelay.SendData(localPlayer);

        if (nakamaDataRelay.GameState != null)
        {

        }
    }

}
