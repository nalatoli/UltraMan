using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltraMan.Managers.GameManagerHelpers;

namespace UltraMan
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] RectTransform debugUi;
        [SerializeField] RectTransform mainMenuUi;

        private void Start()
        {
            //GameManager.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.Uninitialzed || newState == GameState.InMainMenu)
            {
                debugUi.gameObject.SetActive(false);
                mainMenuUi.gameObject.SetActive(true);
            }

            else if(newState == GameState.InGame)
            {
                debugUi.gameObject.SetActive(true);
                mainMenuUi.gameObject.SetActive(false);
            }
        }
    }
}
