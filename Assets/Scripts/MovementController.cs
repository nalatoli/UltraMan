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
    public void RequestAdjacentMovementServerRpc(InputState input, ServerRpcParams serverRpcParams = default)
    {
        Vector2 moveDir = Vector2.zero;
        if (input.IsMoveUpKeyDown) moveDir.y = +1f;
        if (input.IsMoveDownKeyDown) moveDir.y = -1f;
        if (input.IsMoveLeftKeyDown) moveDir.x = -1f;
        if (input.IsMoveRightKeyDown) moveDir.x = +1f;

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

        if (animator != null)
            animator.SetTrigger(UltraMan.AnimationParameters.OnPhaseTrigger);

        transform.position = new Vector3(
            hit.collider.transform.position.x,
            hit.collider.transform.position.y,
            transform.position.z);

    }
}
