using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Rendering;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;
    
    public TextMeshProUGUI lobbyText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;
    
    public ulong lobbyID;
    public bool PlayerItemCreated = false;
    
    
    
    
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
        
        
        
        
    public PlayerObjectController LocalPlayerController;
    
    private CustomNetworkManager networkManager;
    private CustomNetworkManager NetworkManager
    {
        get
        {
            if (networkManager != null) { return networkManager; }
            return networkManager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        NetworkManager.Players.CollectionChanged += UpdatePlayerList;
    }
    
    private void OnDisable()
    {
        NetworkManager.Players.CollectionChanged -= UpdatePlayerList;
    }

    public void UpdateLobbyName()
    {
        lobbyID = NetworkManager.GetComponent<SteamLobby>().lobbyID;
        lobbyText.text = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyID), "name");
    }

    public void UpdatePlayerList(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }
        
        if(PlayerListItems.Count < NetworkManager.Players.Count)
        {
            CreateClientPlayerItem();
        }

        if(PlayerListItems.Count > NetworkManager.Players.Count)
        {
            RemovePlayerItem();
        }
        
        if(PlayerListItems.Count == NetworkManager.Players.Count)
        {
            UpdatePlayerItem();
        }
        
    }
    
    // public void UpdatePlayerList()
    // {
    //     if (!PlayerItemCreated)
    //     {
    //         CreateHostPlayerItem();
    //     }
    //     
    //     if(PlayerListItems.Count < NetworkManager.Players.Count)
    //     {
    //         CreateClientPlayerItem();
    //     }
    //
    //     if(PlayerListItems.Count > NetworkManager.Players.Count)
    //     {
    //         RemovePlayerItem();
    //     }
    //     
    //     if(PlayerListItems.Count == NetworkManager.Players.Count)
    //     {
    //         UpdatePlayerItem();
    //     }
    //     
    // }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer"); //TODO Change to SteamFriends.GetPersonaName()
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in NetworkManager.Players)
        {

            GameObject playerItem = Instantiate(PlayerListItemPrefab);
            PlayerListItem playerListItem = playerItem.GetComponent<PlayerListItem>();
            
            playerListItem.PlayerName = player.PlayerName;
            playerListItem.PlayerSteamID = player.PlayerSteamID;
            playerListItem.ConnectionID = player.ConnectionID;
            playerListItem.SetPlayerValues();
            
            playerItem.transform.SetParent(PlayerListViewContent.transform);
            playerItem.transform.localScale = Vector3.one;
            
            PlayerListItems.Add(playerListItem);
        }
        
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in NetworkManager.Players)
        {
            if(PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject playerItem = Instantiate(PlayerListItemPrefab);
                PlayerListItem playerListItem = playerItem.GetComponent<PlayerListItem>();
            
                playerListItem.PlayerName = player.PlayerName;
                playerListItem.PlayerSteamID = player.PlayerSteamID;
                playerListItem.ConnectionID = player.ConnectionID;
                playerListItem.SetPlayerValues();
            
                playerItem.transform.SetParent(PlayerListViewContent.transform);
                playerItem.transform.localScale = Vector3.one;
            
                PlayerListItems.Add(playerListItem);
            }
        }
        
        PlayerItemCreated = true;
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in NetworkManager.Players)
        {
            foreach (PlayerListItem playerListItem in PlayerListItems)
            {
                if (playerListItem.ConnectionID == player.ConnectionID)
                {
                    playerListItem.PlayerName = player.PlayerName;
                    playerListItem.PlayerSteamID = player.PlayerSteamID;
                    playerListItem.ConnectionID = player.ConnectionID;
                    playerListItem.SetPlayerValues();
                }
            }

        }
    }
    
    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();
        
        foreach (PlayerListItem playerListItem in PlayerListItems)
        {
            if (!NetworkManager.Players.Any(b => b.ConnectionID == playerListItem.ConnectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }
        
        if(playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItem in playerListItemsToRemove)
            {
                PlayerListItems.Remove(playerListItem);
                Destroy(playerListItem.gameObject);
            }
        }
    }


}
