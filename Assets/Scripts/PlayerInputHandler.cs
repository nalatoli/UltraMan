using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(MovementController))]
public class PlayerInputHandler : NetworkBehaviour
{
    private MovementController movementController;


    public override void OnNetworkSpawn()
    {
        movementController = GetComponent<MovementController>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if(TryGetInput(out bool[] input))
            movementController.RequestAdjacentMovementServerRpc(input);

    }

    private bool TryGetInput(out bool[] input)
    {
        input = new bool[4];
        bool inputDetected = false;

        if (Input.GetKeyDown(KeyCode.W)) { input[0] = true; inputDetected = true; }
        if (Input.GetKeyDown(KeyCode.S)) { input[1] = true; inputDetected = true; }
        if (Input.GetKeyDown(KeyCode.A)) { input[2] = true; inputDetected = true; }
        if (Input.GetKeyDown(KeyCode.D)) { input[3] = true; inputDetected = true; }

        return inputDetected;
    }
}
