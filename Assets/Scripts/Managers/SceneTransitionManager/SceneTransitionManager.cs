using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UltraMan.Managers
{
    [RequireComponent(typeof(Animator))]
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Singleton { get; private set; }
        private Animator animator;

        private void Awake()
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void SetSolid(bool state)
        {
            animator.SetBool("IsSolid", state);
        }
    }
}
