using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataResponse
{
    public string userId;
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public PlayerDataResponse(string id, string n, Transform t)
    {
        userId = id;
        name = n;

        position = t.position;
        rotation = t.rotation;
        scale = t.localScale;
    }
}
