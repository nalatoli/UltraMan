using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(MovementController))]
public class PlayerInputHandler : MonoBehaviour
{
    private MovementController movementController;

    private Vector2 lastSnapInput = Vector2.zero;

    private void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    void Update()
    {
        Vector2 snapInput = GetSnapInput();
        if (snapInput != lastSnapInput)
        {
            if(snapInput != Vector2.zero)
                movementController.MoveAdjacentServerRpc(snapInput);
                
            lastSnapInput = snapInput;
        }
    }

    private Vector2 GetSnapInput()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        return new Vector2(Mathf.Sign(hInput) * Mathf.Ceil(Mathf.Abs(hInput)), Mathf.Sign(vInput) * Mathf.Ceil(Mathf.Abs(vInput)));
    }
}
