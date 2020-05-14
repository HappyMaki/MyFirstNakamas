using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataRequest 
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public PlayerDataRequest(GameObject obj)
    {
        Transform t = obj.transform;

        position = t.position;
        rotation = t.rotation;
        scale = t.localScale;
    }
}
