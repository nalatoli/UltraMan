using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Entity))]
public class MovementController : NetworkBehaviour
{
    public NetworkAnimator animator;
    private Entity entity;
    private float RayCastDistance => 100f;

    public override void OnNetworkSpawn()
    {
        entity = GetComponent<Entity>();
    }

    [ServerRpc]
    public void RequestAdjacentMovementServerRpc(bool[] inputs, ServerRpcParams serverRpcParams = default)
    {
        Vector2 moveDir = Vector2.zero;
        if (inputs[0]) moveDir.y = +1f;
        if (inputs[1]) moveDir.y = -1f;
        if (inputs[2]) moveDir.x = -1f;
        if (inputs[3]) moveDir.x = +1f;

        if (moveDir == Vector2.zero)
            return;

        Vector2 origin = new(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDir, RayCastDistance, UltraMan.LayerMasks.Stage);

        if (hit.collider == null)
            return;

        if (!hit.collider.TryGetComponent(out Entity stageEntity))
            throw new System.ApplicationException(hit.collider.name + " does not contain an Entity component");

        if(stageEntity.StageSide.Value != entity.StageSide.Value)
            return;

        Debug.Log(hit.collider.name);

        if (animator != null)
            animator.SetTrigger(UltraMan.AnimationParameters.OnPhaseTrigger);

        transform.position = new Vector3(
            hit.collider.transform.position.x,
            hit.collider.transform.position.y,
            transform.position.z);

    }
}
