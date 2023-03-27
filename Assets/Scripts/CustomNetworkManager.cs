using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController playerObjectController;
    public List<PlayerObjectController> Players = new List<PlayerObjectController>();
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController player = Instantiate(playerObjectController);
            player.ConnectionID = conn.connectionId;
            player.PlayerID = Players.Count + 1;
            player.PlayerSteamID = SteamUser.GetSteamID().m_SteamID;
            
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        }
    }
}
