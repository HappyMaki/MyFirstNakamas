using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerHandler : MonoBehaviour
{
    public GameObject localMalePlayerPrefab;
    public GameObject localFemalePlayerPrefab;
    public GameObject remoteMalePlayerPrefab;
    public GameObject remoteFemalePlayerPrefab;
    public static PlayerGender localPlayerGender;

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

    void SpawnLocalPlayer(Vector3 position, Quaternion rotation, PlayerGender gender)
    {
        switch (gender)
        {
            case PlayerGender.MALE:
                localPlayer = Instantiate(localMalePlayerPrefab, position, rotation);
                localPlayer.name = nakamaDataRelay.ClientId;
                break;

            case PlayerGender.FEMALE:
                localPlayer = Instantiate(localFemalePlayerPrefab, position, rotation);
                localPlayer.name = nakamaDataRelay.ClientId;
                break;

            default:
                break;
        }
        
    }

    void SpawnRemotePlayer(IUserPresence presence)
    {
        remotePlayersToSpawn.Add(presence.UserId);
    }

    GameObject InstantiateRemotePlayer(PlayerDataResponse response)
    {
        PlayerGender gender = (PlayerGender)System.Enum.Parse(typeof(PlayerGender), response.gender);
        GameObject rPlayer;
        switch (gender)
        {
            case PlayerGender.MALE:
                rPlayer = Instantiate(remoteMalePlayerPrefab, response.position, response.rotation);
                rPlayer.tag = "init_not_synced";
                rPlayer.name = response.userId;
                return rPlayer;

            case PlayerGender.FEMALE:
                rPlayer = Instantiate(remoteFemalePlayerPrefab, response.position, response.rotation);
                rPlayer.tag = "init_not_synced";
                rPlayer.name = response.userId;
                return rPlayer;

            default:
                return null;
        }
    }

    private void Update()
    {
        if (remotePlayersToDestroy.Count > 0)
        {
            for (int i = 0; i < remotePlayersToDestroy.Count; i++)
            {
                GameObject.Destroy(remotePlayers[remotePlayersToDestroy[i]]);
                remotePlayers.Remove(remotePlayersToDestroy[i]);
            }
            remotePlayersToDestroy.Clear();
        }

        if (nakamaDataRelay.PlayerData != null)
        {
            Dictionary<string, PlayerDataResponse> playerDataCopy = new Dictionary<string, PlayerDataResponse>(nakamaDataRelay.PlayerData);
            foreach (KeyValuePair<string, PlayerDataResponse> entry in playerDataCopy)
            {
                if (entry.Key == nakamaDataRelay.ClientId) //is local player
                {
                    if (!initialized)
                    {
                        localPlayerGender = (PlayerGender)System.Enum.Parse(typeof(PlayerGender), entry.Value.gender);
                        SpawnLocalPlayer(entry.Value.position, entry.Value.rotation, localPlayerGender);
                        localPlayer.transform.position = entry.Value.position;
                        localPlayer.transform.rotation = entry.Value.rotation;
                        localPlayer.transform.localScale = entry.Value.scale;
                        initialized = true;
                    }
                    continue;
                }
                if (remotePlayers.ContainsKey(entry.Key))
                {
                    if (remotePlayers[entry.Key].tag == "init_not_synced")
                    {
                        remotePlayers[entry.Key].transform.position = entry.Value.position;
                        remotePlayers[entry.Key].transform.rotation = entry.Value.rotation;
                        remotePlayers[entry.Key].transform.localScale = entry.Value.scale;
                        remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("InputHorizontal", entry.Value.InputHorizontal);
                        remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("InputVertical", entry.Value.InputVertical);
                        remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("InputMagnitude", entry.Value.InputMagnitude);
                        remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("GroundDistance", entry.Value.GroundDistance);
                        remotePlayers[entry.Key].GetComponent<Animator>().SetBool("IsGrounded", entry.Value.IsGrounded);
                        remotePlayers[entry.Key].GetComponent<Animator>().SetBool("IsStrafing", entry.Value.IsStrafing);
                        remotePlayers[entry.Key].GetComponent<Animator>().SetBool("IsSprinting", entry.Value.IsSprinting);
                        remotePlayers[entry.Key].tag = "init_synced";
                    }

                    remotePlayers[entry.Key].transform.position = Vector3.MoveTowards(remotePlayers[entry.Key].transform.position, entry.Value.position, 0.15f);
                    remotePlayers[entry.Key].transform.rotation = entry.Value.rotation;
                    remotePlayers[entry.Key].transform.localScale = Vector3.MoveTowards(remotePlayers[entry.Key].transform.localScale, entry.Value.scale, 0.15f);
                    remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("InputHorizontal", entry.Value.InputHorizontal);
                    remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("InputVertical", entry.Value.InputVertical);
                    remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("InputMagnitude", entry.Value.InputMagnitude);
                    remotePlayers[entry.Key].GetComponent<Animator>().SetFloat("GroundDistance", entry.Value.GroundDistance);
                    remotePlayers[entry.Key].GetComponent<Animator>().SetBool("IsGrounded", entry.Value.IsGrounded);
                    remotePlayers[entry.Key].GetComponent<Animator>().SetBool("IsStrafing", entry.Value.IsStrafing);
                    remotePlayers[entry.Key].GetComponent<Animator>().SetBool("IsSprinting", entry.Value.IsSprinting);
                }
                else //We haven't spawned this player yet
                {
                    GameObject obj = InstantiateRemotePlayer(entry.Value);
                    remotePlayers.Add(entry.Key, obj);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (localPlayer && initialized)
            nakamaDataRelay.SendData(localPlayer);

        
    }

}
