using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController playerObjectController;
    public ObservableCollection<PlayerObjectController> GamePlayers {  get;  } = new ObservableCollection<PlayerObjectController>();
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController player = Instantiate(playerObjectController);
            player.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.lobbyID, GamePlayers.Count);
            player.ConnectionID = conn.connectionId;
            player.PlayerID = GamePlayers.Count + 1;

            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        }
    }
}
