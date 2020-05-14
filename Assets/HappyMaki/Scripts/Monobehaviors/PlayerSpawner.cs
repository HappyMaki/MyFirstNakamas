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
    List<string> remotePlayersToDestroy = new List<string>();
    Dictionary<string, GameObject> remotePlayers = new Dictionary<string, GameObject>();

    private void Start()
    {
        nakamaDataRelay = FindObjectOfType<NakamaDataRelay>();

        EventManager.onLocalConnectedPlayer.AddListener(SpawnLocalPlayer);
        EventManager.onRemoteConnectedPlayer.AddListener(SpawnRemotePlayer);
        EventManager.onRemoteDisconnectedPlayer.AddListener(DeleteRemotePlayer);
    }

    void DeleteRemotePlayer(IUserPresence presence)
    {
        remotePlayersToDestroy.Add(presence.UserId);
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

    GameObject InstantiateRemotePlayer(string id)
    {
        GameObject rPlayer = Instantiate(remotePlayerPrefab, transform.position, transform.rotation);
        rPlayer.name = id;
        return rPlayer;
    }

    private void Update()
    {
        if (remotePlayersToSpawn.Count > 0)
        {
            for (int i = 0; i < remotePlayersToSpawn.Count; i++)
            {
                GameObject obj = InstantiateRemotePlayer(remotePlayersToSpawn[i]);
                remotePlayers.Add(remotePlayersToSpawn[i], obj);
            }
            remotePlayersToSpawn.Clear();
        }

        if (remotePlayersToDestroy.Count > 0)
        {
            for (int i = 0; i < remotePlayersToDestroy.Count; i++)
            {
                GameObject.Destroy(remotePlayers[remotePlayersToDestroy[i]]);
                remotePlayers.Remove(remotePlayersToDestroy[i]);
            }
            remotePlayersToDestroy.Clear();
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
