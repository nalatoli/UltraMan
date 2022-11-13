using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(BasicAttackController))]
[RequireComponent(typeof(MovementController))]
public class PlayerInputHandler : NetworkBehaviour
{
    private MovementController movementController;
    private BasicAttackController attackController;


    public override void OnNetworkSpawn()
    {
        movementController = GetComponent<MovementController>();
        attackController = GetComponent<BasicAttackController>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        TryGetInput(out InputState input);
        
        movementController.RequestAdjacentMovementServerRpc(input);
        attackController.ChangeInputServerRpc(input);
        
            

    }

    private bool TryGetInput(out InputState input)
    {
        input = new InputState();
        bool inputDetected = false;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) { input.IsMoveUpKeyDown = true; inputDetected = true; }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) { input.IsMoveDownKeyDown = true; inputDetected = true; }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) { input.IsMoveLeftKeyDown = true; inputDetected = true; }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { input.IsMoveRightKeyDown = true; inputDetected = true; }
        if (Input.GetKey(KeyCode.Q)) { input.IsBasicFireKeyPressed = true; inputDetected = true; }

        return inputDetected;
    }
}
