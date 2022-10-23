using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Side StageSide;

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(0, StageSide == Side.Right ? 180 : 0, 0);
    }
}
