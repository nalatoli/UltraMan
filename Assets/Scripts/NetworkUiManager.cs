using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUiManager : NetworkBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button requestSpawnLeftButton;
    [SerializeField] private Button spawnRightButton;
    [SerializeField] private Button requestSpawnRightButton;
    [SerializeField] private Button spawnLeftButton;
    private Transform configGroup;
    private Transform serverGroup;
    private Transform clientGroup;

    [SerializeField]
    private Entity leftSideTile;

    [SerializeField]
    private Entity rightSideTile;

    [SerializeField]
    private NetworkObject naviPrefab;

    private NetworkObject naviInstance;

    private void Awake()
    {
        configGroup = (RectTransform)serverButton.GetComponent<Transform>().parent;
        serverGroup = (RectTransform)spawnLeftButton.GetComponent<Transform>().parent;
        clientGroup = (RectTransform)requestSpawnLeftButton.GetComponent<Transform>().parent;

        serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer() );
        hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        spawnLeftButton.onClick.AddListener(() => OnSpawnLeftClick());
        requestSpawnLeftButton.onClick.AddListener(() => OnRequestSpawnLeftClick());
        spawnRightButton.onClick.AddListener(() => OnSpawnRightClick());
        requestSpawnRightButton.onClick.AddListener(() => OnRequestSpawnRightClick());
    }

    private void Update()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            configGroup.gameObject.SetActive(false);
            serverGroup.gameObject.SetActive(true);
            clientGroup.gameObject.SetActive(false);
        }

        else if (NetworkManager.Singleton.IsClient)
        {
            configGroup.gameObject.SetActive(false);
            serverGroup.gameObject.SetActive(false);
            clientGroup.gameObject.SetActive(true);
        }

        else
        {
            configGroup.gameObject.SetActive(true);
            serverGroup.gameObject.SetActive(false);
            clientGroup.gameObject.SetActive(false);
        }
    }

    private void OnRequestSpawnLeftClick()
    {
        RequestSpawnServerRpc(Side.Left);
    }

    private void OnSpawnLeftClick()
    {
        RequestSpawnServerRpc(Side.Left);
    }

    private void OnRequestSpawnRightClick()
    {
        RequestSpawnServerRpc(Side.Right);
    }

    private void OnSpawnRightClick()
    {
        RequestSpawnServerRpc(Side.Right);
    }

    [ServerRpc(RequireOwnership =false)]
    private void RequestSpawnServerRpc(Side side, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Requested spawning on " + side + " for client " + serverRpcParams.Receive.SenderClientId);

        naviInstance = CreateNavi(side);
        naviInstance.SpawnAsPlayerObject(serverRpcParams.Receive.SenderClientId);
    }

    private NetworkObject CreateNavi(Side side)
    {
        var navi = Instantiate(naviPrefab);
        navi.GetComponent<Entity>().StageSide.Value = side;
        navi.transform.position = side == Side.Right ? rightSideTile.transform.position : leftSideTile.transform.position;
        return navi;
    }
}
