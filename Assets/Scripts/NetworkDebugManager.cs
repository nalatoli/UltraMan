using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace UltraMan
{
    public class NetworkDebugManager : MonoBehaviour
    {
        [SerializeField]
        private Entity leftSideTile;

        [SerializeField]
        private Entity rightSideTile;

        [SerializeField]
        private NetworkObject naviPrefab;

        private List<NetworkObject> instances = new List<NetworkObject>();

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                if (NetworkManager.Singleton.IsServer)
                    SubmitSpawn();
            }

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        //static void SubmitNewPosition()
        //{
        //    if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
        //    {
        //        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        //        {
        //            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
        //                NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
        //        }
        //        else
        //        {
        //            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        //            var player = playerObject.GetComponent<HelloWorldPlayer>();
        //            player.Move();
        //        }
        //    }
        //}

        void SubmitSpawn()
        {      
            if (GUILayout.Button("Spawn"))
            {
                foreach(var instance in instances)
                    instance.Despawn();
                instances.Clear();


                if (NetworkManager.Singleton.ConnectedClients.Count == 0)
                {
                    Debug.Log("Skipping spawn - No client connected");
                }

                else if(NetworkManager.Singleton.ConnectedClients.Count == 1)
                {
                    var clientId = NetworkManager.Singleton.ConnectedClients.First().Value.ClientId;
                    Debug.Log("Spawning one navi on the left for " + clientId + " at " + leftSideTile.transform.position);
                    var navi = Spawn(clientId, Side.Left);
                    instances.Add(navi);

                }

                else if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
                {
                    var clientId1 = NetworkManager.Singleton.ConnectedClients.First().Value.ClientId;
                    var clientId2 = NetworkManager.Singleton.ConnectedClients.Last().Value.ClientId;
                    Debug.Log("Spawning one navi on the left for " + clientId1 + " at " + leftSideTile.transform.position + 
                        " and one on the right" + clientId2 + " at " + rightSideTile.transform.position);
                    var navi1 = Spawn(clientId1, Side.Left);
                    var navi2 = Spawn(clientId2, Side.Right);
                    instances.Add(navi1);
                    instances.Add(navi2);
                }
            }
        }

        private NetworkObject Spawn(ulong clientId, Side side)
        {
            var navi = Instantiate(naviPrefab);
            navi.GetComponent<Entity>().StageSide = side;
            navi.GetComponent<MovementController>().Position.Value = side == Side.Right ? rightSideTile.transform.position : leftSideTile.transform.position;
            navi.SpawnAsPlayerObject(clientId);
            return navi;
        }
    }
}