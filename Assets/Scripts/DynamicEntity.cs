using System;
using System.Collections;
using System.Collections.Generic;
using UltraMan.Managers;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace UltraMan
{
    [RequireComponent(typeof(Collider2D))]
    public class DynamicEntity : Entity
    {
        public int MaxHealth = 200;
        private NetworkVariable<int> currentHealth = new();
        public int CurrentHealth => currentHealth.Value;
        private NetworkAnimator animator;
        private Collider2D collider2d;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            currentHealth.Value = MaxHealth;
            animator = GetComponent<NetworkAnimator>();
            collider2d = GetComponent<Collider2D>();
        }

        public int TakeDamage(int damage)
        {
            currentHealth.Value = Math.Max(0, currentHealth.Value - damage);
            SoundManager.PlaySound("Damage");
            if (currentHealth.Value <= 0)
                Kill(1);
            return currentHealth.Value;
        }

        public void Kill(float wait)
        {
            collider2d.enabled = false;
            SoundManager.PlaySound("Deleted");
            AnimateDeathClientRpc();
            Destroy(gameObject, wait);
        }

        [ClientRpc]
        private void AnimateDeathClientRpc()
        {
            animator.SetTrigger(AnimationParameters.OnDeathTrigger);
        }
    }
}
