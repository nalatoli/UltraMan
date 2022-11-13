using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UltraMan.Managers;
using Unity.Netcode;
using Mono.Cecil.Cil;
using System.Threading.Tasks;

namespace UltraMan
{
    public class MainMenu : NetworkBehaviour
    {
        #region Properties

        [SerializeField] RectTransform hostGroup;
        [SerializeField] RectTransform configGroup;
        [SerializeField] RectTransform clientGroup;
        [SerializeField] TextMeshProUGUI hostGroupCodeTextField;
        [SerializeField] TextMeshProUGUI hostGroupStatusTextField;
        [SerializeField] TMP_InputField clientGroupCodeInputField;
        [SerializeField] TextMeshProUGUI clientGroupStatusTextField;

        #endregion

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            if(!GameManager.Singleton.UseRelayServices)
            {
                clientGroupCodeInputField.text = "(debug)";
                clientGroupCodeInputField.enabled = false;
            }
        }

        private async void OnClientConnected(ulong obj)
        {
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                SetTransitionClientRpc();
                await Task.Delay(1000);
                GameManager.Singleton.StartMatchServerRpc();
            }
        }

        [ClientRpc]
        private void SetTransitionClientRpc()
        {
            SceneTransitionManager.Singleton.SetSolid(true);
        }

        public async void OnHostConfigClick()
        {
            /* Show Host menu */
            ShowHostGroup();

            /* Set status */
            hostGroupCodeTextField.text = "";
            hostGroupStatusTextField.text = "Creating server";

            try
            {
                if(GameManager.Singleton.UseRelayServices)
                {
                    /* Allocate game on the Relay server and get join code */
                    string code = (await ConnectionManager.HostGame(2)).JoinCode;

                    /* Make this instance a Host */
                    NetworkManager.Singleton.StartHost();

                    /* Set status */
                    hostGroupCodeTextField.text = code;
                    hostGroupStatusTextField.text = "Waiting for opponent";
                }

                else
                {
                    /* Make this instance a Host */
                    NetworkManager.Singleton.StartHost();

                    /* Set status */
                    hostGroupCodeTextField.text = "(debug)";
                    hostGroupStatusTextField.text = "Waiting for opponent";
                }
            }

            catch
            {
                hostGroupStatusTextField.text = "Unable to create server";
                throw;
            }
        }

        public void OnClientConfigClick()
        {
            /* Show Client menu */
            ShowClientGroup();

            /* Set status */
            clientGroupCodeInputField.text = "";
            clientGroupStatusTextField.text = "";

        }

        public void OnBackClick()
        {
            ShowConfigGroup();
        }

        public async void OnClientConnectClick()
        {
            try
            {
                if (GameManager.Singleton.UseRelayServices)
                {
                    /* Get join code input */
                    string code = clientGroupCodeInputField.text;

                    /* Allocate game on the Relay server and get join code */
                    await ConnectionManager.JoinGame(code);
                }

                /* Make this instance a Host */
                NetworkManager.Singleton.StartClient();

                /* Set status */
                clientGroupStatusTextField.text = "Starting match";

            }

            catch
            {
                clientGroupStatusTextField.text = "Unable to join game";
                throw;
            }
        }

        private void ShowConfigGroup()
        {
            hostGroup.gameObject.SetActive(false);
            configGroup.gameObject.SetActive(true);
            clientGroup.gameObject.SetActive(false);
        }

        private void ShowHostGroup()
        {
            hostGroup.gameObject.SetActive(true);
            configGroup.gameObject.SetActive(false);
            clientGroup.gameObject.SetActive(false);
        }

        private void ShowClientGroup()
        {
            hostGroup.gameObject.SetActive(false);
            configGroup.gameObject.SetActive(false);
            clientGroup.gameObject.SetActive(true);
        }

        private void HideAll()
        {
            hostGroup.gameObject.SetActive(false);
            configGroup.gameObject.SetActive(false);
            clientGroup.gameObject.SetActive(false);
        }
    }
}
