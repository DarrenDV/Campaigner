using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using System.Linq;

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

            if (NetworkServer.active)
            {
                player.PlayerType = PlayerType.DungeonMaster;
            }
            else
            {
                player.PlayerType = PlayerType.Player;
            }
            
            

            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        }
    }

    public override void Start()
    {
        base.Start();
        List<GameObject> placeableObjects = new List<GameObject>();
        
        placeableObjects = Resources.LoadAll<GameObject>("Prefabs").ToList();

        foreach (GameObject prefab in placeableObjects)
        {
            spawnPrefabs.Add(prefab);
        }
        
    }
}
