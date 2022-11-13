using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace UltraMan.Managers
{
    public class ConnectionManager : MonoBehaviour
    {
        /// <summary>
        /// RelayHostData represents the necessary information
        /// for a Host to host a game on a Relay
        /// </summary>
        public struct RelayHostData
        {
            public string JoinCode;
            public string IPv4Address;
            public ushort Port;
            public Guid AllocationID;
            public byte[] AllocationIDBytes;
            public byte[] ConnectionData;
            public byte[] Key;
        }

        /// <summary>
        /// RelayHostData represents the necessary information
        /// for a Host to host a game on a Relay
        /// </summary>
        public struct RelayJoinData
        {
            public string IPv4Address;
            public ushort Port;
            public Guid AllocationID;
            public byte[] AllocationIDBytes;
            public byte[] ConnectionData;
            public byte[] HostConnectionData;
            public byte[] Key;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary> Allocates a Relay server and returns needed data to host the game. </summary>
        /// <param name="maxConn">The maximum number of peer connections the host will allow. </param>
        /// <returns> A Task returning the needed hosting data. </returns>
        public static async Task<RelayHostData> HostGame(int maxConn)
        {
            /* Initialize the Unity Services engine */
            await InitializeServices();

            /* Ask Unity Services to allocate a Relay server */
            Allocation allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(maxConn);

            /* Create a default endpoint from the unsecure allocation's IP address + port */
            RelayServerEndpoint defaultEndpoint = GetSecureEndPoint(allocation.RelayServer.IpV4, allocation.RelayServer.Port, allocation.ServerEndpoints);

            /* Get new server end point from the default endpoint found */
            NetworkEndPoint serverEndpoint = NetworkEndPoint.Parse(defaultEndpoint.Host, (ushort)defaultEndpoint.Port);

            /* Populate the hosting data */
            RelayHostData data = new()
            {
                IPv4Address = defaultEndpoint.Host,
                Port = serverEndpoint.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                Key = allocation.Key,
            };

            /* Retrieve the Relay join code for the clients to join a party */
            data.JoinCode = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(data.AllocationID);

            /* Set transport host data */
            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            utp.SetHostRelayData(
                data.IPv4Address,
                data.Port,
                data.AllocationIDBytes,
                data.Key,
                data.ConnectionData,
                true);

            /* Return hosting data */
            return data;
        }

        /// <summary> Joins a Relay server based on the JoinCode received from the Host or Server. </summary>
        /// <param name="joinCode"> The join code on the host or server. </param>
        /// <returns> All the necessary data to connect. </returns>
        public static async Task<RelayJoinData> JoinGame(string joinCode)
        {
            /* Initialize the Unity Services engine */
            await InitializeServices();

            /* Ask Unity Services for allocation data based on a join code */
            JoinAllocation allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);

            /* Create a default endpoint from the unsecure allocation's IP address + port */
            RelayServerEndpoint defaultEndpoint = GetSecureEndPoint(allocation.RelayServer.IpV4, allocation.RelayServer.Port, allocation.ServerEndpoints);

            /* Get new server end point from the default endpoint found */
            NetworkEndPoint serverEndpoint = NetworkEndPoint.Parse(defaultEndpoint.Host, (ushort)defaultEndpoint.Port);

            /* Populate the joining data */
            RelayJoinData data = new()
            {
                IPv4Address = defaultEndpoint.Host,
                Port = serverEndpoint.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                Key = allocation.Key,
            };

            /* Set transport client data */
            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            utp.SetClientRelayData(
                data.IPv4Address,
                data.Port,
                data.AllocationIDBytes,
                data.Key,
                data.ConnectionData,
                data.HostConnectionData,
                true);

            /* Return joining data */
            return data;
        }

        /// <summary> Initializes Unity Services engine. </summary>
        private static async Task InitializeServices()
        {
            /* Initialize the Unity Services engine */
            await UnityServices.InitializeAsync();

            /* If this client is not signed in */
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                /* Sign in the client anonymously */
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        /// <summary> Gets secure UDP server endpoint. </summary>
        /// <param name="ipV4"> Default IPv4 address to use. </param>
        /// <param name="port"> Default port to use. </param>
        /// <param name="availableEndPoints"> Available endpoints to search within. </param>
        /// <returns> Secure Relay server endpoint. </returns>
        private static RelayServerEndpoint GetSecureEndPoint(string ipV4, int port, List<RelayServerEndpoint> availableEndPoints)
        {
            /* Create a default endpoint from the unsecure allocation's IP address + port */
            RelayServerEndpoint defaultEndPoint = new(
                "udp",
                RelayServerEndpoint.NetworkOptions.Udp,
                true,
                false,
                ipV4,
                port);

            /* Set the default end point to one that is secure and uses UDP (if available) */
            foreach (var endPoint in availableEndPoints)
            {
                if (endPoint.Secure == true && endPoint.Network == RelayServerEndpoint.NetworkOptions.Udp)
                    defaultEndPoint = endPoint;
            }

            /* Return default end point (new one if one is found) */
            return defaultEndPoint;
        }
    }
}
