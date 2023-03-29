using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using System.Linq;

public class CustomNetworkManager : NetworkManager
{
    
    
    public static CustomNetworkManager Instance => singleton as CustomNetworkManager;
    
    
    [SerializeField] private PlayerObjectController playerObjectController;

    [SerializeField] private List<PlayerObjectController> players; //I just use this List to look at GamePlayers in the inspector
    
    public ObservableCollection<PlayerObjectController> GamePlayers = new ObservableCollection<PlayerObjectController>();
    
    [SerializeField] public PlayerObjectController localPlayer; //The local player object can be used to get a direct reference to their rights
    
    
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) //This function is being called when a player joins a lobby.
    {
        Debug.Log("OnServerAddPlayer");
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController player = Instantiate(playerObjectController);
            
            DontDestroyOnLoad(player);
            
            player.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.lobbyID, GamePlayers.Count);
            player.ConnectionID = conn.connectionId;
            player.PlayerID = GamePlayers.Count + 1;

            if (player.PlayerID == 1) //Only the host is the dungeon master
            {
                player.PlayerType = PlayerType.DungeonMaster;
            }
            else
            {
                player.PlayerType = PlayerType.Player;
            }
            
            //Check if the player is the local player and set the localPlayer variable
            if (player.PlayerSteamID == SteamUser.GetSteamID().m_SteamID)
            {
                localPlayer = player;
            }
            
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        }
    }

    public override void Start() //TODO: Move this functionality somewhere else
    {
        base.Start();
        List<GameObject> placeableObjects = new List<GameObject>();
        
        placeableObjects = Resources.LoadAll<GameObject>("Prefabs").ToList();

        foreach (GameObject prefab in placeableObjects)
        {
            spawnPrefabs.Add(prefab);
        }
        
    }

    
    //These 3 are just used to look at GamePlayers in the inspector
    private void OnEnable()
    {
        GamePlayers.CollectionChanged += UpdateInspectorList;
    }

    private void OnDisable()
    {
        GamePlayers.CollectionChanged -= UpdateInspectorList;
    }

    private void UpdateInspectorList(object sender, NotifyCollectionChangedEventArgs e)
    {
        players.Clear();
        players = GamePlayers.ToList();
    }
}
