using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NakamaGroups : SingletonBehaviour<NakamaGroups>
{
    const string online_players_group_id = "61bad96f-a5f7-4298-b057-88eb2c3780c2";

    NakamaApi nakama;
    ISession session;
    Client client;
    void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();
        session = nakama.Session;
        client = nakama.Client;
        JoinGroup(online_players_group_id); //players online group
        ListGroupMembers(online_players_group_id);
    }

    public async void ListGroupMembers(string groupId)
    {
        var result = await client.ListGroupUsersAsync(session, groupId, state:2, limit:100, cursor:null);
        foreach (var ug in result.GroupUsers)
        {
            IApiUser g = ug.User;
            Debug.LogFormat("User '{0}' role '{1}', '{2}'", g.Id, g.Username, g.Online);
        }
    }

    public async void JoinGroup(string groupId)
    {
        await client.JoinGroupAsync(session, groupId);
        Debug.LogFormat("Sent group join request '{0}'", groupId);
    }
    public async void LeaveGroup(string groupId)
    {
        await client.LeaveGroupAsync(session, groupId);
        Debug.LogFormat("Sent group leave request '{0}'", groupId);
    }
    private void OnDestroy()
    {
        if (session != null)
            LeaveGroup(online_players_group_id); //players online group
    }

}
