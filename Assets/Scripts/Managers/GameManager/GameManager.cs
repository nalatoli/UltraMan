using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UltraMan.Managers;
using UltraMan.Managers.GameManagerHelpers;
using UltraMan.Managers.SoundManagerHelpers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraMan
{
    public class GameManager : NetworkBehaviour
    {
        #region Properties

        /// <summary> Instance of GameManager. </summary>
        public static GameManager Singleton { get; private set; }

        [Tooltip("Song to play when battle starts.")]
        public Music song;

        [SerializeField] private bool useRelayServices = true;
        public bool UseRelayServices => useRelayServices;

        [SerializeField]
        private NetworkObject naviPrefab;

        private NetworkObject naviInstance;
        private bool matchReady = false;

        #endregion

        private void Awake()
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            Debug.Log(naviInstance.IsSpawned);
            if (matchReady && Input.GetKeyDown(KeyCode.R) && !naviInstance.IsSpawned)
            {
                Debug.Log("REQUESTED");
                RequestRespawnServerRpc(OwnerClientId);
            }
        }

        [ServerRpc]
        private void RequestRespawnServerRpc(ulong clientId)
        {
            naviInstance = CreatePlayerPrefab(naviPrefab, clientId == OwnerClientId ? Side.Left : Side.Right, clientId);
        }

        [ServerRpc]
        public void StartMatchServerRpc()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
            NetworkManager.Singleton.SceneManager.LoadScene("MatchScene", LoadSceneMode.Single);    
        }

        private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            RequestRespawnServerRpc(clientId);           
            EndTransitionClientRpc();
            matchReady = true;
        }

        [ClientRpc]
        private void EndTransitionClientRpc()
        {
            SceneTransitionManager.Singleton.SetSolid(false);
            SoundManager.PlayMusic(song);
        }

        private NetworkObject CreatePlayerPrefab(NetworkObject prefab, Side side, ulong clientId)
        {
            var navi = CreatePrefab(prefab, side);
            navi.SpawnAsPlayerObject(clientId);
            return navi;
        }

        private NetworkObject CreatePrefab(NetworkObject prefab, Side side)
        {
            var navi = Instantiate(prefab);
            navi.GetComponent<Entity>().StageSide.Value = side;
            navi.transform.position = side == Side.Right ? new Vector3(10f, -4.5f, 0) : new Vector3(-10f, -4.5f, 0);
            return navi;
        }
    }
}
