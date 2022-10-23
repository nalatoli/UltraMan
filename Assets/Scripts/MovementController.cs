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

    #region Network Properties

    public NetworkVariable<Vector3> Position = new();

    #endregion

    private void Start()
    {
        entity = GetComponent<Entity>();
        Position.Value = transform.position;
    }

    private void Update()
    {
        transform.position = Position.Value;
    }

    [ServerRpc]
    public void MoveAdjacentServerRpc(Vector2 dir)
    {
        Vector2 origin = new(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, RayCastDistance, UltraMan.LayerMasks.Stage);

        if (hit.collider == null)
            return;

        if (!hit.collider.TryGetComponent(out Entity stageEntity))
            throw new System.ApplicationException(hit.collider.name + " does not contain an Entity component");

        if(stageEntity.StageSide != entity.StageSide)
            return;

        if (animator != null)
            animator.SetTrigger(UltraMan.AnimationParameters.OnPhaseTrigger);

        Position.Value = new Vector3(
            hit.collider.transform.position.x,
            hit.collider.transform.position.y,
            transform.position.z);

    }
}
