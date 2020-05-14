using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerHandler : MonoBehaviour
{
    public GameObject localPlayerPrefab;
    public GameObject remotePlayerPrefab;

    GameObject localPlayer;
    NakamaDataRelay nakamaDataRelay;

    List<string> remotePlayersToSpawn = new List<string>();
    List<string> remotePlayersToDestroy = new List<string>();
    Dictionary<string, GameObject> remotePlayers = new Dictionary<string, GameObject>();
    bool initialized = false;

    private void Start()
    {
        nakamaDataRelay = FindObjectOfType<NakamaDataRelay>();

        //EventManager.onLocalConnectedPlayer.AddListener(SpawnLocalPlayer);
        EventManager.onRemoteConnectedPlayer.AddListener(SpawnRemotePlayer);
        EventManager.onRemoteDisconnectedPlayer.AddListener(DeleteRemotePlayer);
    }

    void DeleteRemotePlayer(IUserPresence presence)
    {
        remotePlayersToDestroy.Add(presence.UserId);
    }

    void SpawnLocalPlayer(Vector3 position, Quaternion rotation)
    {
        localPlayer = Instantiate(localPlayerPrefab, position, rotation);
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
        if (localPlayer && initialized)
            nakamaDataRelay.SendData(localPlayer);

        if (nakamaDataRelay.PlayerData != null)
        {
            Dictionary<string, PlayerDataResponse> playerDataCopy = new Dictionary<string, PlayerDataResponse>(nakamaDataRelay.PlayerData);
            foreach (KeyValuePair<string, PlayerDataResponse> entry in playerDataCopy)
            {
                if (entry.Key == nakamaDataRelay.ClientId) //is local player
                {
                    if (!initialized)
                    {
                        Debug.Log(entry.Value.position);
                        //localPlayer.transform.position = new Vector3(entry.Value.position.x, entry.Value.position.y + 0.2f, entry.Value.position.z);
                        SpawnLocalPlayer(entry.Value.position, entry.Value.rotation);
                        localPlayer.transform.position = entry.Value.position;
                        localPlayer.transform.rotation = entry.Value.rotation;
                        localPlayer.transform.localScale = entry.Value.scale;
                        initialized = true;
                    }
                    continue;
                }
                if (remotePlayers.ContainsKey(entry.Key))
                {
                    remotePlayers[entry.Key].transform.position = Vector3.MoveTowards(remotePlayers[entry.Key].transform.position, entry.Value.position, 0.15f);
                    remotePlayers[entry.Key].transform.rotation = entry.Value.rotation;
                    remotePlayers[entry.Key].transform.localScale = Vector3.MoveTowards(remotePlayers[entry.Key].transform.localScale, entry.Value.scale, 0.15f);
                }
            }
        }
    }

}
