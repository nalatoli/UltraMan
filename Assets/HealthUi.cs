using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UltraMan
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class HealthUi : MonoBehaviour
    {
        [Range(1f, 20f)]
        public float changeDuration = 0.5f;

        [SerializeField] DynamicEntity entity;
        TextMeshProUGUI tmp;
        Animator animator;
        private int previousHealth = 0;
        private Coroutine routineInstance = null;

        private void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            animator = GetComponent<Animator>();
            previousHealth = entity.CurrentHealth;
            tmp.text = previousHealth.ToString();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (previousHealth != entity.CurrentHealth && routineInstance == null)
                routineInstance = StartCoroutine(UpdateHealth());
        }

        IEnumerator UpdateHealth()
        {
            while (previousHealth != entity.CurrentHealth)
            {
                if(entity.CurrentHealth < previousHealth)
                {
                    previousHealth = Math.Max(
                        previousHealth - (int)Math.Ceiling((previousHealth - entity.CurrentHealth) / changeDuration),
                        entity.CurrentHealth);
                    animator.SetInteger("HealthChange", -1);
                }
                else
                {
                    previousHealth = Math.Min(
                        previousHealth + (int)Math.Ceiling((entity.CurrentHealth - previousHealth) / changeDuration), 
                        entity.CurrentHealth);
                    animator.SetInteger("HealthChange", 1);
                }

                tmp.text = previousHealth.ToString();
                yield return new WaitForFixedUpdate();
            }

            animator.SetInteger("HealthChange", 0);
            routineInstance = null;

        }
    }
}
