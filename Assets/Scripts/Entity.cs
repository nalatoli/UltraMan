using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public NetworkVariable<Side> StageSide;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, StageSide.Value == Side.Right ? 180 : 0, 0);
    }
}
