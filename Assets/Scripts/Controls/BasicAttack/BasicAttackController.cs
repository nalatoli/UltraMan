using System;
using System.Collections;
using System.Collections.Generic;
using UltraMan;
using UltraMan.Infrastructure;
using UltraMan.Managers;
using UltraMan.Managers.SoundManagerHelpers;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;



[RequireComponent(typeof(Entity))]
public class BasicAttackController : FsmStateMachineBehavior
{

    #region FSM

    public class IdleFsmState : FsmState
    {
        private readonly BasicAttackController con;

        public IdleFsmState(BasicAttackController stateMachine) : base("Idle", stateMachine)
        {
            con = stateMachine;
        }

        public override void UpdateLogic()
        {
            /* Set state to 'Charging' if fire key is pressed */
            if (con.currentInput.IsBasicFireKeyPressed)
                con.ChangeState(con.ChargingState);
        }
    }

    public class ChargingFsmState : FsmState
    {
        private readonly BasicAttackController con;
        private float startChargeTime;

        public ChargingFsmState(BasicAttackController stateMachine) : base("State", stateMachine)
        {
            con = stateMachine;
        }

        public override void Enter()
        {
            /* Spawn small shot if not already spawned */
            if (con.smallBusterShotInstance == null || !con.smallBusterShotInstance.IsSpawned)
                con.smallBusterShotInstance = con.CreateShot(con.smallBusterShotPrefab, con.holderTransform, true);

            /* Shoot small shot */
            con.Shoot(con.transform.right, 10);

            /* Play shoot sound */
            SoundManager.PlaySound(con.shootSound);

            /* Animate shooting */
            con.AnimateBasicFire(con.basicFireAnimateDuration);

            /* Start charge time */
            startChargeTime = Time.time;

        }

        public override void UpdateLogic()
        {
            /* Set state to 'Idle' if fire key is not pressed */
            if(!con.currentInput.IsBasicFireKeyPressed)
                con.ChangeState(con.IdleState);

            /* Get current charge time */
            float currentChargeTime = Time.time - startChargeTime;

            /* Set state to 'Ready' if current charge time reached threshold */
            if (currentChargeTime > con.chargeTimeThreshold)
                con.ChangeState(con.ReadyState);
        }
    }

    public class ReadyFsmState : FsmState
    {
        private readonly BasicAttackController con;

        public ReadyFsmState(BasicAttackController stateMachine) : base("Ready", stateMachine)
        {
            con = stateMachine;
        }

        public override void Enter()
        {           
            /* Spawn charge if not already spawned */
            if (con.busterChargeInstance == null || !con.busterChargeInstance.IsSpawned)
            {
                con.busterChargeInstance = con.CreateShot(con.busterChargePrefab, con.chargerTransform, false);
                SoundManager.PlaySound(con.chargedSound);
            }

        }

        public override void UpdateLogic()
        {
            /* If fire key is not pressed */
            if (!con.currentInput.IsBasicFireKeyPressed)
            {
                /* Despawn charge instance */
                NetworkObjectPool.Singleton.ReturnNetworkObject(con.busterChargeInstance, con.busterChargePrefab.gameObject);
                con.busterChargeInstance.Despawn();

                /* Spawn strong shot  */
                con.CreateShot(con.strongBusterShotPrefab, con.holderTransform, true);
                SoundManager.PlaySound(con.shootSound);

                /* Shoot strong shot */
                con.Shoot(con.transform.right, 100);

                /* Animate shooting */
                con.AnimateBasicFire(con.basicFireAnimateDuration);

                /* Set state to 'Idle' */
                con.ChangeState(con.IdleState);
            }
        }
    }

    public IdleFsmState IdleState { get; private set; }

    public ChargingFsmState ChargingState { get; private set; }

    public ReadyFsmState ReadyState { get; private set; }

    #endregion

    #region Public Editor Fields

    [Tooltip("Time it takes from 0 charge to max charge (in seconds)")]
    public float chargeTimeThreshold = 1f;

    [Tooltip("Duration of basic fire animation (in seconds)")]
    public float basicFireAnimateDuration = 1.1f;

    #endregion

    #region Private Editor Fields

    [SerializeField] private NetworkObject smallBusterShotPrefab;
    [SerializeField] private NetworkObject busterChargePrefab;
    [SerializeField] private NetworkObject strongBusterShotPrefab;
    [SerializeField] private Transform holderTransform;
    [SerializeField] private Transform chargerTransform;
    [SerializeField] private Sound chargingSound;
    [SerializeField] private Sound chargedSound;
    [SerializeField] private Sound shootSound;
    [SerializeField] private Sound shootHitSound;

    #endregion

    #region Prefab Instances

    private NetworkObject smallBusterShotInstance;
    private NetworkObject busterChargeInstance;

    #endregion

    #region Components

    public NetworkAnimator animator;
    private Entity entity;

    #endregion

    #region Private Fields

    private Coroutine shootRoutineInstance;
    private InputState currentInput;

    #endregion

    public override void OnNetworkSpawn()
    {
        entity = GetComponent<Entity>();
        IdleState = new IdleFsmState(this);
        ChargingState = new ChargingFsmState(this);
        ReadyState = new ReadyFsmState(this);
        SetInitialState(IdleState);
    }

    [ServerRpc]
    public void ChangeInputServerRpc(InputState input, ServerRpcParams serverRpcParams = default)
    {
        currentInput = input;
    }


    private void Shoot(Vector2 dir, int damage)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1000f, UltraMan.LayerMasks.Entity);
        Debug.DrawRay(transform.position, dir * 1000f, Color.green, 2f);

        if (hit.collider == null)
            return;

        if (!hit.collider.TryGetComponent(out DynamicEntity entity))
            throw new System.ApplicationException(hit.collider.name + " does not contain an Entity component");

        if (entity.StageSide.Value != entity.StageSide.Value)
            return;

        SoundManager.PlaySound(shootSound);
        entity.TakeDamage(damage);

    }

    private NetworkObject CreateShot(NetworkObject prefab, Transform parent, bool destroyOnAnimationFinish)
    {
        /* Get instance from network pool */    
        var instance = NetworkObjectPool.Singleton.GetNetworkObject(prefab.gameObject);
      
        /* Spawn instance */
        instance.transform.SetPositionAndRotation(parent.position, parent.rotation);
        instance.Spawn();
        instance.transform.parent = transform;

        /* Schedule despawn if specified to despawn on animation completion */
        if (destroyOnAnimationFinish && instance.TryGetComponent(out Animator animator))
            StartCoroutine(ScheduleDespawn(prefab, instance, animator.GetCurrentAnimatorStateInfo(0).length - 0.1f));

        /* Return instance */
        return instance;
    }

    private IEnumerator ScheduleDespawn(NetworkObject prefab, NetworkObject instance, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkObjectPool.Singleton.ReturnNetworkObject(instance, prefab.gameObject);
        instance.Despawn();
    }

    /// <summary> Animates basic fire shooting. </summary>
    /// <param name="duration"> Duration for animation in seconds. </param>
    private void AnimateBasicFire(float duration)
    {
        if (shootRoutineInstance != null)
            StopCoroutine(shootRoutineInstance);
        shootRoutineInstance = StartCoroutine(AnimateBasicFireCoroutine(duration));
    }

    /// <summary> Courotuine to animate basic fire shooting. </summary>
    /// <param name="duration"> Duration for animation in seconds. </param>
    private IEnumerator AnimateBasicFireCoroutine(float duration)
    {
        animator.Animator.SetBool(UltraMan.AnimationParameters.IsShootingBool, true);
        yield return new WaitForSeconds(duration);
        animator.Animator.SetBool(UltraMan.AnimationParameters.IsShootingBool, false);
    }
}
