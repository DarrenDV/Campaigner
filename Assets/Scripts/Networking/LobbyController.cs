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
    /*
     *  This heap of junk is used to update the player list in the lobby scene
     *  It's absolute garbage and is the main reason why I want to rewrite the lobby system
     * 
     */
    
    
    public static LobbyController Instance;
    
    public TextMeshProUGUI lobbyText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;
    
    public ulong lobbyID;
    public bool PlayerItemCreated = false;
    
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();

    public PlayerObjectController LocalPlayerController;
    
    [SerializeField] private GameObject startGameButton;
    
    
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
        NetworkManager.GamePlayers.CollectionChanged += UpdatePlayerList;
    }
    
    private void OnDisable()
    {
        NetworkManager.GamePlayers.CollectionChanged -= UpdatePlayerList;
    }
    
    public void UpdateLobbyName()
    {
        lobbyID = NetworkManager.GetComponent<SteamLobby>().lobbyID;
        lobbyText.text = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyID), "name");
    }

    /*
     * This function causes all of my pain.
     * Normally this function should just be called from the playerobjectcontroller when a player joins, leaves or updates their name.
     * This does not work however because this function is dependedent on the NetworkManager.GamePlayers list.
     * Whilst the list works it's updated slower then when the function would normally be called.
     * This causes the function to be ran before the list is updated and thus the list is not updated.
     *
     * This function now runs every time the list is updated and checks if the list is the same size as the list of players.
     * This works but I hate it
     */
    public void UpdatePlayerList(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }
        
        if(PlayerListItems.Count < NetworkManager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }

        if(PlayerListItems.Count > NetworkManager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }
        
        if(PlayerListItems.Count == NetworkManager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
        
        //UpdateUIElements();
    }

    public void UpdateUIElements()
    {
        if (LocalPlayerController != null)
        {
            if(LocalPlayerController.PlayerType == PlayerType.DungeonMaster)
            {
                startGameButton.SetActive(true);
            }
        }
    }

    //This code is buggy so I decided to just set the Local player from it's own class. 
    
    // public void FindLocalPlayer()
    // {
    //     LocalPlayerObject = GameObject.Find("LocalGamePlayer"); //TODO Change to SteamFriends.GetPersonaName()
    //     LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    // }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in NetworkManager.GamePlayers)
        {

            GameObject playerItem = Instantiate(PlayerListItemPrefab);
            PlayerListItem playerListItem = playerItem.GetComponent<PlayerListItem>();

            playerListItem.PlayerName = player.PlayerName;
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
        foreach (PlayerObjectController player in NetworkManager.GamePlayers)
        {
            if(!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject playerItem = Instantiate(PlayerListItemPrefab);
                PlayerListItem playerListItem = playerItem.GetComponent<PlayerListItem>();
            
                playerListItem.PlayerName = player.PlayerName;
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
        foreach (PlayerObjectController player in NetworkManager.GamePlayers)
        {
            foreach (PlayerListItem playerListItem in PlayerListItems)
            {
                if (playerListItem.ConnectionID == player.ConnectionID)
                {
                    playerListItem.PlayerName = player.PlayerName;
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
            if (!NetworkManager.GamePlayers.Any(b => b.ConnectionID == playerListItem.ConnectionID))
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