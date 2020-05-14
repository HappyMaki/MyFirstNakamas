using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerPayload 
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public PlayerPayload(Transform t)
    {
        position = t.position;
        rotation = t.rotation;
        scale = t.localScale;
    }
}
